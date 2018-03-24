using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class HexGrid : MonoBehaviour {

	int width = 6;
	int height = 6;

    int oldCountX = 4;
    int oldCountZ = 4;

    public int chunkCountX = 4;
    public int chunkCountZ = 4;

    int cellCountHeight = 0;
    int cellCountWidth = 0;

	public Color defaultColor = Color.white;

    //不规则噪声
    public Texture2D noiseSource;

    public HexGridChunk gridChunkPerfab;
    public HexCell cellPrefab;
	public Text cellLabelPrefab;

	HexCell[] cells;


    HexGridChunk[] chunks;
    //Canvas gridCanvas;
	//HexMesh hexMesh;


    void Start() {
        ConfigDateManage.instance.InitData();
        oldCountX = chunkCountX;
        oldCountZ = chunkCountZ;
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];
        NewMap();

    }

    //改变地图尺寸
    public void ChangeSize(int width,int height)
    {
        chunkCountX = width;
        chunkCountZ = height;
    }

    //新建地图
    public void NewMap()
    {

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
        HexMetrics.noiseSource = noiseSource;
        //gridCanvas = GetComponentInChildren<Canvas>();

        cells = new HexCell[cellCountWidth * cellCountHeight];

        for (int j = 0, i = 0; j < chunkCountZ; j++)
        {
            for (int k = 0; k < chunkCountX; k++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(gridChunkPerfab) as HexGridChunk;
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
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Save(writer);
        }
    }

    public void Load(BinaryReader reader)
    {
        chunkCountX = reader.ReadByte();
        chunkCountZ = reader.ReadByte();
        NewMap();
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Load(reader);
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

		//Text label = Instantiate<Text>(cellLabelPrefab);
		//label.rectTransform.SetParent(gridCanvas.transform, false);
		//label.rectTransform.anchoredPosition =new Vector2(position.x, position.z);
		//label.text = cell.coordinates.ToStringOnSeparateLines();
		//cell.uiRect = label.rectTransform;


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
}