using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexMapEditor : MonoBehaviour {

	public Color[] colors;

	public HexGrid hexGrid;

    public HexEdgeMesh hexEdgeMesh;

    int activeElevation;

	Color activeColor;

    public Toggle isStepEditorToggle;


    bool SetIsEditorStep()
    {
        if(isStepEditorToggle.isOn)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

	public void SelectColor (int index) {
		activeColor = colors[index];
	}

	public void SetElevation (float elevation) {
		activeElevation = (int)elevation;
	}

	void Awake () {
		SelectColor(0);

    }

	void Update () {
		if (
			Input.GetMouseButtonDown(0) &&
			!EventSystem.current.IsPointerOverGameObject()
		) {
			HandleInput();
		}
        if(SetIsEditorStep())
        {
            HandleInputEdge();
        }
        else
        {
            hexEdgeMesh.Clear();
        }
	}

    //编辑边
    void HandleInputEdge()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            EditEdge(hit.point, hexGrid.GetCell(hit.point));
        }
    }

    void EditEdge(Vector3 pos, HexCell cell)
    {
        if (hexGrid.IsClickInEdge(cell, pos))
        {
            HexDirection clickDir = hexGrid.GetPointDirection(new Vector2(pos.x - cell.transform.position.x, pos.z - cell.transform.position.z));
            if (cell.GetEdgeType(cell.isStepDirection[(int)clickDir], clickDir) == HexEdgeType.Slope)
            {
                hexEdgeMesh.Triangulate(cell, cell.GetNeighbor(clickDir), clickDir,true);
            }
            else if (cell.GetEdgeType(cell.isStepDirection[(int)clickDir], clickDir) == HexEdgeType.Step)
            {
                hexEdgeMesh.Triangulate(cell, cell.GetNeighbor(clickDir), clickDir,false);
            }
        }
    }

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			EditCell(hit.point,hexGrid.GetCell(hit.point));
		}
	}

	void EditCell (Vector3 pos,HexCell cell) {
        if (SetIsEditorStep())
        {
            HexDirection clickDir = hexGrid.GetPointDirection(new Vector2(pos.x - cell.transform.position.x, pos.z - cell.transform.position.z));
            if (hexGrid.IsClickInEdge(cell, pos))
            {
                if (cell.GetEdgeType(cell.isStepDirection[(int)clickDir], clickDir) == HexEdgeType.Slope)
                {
                    cell.isStepDirection[(int)clickDir] = true;
                    cell.GetNeighbor(clickDir).isStepDirection[(int)clickDir.Opposite()] = true;
                }
                else
                {
                    cell.isStepDirection[(int)clickDir] = false;
                    cell.GetNeighbor(clickDir).isStepDirection[(int)clickDir.Opposite()] = false;
                }
            }
        }
        else
        {
            cell.color = activeColor;
            cell.Elevation = activeElevation;
        }

        hexGrid.Refresh();
	}


}