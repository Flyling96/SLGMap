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
    public Toggle isSlopeEditorToggle;
    public Toggle isStepWholeEditorToggle;


    bool IsEditorStep()
    {
        return isStepEditorToggle.isOn;
    }

    bool IsEditorSlope()
    {
        return isSlopeEditorToggle.isOn;
    }

    bool IsWholeEditor()
    {
        return isStepWholeEditorToggle.isOn;
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

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }

        if (IsEditorStep() || IsEditorSlope())
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
            EditMeshRefresh(hit.point, hexGrid.GetCell(hit.point));

        }
    }
    //刷新编辑指引的mesh
    void EditMeshRefresh(Vector3 pos, HexCell cell)
    {
        if (hexGrid.IsClickInEdge(cell, pos))
        {
            HexDirection clickDir = hexGrid.GetPointDirection(new Vector2(pos.x - cell.transform.position.x, pos.z - cell.transform.position.z));
            if (IsEditorStep())
            {
                if (!IsWholeEditor())
                {
                    if (cell.GetEdgeType(cell.isStepDirection[(int)clickDir], clickDir) == HexEdgeType.Slope)
                    {
                        hexEdgeMesh.Triangulate(cell, cell.GetNeighbor(clickDir), clickDir, true, new Color(0.18f, 1, 0.18f, 0.5f));
                    }
                    else
                    {
                        hexEdgeMesh.Triangulate(cell, cell.GetNeighbor(clickDir), clickDir, true, new Color(1, 0.18f, 0.18f, 0.5f));
                    }
                }
                else
                {
                    hexEdgeMesh.Triangulate(cell, new Color(0.18f, 1, 0.18f, 0.5f));
                }
            }
            else if (IsEditorSlope())
            {
                if (!IsWholeEditor())
                {
                    if (cell.GetEdgeType(cell.isStepDirection[(int)clickDir], clickDir) == HexEdgeType.Step)
                    {
                        hexEdgeMesh.Triangulate(cell, cell.GetNeighbor(clickDir), clickDir, false, new Color(0.18f,1,0.18f,0.5f));
                    }
                    else
                    {
                        hexEdgeMesh.Triangulate(cell, cell.GetNeighbor(clickDir), clickDir, false, new Color(1, 0.18f, 0.18f, 0.5f));
                    }
                }
                else
                {
                    hexEdgeMesh.Triangulate(cell, new Color(0.18f, 1, 0.18f, 0.5f));
                }
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

    //对整个六边形的边进行编辑
    void EditorWholeCell(HexCell cell)
    {
        if (IsEditorStep())
        {
            for (int i = 0; i < 6; i++)
            {
                {
                    cell.isStepDirection[i] = true;
                    if (cell.GetNeighbor((HexDirection)i) != null)
                        cell.GetNeighbor((HexDirection)i).isStepDirection[(int)((HexDirection)i).Opposite()] = true;
                }
            }
        }
        else if(IsEditorSlope())
        {
            for (int i = 0; i < 6; i++)
            {
                {
                    cell.isStepDirection[i] = false;
                    if (cell.GetNeighbor((HexDirection)i) != null)
                        cell.GetNeighbor((HexDirection)i).isStepDirection[(int)((HexDirection)i).Opposite()] = false;
                }
            }
        }
    }
    //对六边形的某个边进行编辑
    void EditorEdge(HexCell cell,HexDirection clickDir)
    {
        if (IsEditorStep())
        {
            cell.isStepDirection[(int)clickDir] = true;
            if (cell.GetNeighbor(clickDir) != null)
                cell.GetNeighbor(clickDir).isStepDirection[(int)clickDir.Opposite()] = true;
        }
        else if (IsEditorSlope())
        {
            cell.isStepDirection[(int)clickDir] = false;
            if (cell.GetNeighbor(clickDir) != null)
                cell.GetNeighbor(clickDir).isStepDirection[(int)clickDir.Opposite()] = false;
        }
    }

	void EditCell (Vector3 pos,HexCell cell) {

        if (IsEditorStep()|| IsEditorSlope())
        {
            if (!IsWholeEditor())
            {
                HexDirection clickDir = hexGrid.GetPointDirection(new Vector2(pos.x - cell.transform.position.x, pos.z - cell.transform.position.z));
                if (hexGrid.IsClickInEdge(cell, pos))
                {
                    EditorEdge(cell, clickDir);
                }
            }
            else
            {
                EditorWholeCell(cell);
            }
        }
        else
        {
            cell.color = activeColor;
            cell.Elevation = activeElevation;
        }
        cell.Refresh();

    }


}