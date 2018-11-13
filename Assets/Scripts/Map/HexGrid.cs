using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Collections;

public class HexGrid : SingletonDestory<HexGrid> {

	int width = 6;
	int height = 6;

    int oldCountX = 0;
    int oldCountZ = 0;

    public int chunkCountX = 4;
    public int chunkCountZ = 4;

    int cellCountHeight = 0;
    int cellCountWidth = 0;

	public Color defaultColor = Color.white;

    //不规则噪声
    public Texture2D noiseSource;

    HexGridChunk gridChunkPerfab;

    private HexEdgeMesh hexEdgeMesh;
    public HexCell cellPrefab;
	public Text cellLabelPrefab;

    public HexEdgeMesh HexEditMesh
    {
        get
        {
            if(hexEdgeMesh==null)
            {
                hexEdgeMesh = (Instantiate(Resources.Load("Prefabs/Hex Edge Mesh") as GameObject)).GetComponent<HexEdgeMesh>();
                hexEdgeMesh.Init();
                hexEdgeMesh.name = "Hex Edge Mesh";
            }
            return hexEdgeMesh;
        }
    }

	public HexCell[] cells;

    List<HexCell> allCellList;
    public List<HexCell> AllCellList
    {
        get
        {
            if (allCellList == null && cells != null)
            {
                allCellList = new List<HexCell>();
                for (int i = 0; i <cells.Length; i++)
                {
                    allCellList.Add(cells[i]);
                }
            }
            return allCellList;
        }
    }


    HexGridChunk[] chunks;
    Canvas gridCanvas;
    //HexMesh hexMesh;


    void Start() {
        //ConfigDateManage.instance.InitData();
        //oldCountX = chunkCountX;
        //oldCountZ = chunkCountZ;
        //chunks = new HexGridChunk[chunkCountX * chunkCountZ];
        //NewMap();

    }

    public void DistanceToOther(HexCell cell)
    {
        StopAllCoroutines();
        StartCoroutine(Search(cell));
    }

    IEnumerator Search(HexCell cell)
    {
        WaitForSeconds delay = new WaitForSeconds(1 / 60f);

        for (int i = 0; i < cells.Length; i++)
        {

            yield return delay;

            cells[i].DistanceWithOthers =cell.coordinates.DistanceToOther(cells[i].coordinates);

        }
    }
    //改变地图尺寸
    public void ChangeSize(int width,int height)
    {
        chunkCountX = width;
        chunkCountZ = height;
    }

    IEnumerator waitInstantiate()
    {
        while(cellPrefab==null||cellLabelPrefab==null||noiseSource==null)
        {
            yield return null;
        }
    }

    //新建地图
    public void NewMap()
    {
        if (!HexTerrain.isEditor)
        {
            gridCanvas = (Instantiate(Resources.Load("Prefabs/UIPrefabs/Hex Grid Canvas") as GameObject)).GetComponent<Canvas>();
            gridCanvas.transform.SetParent(transform);
        }

        cellPrefab = (Instantiate(Resources.Load("Prefabs/Hex Cell") as GameObject)).GetComponent<HexCell>();
        cellPrefab.name = "Hex Cell";
        cellLabelPrefab = (Instantiate(Resources.Load("Prefabs/UIPrefabs/Hex Cell Label") as GameObject)).GetComponent<Text>();
        cellLabelPrefab.name = "Hex Cell Label";
        noiseSource = Resources.Load("Texture/Noise") as Texture2D;

        StartCoroutine(waitInstantiate());
        //chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        if (HexMetrics.instance.isEditorTexture)
        {
            gridChunkPerfab = (Instantiate(Resources.Load("Prefabs/Hex Grid Chunk") as GameObject)).GetComponent<HexGridChunk>();
            gridChunkPerfab.name = "Hex Grid Chunk Prefab";
        }
        else
        {
            gridChunkPerfab = (Instantiate(Resources.Load("Prefabs/Hex Grid Chunk Color") as GameObject)).GetComponent<HexGridChunk>();
            gridChunkPerfab.name = "Hex Grid Chunk Color Prefab";
        }

        for(int i=0;i< oldCountX * oldCountZ;i++)
        {
            if (chunks[i] != null)
            {
                chunks[i].Clear();
                Destroy(chunks[i].gameObject);
            }
        }

        HexMetrics.noiseSource = noiseSource;
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];
        width = HexMetrics.instance.chunkWidth;
        height = HexMetrics.instance.chunkHeight;
        oldCountX = chunkCountX;
        oldCountZ = chunkCountZ;
        cellCountWidth = width * chunkCountX;
        cellCountHeight = height * chunkCountZ;

        cells = new HexCell[cellCountWidth * cellCountHeight];

        for (int j = 0, i = 0; j < chunkCountZ; j++)
        {
            for (int k = 0; k < chunkCountX; k++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(gridChunkPerfab) as HexGridChunk;
                //chunk.transform.SetParent(GameObject.Find("Map").transform);
                chunk.transform.SetParent(transform);
                chunk.transform.localPosition = new Vector3(chunk.transform.localPosition.x, 0, chunk.transform.localPosition.z);
            }
        }


        for (int j = 0, i = 0; j < cellCountHeight; j++)
        {
            for (int k = 0; k < cellCountWidth; k++)
            {
                CreateCell(k, j, i++);
            }
        }

        Refresh();
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)chunkCountX);
        writer.Write((byte)chunkCountZ);
        writer.Write(HexMetrics.instance.isEditorTexture);
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Save(writer);
        }

        for(int i=0;i< chunks.Length;i++)
        {
            chunks[i].Save(writer);
        }
    }

    public void Load(BinaryReader reader)
    {
        chunkCountX = reader.ReadByte();
        chunkCountZ = reader.ReadByte();
        HexMetrics.instance.isEditorTexture = reader.ReadBoolean();
        NewMap();
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Load(reader);
        }

        List<SceneObjectClass> itemClassList = new List<SceneObjectClass>();
        GameObject sceneObject;
        for (int i = 0; i < chunks.Length; i++)
        {
            itemClassList = chunks[i].Load(reader);
            for (int j = 0; j < itemClassList.Count; j++)
            {
                Vector3 itemPosition = chunks[i].transform.TransformPoint(itemClassList[j].position);
                itemClassList[j].cell = GetCell(itemPosition);
                itemClassList[j].direction = GetPointDirection(new Vector2(itemPosition.x - itemClassList[j].cell.transform.position.x, itemPosition.z - itemClassList[j].cell.transform.position.z));
                sceneObject = GameObject.Instantiate(Resources.Load(itemClassList[j].sceneObjectInfo.modelPath) as GameObject);
                sceneObject.AddComponent<SceneObjectClass>();
                sceneObject.GetComponent<SceneObjectClass>().SetInfo(itemClassList[j].position, itemClassList[j].rotation, itemClassList[j].direction, itemClassList[j].cell);
                sceneObject.GetComponent<SceneObjectClass>().sceneObjectInfo = itemClassList[j].sceneObjectInfo;
                sceneObject.transform.SetParent(chunks[i].sceneObjectMgr.transform);
                sceneObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                sceneObject.tag = "SceneObject";
                sceneObject.SetActive(true);
                sceneObject.GetComponent<SceneObjectClass>().Refresh(true);
                sceneObject.AddComponent<BoxCollider>();
                sceneObject.GetComponent<BoxCollider>().size = new Vector3(150, 150, 150);
                sceneObject.GetComponent<BoxCollider>().center = new Vector3(0, 150, 0);
                chunks[i].sceneObjectMgr.AddSceneObject(sceneObject.GetComponent<SceneObjectClass>());
            }

        }


    }


    public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);//坐标转换
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * cellCountWidth + coordinates.Z / 2;
		return cells[index];
	}

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        int x = coordinates.X + z / 2;
        if (z < 0 || x < 0 || z >= cellCountHeight || x >= cellCountWidth)
            return null;
        return cells[x + z * cellCountWidth];

    }

    public HexGridChunk GetChunk(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);//坐标转换
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountWidth + coordinates.Z / 2;
        return cells[index].chunkParent;
    }


    public bool IsClickInEdge(HexCell cell,Vector3 position)
    {
        if(Vector2.Distance(new Vector2(cell.transform.position.x,cell.transform.position.z),new Vector2(position.x,position.z))> HexMetrics.instance.innerRadius * HexMetrics.instance.solidFactor)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //获取鼠标当前所在的方向
    public HexDirection GetPointDirection(Vector2 position)
    {
        float angle = Vector2.Angle(position, new Vector2(0, 1));
        int dic = Mathf.RoundToInt(angle) / 60;
        if(dic == 0)
        {
            if(position.x > 0)
            {
                return HexDirection.NE;
            }
            else
            {
                return HexDirection.NW;
            }
        }
        else if(dic == 1)
        {
            if(position.x > 0)
            {
                return HexDirection.E;
            }
            else
            {
                return HexDirection.W;
            }
        }
        else
        {
            if(position.x > 0)
            {
                return HexDirection.SE;
            }
            else
            {
                return HexDirection.SW;
            }
        }

    }

	public void Refresh () {
        for (int j = 0, i = 0; j < chunkCountZ; j++)
        {
            for (int k = 0; k < chunkCountX; k++)
            {
                chunks[i++].Refresh();
            }

        }
       // hexMesh.Triangulate(cells);
	}

	void CreateCell (int x, int z, int i) {
        Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.instance.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.instance.outerRadius * 1.5f);


        HexCell cell = cells[i] = Instantiate(cellPrefab) as HexCell;
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.color = defaultColor;
       

        //设置邻居
        if (x > 0) {
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
		}
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor(HexDirection.SE, cells[i - cellCountWidth]);
				if (x > 0) {
					cell.SetNeighbor(HexDirection.SW, cells[i - cellCountWidth - 1]);
				}
			}
			else {
				cell.SetNeighbor(HexDirection.SW, cells[i - cellCountWidth]);
				if (x < cellCountWidth - 1) {
					cell.SetNeighbor(HexDirection.SE, cells[i - cellCountWidth + 1]);
				}
			}
		}

        if (!HexTerrain.isEditor)
        {
            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition3D = new Vector3(position.x, position.z, cell.transform.position.y);
            cell.label = label;
            cell.uiRect = label.rectTransform;
        }

        cell.isStepDirection = new bool[]{false,false,false,false,false,false};
        CellToChunk(x, z, cell);
        cell.Elevation = 0;

    }

    void CellToChunk(int x,int z,HexCell cell)
    {
        int chunkX = x / width;
        int chunkZ = z / height;

        HexGridChunk tmpChunk = chunks[chunkZ * chunkCountX + chunkX];

        int cellOfChunkX = x - x / width * width;
        int cellOfChunkZ = z - z / height * height;
        tmpChunk.AddCell(cellOfChunkZ * width + cellOfChunkX, cell);
        cell.SetParentChunk(tmpChunk);
    }

    public List<HexCell> GetRangeCells(HexCell cell,int range)
    {
        List<HexCell> cells = new List<HexCell>();
        int centerX = cell.coordinates.X;
        int centerZ = cell.coordinates.Z;
        for (int l = 0, z = centerZ; z >= centerZ - range + 1; l++, z--)
        {
            for (int x = centerX - range + 1 + l; x <= centerX + range - 1; x++)
            {
                if (HexGrid.instance.GetCell(new HexCoordinates(x, z)) != null)
                {
                    cells.Add(HexGrid.instance.GetCell(new HexCoordinates(x, z)));
                }
            }
        }

        for (int l = 1, z = centerZ + 1; z <= centerZ + range - 1; l++, z++)
        {
            for (int x = centerX - range + 1; x <= centerX + range - 1 - l; x++)
            {
                cells.Add(HexGrid.instance.GetCell(new HexCoordinates(x, z)));
            }
        }

        return cells;
    }
}