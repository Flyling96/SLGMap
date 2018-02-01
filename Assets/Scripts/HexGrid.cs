using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {

	int width = 6;
	int height = 6;

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
    Canvas gridCanvas;
	HexMesh hexMesh;

	void Awake () {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];
        width = HexMetrics.chunkWidth;
        height = HexMetrics.chunkHeight;
        cellCountWidth = width * chunkCountX;
        cellCountHeight = height * chunkCountZ;
        HexMetrics.noiseSource = noiseSource;
        gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

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

	}

    void OnEnable()
    {
        HexMetrics.noiseSource = noiseSource;
    }

    void Start () {
        //hexMesh.Triangulate(cells);
        Refresh();

    }

	public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * cellCountWidth + coordinates.Z / 2;
		return cells[index];
	}

    public bool IsClickInEdge(HexCell cell,Vector3 position)
    {
        if(Vector2.Distance(new Vector2(cell.transform.position.x,cell.transform.position.z),new Vector2(position.x,position.z))>HexMetrics.innerRadius * HexMetrics.solidFactor)
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
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);


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

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparateLines();
		cell.uiRect = label.rectTransform;
        cell.isStepDirection = new bool[]{false,false,false,false,false,false};
        cell.Elevation = 0;
        CellToChunk(x, z, cell);
       

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