using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HexGrid : SingletonDestory<HexGrid> {

    public bool isLoadPrefab = false;

	int width = 6;
	int height = 6;

    int oldCountX = 0;
    int oldCountZ = 0;

    public int chunkCountX = 4;
    public int chunkCountZ = 4;

    public int ChunkHeight
    {
        get
        {
            return (int)(chunkCountZ * 2 * HexMetrics.instance.outerRadius)+1;   
        }
    }

    public int ChunkWidth
    {
        get
        {
            return (int)((chunkCountX + 0.5f) * 2 * HexMetrics.instance.innerRadius)+1;
        }
    }

    int cellCountHeight = 0;
    int cellCountWidth = 0;

    //不规则噪声
    public Texture2D noiseSource;

    HexMap mapPrefab;
    HexGridChunk gridChunkPerfab;
    HexCell cellPrefab;
    Text cellLabelPrefab;

    private HexEdgeMesh hexEdgeMesh;

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


    public HexMap[] maps;
    [HideInInspector]
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


    public HexGridChunk[] chunks;
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
        while(cellPrefab==null||cellLabelPrefab==null||noiseSource==null
            ||gridChunkPerfab == null || mapPrefab == null)
        {
            yield return null;
        }
    }

    //新建地图
    public void NewMap(string mapName = "Hex Map", bool isNewMaterial = false, Texture2D[] terrainTexs = null,int chunkWidth = 0,int chunkHeight = 0)
    {
        if (!HexMetrics.instance.isEditor)
        {
            gridCanvas = (Instantiate(Resources.Load("Prefabs/UIPrefabs/Hex Grid Canvas") as GameObject)).GetComponent<Canvas>();
            gridCanvas.transform.SetParent(transform);
        }
        mapPrefab = (Instantiate(Resources.Load("Prefabs/Hex Map") as GameObject)).GetComponent<HexMap>();
        mapPrefab.name = "HexMap";
        cellPrefab = (Instantiate(Resources.Load("Prefabs/Hex Cell") as GameObject)).GetComponent<HexCell>();
        cellPrefab.name = "HexCell";
        cellLabelPrefab = (Instantiate(Resources.Load("Prefabs/UIPrefabs/Hex Cell Label") as GameObject)).GetComponent<Text>();
        cellLabelPrefab.name = "HexCellLabel";
        noiseSource = Resources.Load("Texture/Noise") as Texture2D;

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
                if (HexMetrics.instance.isEditor)
                {
                    DestroyImmediate(chunks[i].gameObject);
                }
                else
                {
                    Destroy(chunks[i].gameObject);
                }
            }
        }

        HexMetrics.instance.noiseSource = noiseSource;
        if (chunkWidth == 0 || chunkHeight == 0)
        {
            width = HexMetrics.instance.chunkWidth;
            height = HexMetrics.instance.chunkHeight;
        }
        else
        {
            width = chunkWidth;
            height = chunkHeight;
        }

        oldCountX = chunkCountX;
        oldCountZ = chunkCountZ;
        cellCountWidth = width * chunkCountX;
        cellCountHeight = height * chunkCountZ;

        maps = new HexMap[1];
        maps[0] = Instantiate(mapPrefab) as HexMap;
        maps[0].name = mapName;
        maps[0].transform.localPosition = Vector3.zero;
        maps[0].transform.localRotation = Quaternion.Euler(0, 0, 0);
        maps[0].SetMapSize(chunkCountX, chunkCountZ);
        maps[0].NewMap(cellPrefab, cellLabelPrefab, gridChunkPerfab, isNewMaterial, terrainTexs, width, height);
        cells = maps[0].cells;
        chunks = maps[0].chunks;

        //cells = new HexCell[cellCountWidth * cellCountHeight];

        //for (int j = 0, i = 0; j < chunkCountZ; j++)
        //{
        //    for (int k = 0; k < chunkCountX; k++)
        //    {
        //        HexGridChunk chunk = chunks[i++] = Instantiate(gridChunkPerfab) as HexGridChunk;
        //        //chunk.transform.SetParent(GameObject.Find("Map").transform);
        //        chunk.name = "HexChunk_" + k.ToString("000") + "_" + j.ToString("000"); 
        //        chunk.transform.SetParent(transform);
        //        chunk.transform.localPosition = new Vector3(chunk.transform.localPosition.x, 0, chunk.transform.localPosition.z);
        //    }
        //}


        //for (int j = 0, i = 0; j < cellCountHeight; j++)
        //{
        //    for (int k = 0; k < cellCountWidth; k++)
        //    {
        //        CreateCell(k, j, i++);
        //    }
        //}

        //Refresh();
    }

    public void MapInit()
    {
        if(noiseSource == null)
        {
            noiseSource = Resources.Load("Texture/Noise") as Texture2D;
            HexMetrics.instance.noiseSource = noiseSource;
        }

        if (maps.Length > 0)
        {
            cells = maps[0].cells;
            chunks = maps[0].chunks;
            cellCountHeight = maps[0].cellCountHeight;
            cellCountWidth = maps[0].cellCountWidth;
            chunkCountX = maps[0].chunkCountX;
            chunkCountZ = maps[0].chunkCountZ;
        }
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

        for (int i = 0; i < chunks.Length; i++)
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
                if (itemClassList[j].cell == null) continue;
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
        if (cells == null) return null;
        if (index<0 || index >= cells.Length) return null;
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

    public void ChunkInit()
    {
        for (int j = 0, i = 0; j < chunkCountZ; j++)
        {
            for (int k = 0; k < chunkCountX; k++)
            {
                chunks[i++].Init();
            }

        }
    }

    public void ChunkMeshInit()
    {
        for (int j = 0, i = 0; j < chunkCountZ; j++)
        {
            for (int k = 0; k < chunkCountX; k++)
            {
                chunks[i++].MeshInit();
            }

        }
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
		cell.color = HexMetrics.instance.defaultColor;
       

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

        if (!HexMetrics.instance.isEditor)
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
        if(cellOfChunkZ * width + cellOfChunkX == 0)
        {
            tmpChunk.transform.position = cell.transform.position;
        }
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