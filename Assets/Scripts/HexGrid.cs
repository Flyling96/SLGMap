using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {

	public int width = 6;
	public int height = 6;

	public Color defaultColor = Color.white;

    //不规则噪音
    public Texture2D noiseSource;

    public HexCell cellPrefab;
	public Text cellLabelPrefab;

	HexCell[] cells;

	Canvas gridCanvas;
	HexMesh hexMesh;

	void Awake () {

        HexMetrics.noiseSource = noiseSource;
        gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++) {
			for (int x = 0; x < width; x++) {
				CreateCell(x, z, i++);
			}
		}
	}

    void OnEnable()
    {
        HexMetrics.noiseSource = noiseSource;
    }

    void Start () {
		hexMesh.Triangulate(cells);
	}

	public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
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
		hexMesh.Triangulate(cells);
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
				cell.SetNeighbor(HexDirection.SE, cells[i - width]);
				if (x > 0) {
					cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
				}
			}
			else {
				cell.SetNeighbor(HexDirection.SW, cells[i - width]);
				if (x < width - 1) {
					cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
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
	}
}