using UnityEngine;

public class HexCell : MonoBehaviour {

	public HexCoordinates coordinates;

	public Color color;

	public RectTransform uiRect;

	public int Elevation {
		get {
			return elevation;
		}
		set {
			elevation = value;
			Vector3 position = transform.localPosition;
			position.y = value * HexMetrics.elevationStep;
            position.y +=(HexMetrics.SampleNoise(position).y * 2f - 1f) *HexMetrics.elevationPerturbStrength;//对高度进行微扰;
            transform.localPosition = position;

			Vector3 uiPosition = uiRect.localPosition;
			uiPosition.z = -position.y;
			uiRect.localPosition = uiPosition;
		}
	}

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }

    int elevation;

    [SerializeField]
    public bool[] isStepDirection;

    [SerializeField]
	HexCell[] neighbors;

	public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}

	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	public HexEdgeType GetEdgeType (bool isStep,HexDirection direction) {
        if (neighbors[(int)direction] == null)
            return 0;
		return HexMetrics.GetEdgeType(
			elevation, neighbors[(int)direction].elevation, isStep
        );
	}

    public HexEdgeType GetEdgeType (bool isStep, HexCell otherCell) {
		return HexMetrics.GetEdgeType(
			elevation, otherCell.elevation, isStep
        );
	}

}