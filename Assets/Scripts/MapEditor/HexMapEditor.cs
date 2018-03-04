using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.IO;


public class HexMapEditor : MonoBehaviour {

	public Color[] colors;
    public static GameObject uiRoot;

	public HexGrid hexGrid;

    public HexEdgeMesh hexEdgeMesh;

    int elevation = 0;
    int waterLevel = 0;
    int brushRange = 0;

	Color cellColor;

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

    bool isEditorElevation = false;
    public void IsEditorElevation(bool value)
    {
        isEditorElevation = value;
    }

    bool isEditorWater = false;
    public void IsEditorWater(bool value)
    {
        isEditorWater = value;
    }

	public void SelectColor (int index)
    {
		cellColor = colors[index];
	}

	public void SetElevation (float sliderValue)
    {
		elevation = (int)sliderValue;
	}

    public void SetWaterLevel(float sliderValue)
    {
        waterLevel = (int)sliderValue;
    }


    public void SetBrushRange(float sliderValue)
    {
        brushRange = (int)sliderValue;
    }

	void OnEnable()
    {
        uiRoot = this.gameObject;
        SelectColor(0);
        brushRange = 1;
    }

	void Update () {


        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }

        RefreshHexEdgeMesh();
    }

    //刷新提示Mesh
    void RefreshHexEdgeMesh()
    {
        List<HexCell> cells = new List<HexCell>();
        HexCell centerCell = null;
        int centerX = 0;
        int centerZ = 0;
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            if (IsEditorStep() || IsEditorSlope())
            {
                EditMeshRefresh(hit.point, hexGrid.GetCell(hit.point));
            }
            else
            {
                centerCell = hexGrid.GetCell(hit.point);
                centerX = centerCell.coordinates.X;
                centerZ = centerCell.coordinates.Z;
                for (int l = 0, z = centerZ; z >= centerZ - brushRange + 1; l++, z--)
                {
                    for (int x = centerX - brushRange + 1 + l; x <= centerX + brushRange - 1; x++)
                    {
                        if (hexGrid.GetCell(new HexCoordinates(x, z) )!= null)
                        {
                            cells.Add(hexGrid.GetCell(new HexCoordinates(x, z)));
                        }
                    }
                }

                for (int l = 1, z = centerZ + 1; z <= centerZ + brushRange - 1; l++, z++)
                {
                    for (int x = centerX - brushRange + 1; x <= centerX + brushRange - 1 - l; x++)
                    {
                        cells.Add(hexGrid.GetCell(new HexCoordinates(x, z)));
                    }
                }
                hexEdgeMesh.Triangulate(cells, cellColor);
            }
        }
    }

    //刷新编辑指引的mesh
    void EditMeshRefresh(Vector3 pos, HexCell cell)
    {
        List<HexCell> cells = new List<HexCell>();
        HexCell cellChildren = null;
        int centerX = cell.coordinates.X;
        int centerZ = cell.coordinates.Z;
        for (int l = 0, z = centerZ; z >= centerZ - brushRange + 1; l++, z--)
        {
            for (int x = centerX - brushRange + 1 + l; x <= centerX + brushRange - 1; x++)
            {
                if (hexGrid.GetCell(new HexCoordinates(x, z)) != null)
                {
                    cells.Add(hexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }
        }

        for (int l = 1, z = centerZ + 1; z <= centerZ + brushRange - 1; l++, z++)
        {
            for (int x = centerX - brushRange + 1; x <= centerX + brushRange - 1 - l; x++)
            {
                cells.Add(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }

        HexDirection clickDir = hexGrid.GetPointDirection(new Vector2(pos.x - cell.transform.position.x, pos.z - cell.transform.position.z));
        if (IsEditorStep())
        {
            if (!IsWholeEditor())
            {
                hexEdgeMesh.Clear();
                for (int i = 0; i < cells.Count; i++)
                {
                    cellChildren = cells[i];
                    if(cellChildren==null)
                    {
                        continue;
                    }
                    if (cellChildren.GetEdgeType(cellChildren.isStepDirection[(int)clickDir], clickDir) == HexEdgeType.Slope)
                    {
                        hexEdgeMesh.TriangulateItem(cellChildren, cellChildren.GetNeighbor(clickDir), clickDir, true, new Color(0.18f, 1, 0.18f, 0.5f));
                    }
                    else
                    {
                        hexEdgeMesh.TriangulateItem(cellChildren, cellChildren.GetNeighbor(clickDir), clickDir, true, new Color(1, 0.18f, 0.18f, 0.5f));
                    }
                }
                hexEdgeMesh.InputMeshInfo();
            }
            else
            {
                hexEdgeMesh.Triangulate(cells, new Color(0.18f, 1, 0.18f, 0.5f));
            }

        }
        else if (IsEditorSlope())
        {
            if (!IsWholeEditor())
            {
                hexEdgeMesh.Clear();
                for (int i = 0; i < cells.Count; i++)
                {
                    cellChildren = cells[i];
                    if (cellChildren == null)
                    {
                        continue;
                    }
                    if (cellChildren.GetEdgeType(cellChildren.isStepDirection[(int)clickDir], clickDir) == HexEdgeType.Step)
                    {
                        hexEdgeMesh.TriangulateItem(cellChildren, cellChildren.GetNeighbor(clickDir), clickDir, false, new Color(0.18f, 1, 0.18f, 0.5f));
                    }
                    else
                    {
                        hexEdgeMesh.TriangulateItem(cellChildren, cellChildren.GetNeighbor(clickDir), clickDir, false, new Color(1, 0.18f, 0.18f, 0.5f));
                    }
                }
                hexEdgeMesh.InputMeshInfo();
            }
            else
            {
                hexEdgeMesh.Triangulate(cells, new Color(0.18f, 1, 0.18f, 0.5f));
            }
        }

    }

    List<HexGridChunk> refreshChunkList = new List<HexGridChunk>();
	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
        HexCell centerCell = null;
        int centerX = 0;
        int centerZ = 0;

        refreshChunkList.Clear();
        if (Physics.Raycast(inputRay, out hit)) {
            centerCell = hexGrid.GetCell(hit.point);
            refreshChunkList.Add(centerCell.chunkParent);
            centerX = centerCell.coordinates.X;
            centerZ = centerCell.coordinates.Z;
            Vector3 pos = hit.point;
            HexDirection clickDir = hexGrid.GetPointDirection(new Vector2(pos.x - centerCell.transform.position.x, pos.z - centerCell.transform.position.z));
            for (int l = 0, z = centerZ; z >= centerZ - brushRange + 1; l++, z--)
            {
                for (int x = centerX - brushRange + 1 + l; x <= centerX + brushRange - 1; x++)
                {
                    EditCell(clickDir, hexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }

            for(int l = 1,z = centerZ + 1; z<= centerZ + brushRange - 1;l++,z++)
            {
                for(int x = centerX - brushRange + 1; x<= centerX + brushRange - 1-l;x++)
                {
                    EditCell(clickDir, hexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }
           for(int i=0;i< refreshChunkList.Count;i++)
            {
                if(isEditorWater)
                {
                    refreshChunkList[i].Refresh(MeshClass.waterMesh);
                    refreshChunkList[i].Refresh(MeshClass.waterEdgeMesh);
                }
                else 
                {
                    refreshChunkList[i].Refresh();
                }
            }
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
    //对六边形的某条边进行编辑
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

	void EditCell (HexDirection clickDir, HexCell cell) {

        if (cell == null)
            return;

        if (IsEditorStep()|| IsEditorSlope())
        {
            if (!IsWholeEditor())
            {
                EditorEdge(cell, clickDir);
            }
            else
            {
                EditorWholeCell(cell);
            }
        }
        else
        {
            if (isEditorElevation)
            {
                cell.Elevation = elevation;
            }
            if(isEditorWater)
            {
                cell.WaterLevel = waterLevel;
            }
            else
            {
                cell.color = cellColor;
            }
        }
        List<HexGridChunk> neigborChunk = cell.NeighorChunk();
        if (neigborChunk.Count > 0)
        {
            for (int i = 0; i < neigborChunk.Count; i++)
            {
                if (!refreshChunkList.Contains(neigborChunk[i]))
                {
                    refreshChunkList.Add(neigborChunk[i]);
                }
        }
        }

    }


    public void ShowSave()
    {
        List<string> inputString = new List<string>();
        inputString.Add("文件名");
        UIManage.instance.ShowInputWnd(inputString,Save,null,"保存地图");
    }

    public void ShowLoad()
    {
        List<string> inputString = new List<string>();
        inputString.Add("文件名");
        UIManage.instance.ShowInputWnd(inputString, Load, null,"加载地图");
    }

    public void Save(Dictionary<string, InputField> inputDic)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(Application.persistentDataPath + "/" +inputDic["文件名"].text, FileMode.Create)))
        {
            hexGrid.Save(writer);
        }
        UIManage.instance.HideInputWnd();
        StartCoroutine(WaitSave(Application.persistentDataPath + "/" + inputDic["文件名"].text));
    }

    IEnumerator WaitSave(string path)
    {
        bool isBreak = false;
        int count = 0;
        while(!File.Exists(path))
        {
            yield return new WaitForSeconds(0.05f);
            if(count>9)
            {
                count++;
                isBreak = true;
                break;
            }
        }
        if(isBreak)
        {
            UIManage.instance.ShowTipLine("保存失败", 3f);
        }
        else
        {
            UIManage.instance.ShowTipLine("保存成功", 3f);
        }
    }

    public void Load(Dictionary<string, InputField> inputDic)
    {
        if(!File.Exists(Application.persistentDataPath + "/" + inputDic["文件名"].text))
        {
            UIManage.instance.ShowTipLine("读取文件不存在", 3f);
            return;
        }
        try
        {
            using (BinaryReader reader = new BinaryReader(File.Open(Application.persistentDataPath + "/" + inputDic["文件名"].text, FileMode.Open)))
            {
                hexGrid.Load(reader);
            }
            UIManage.instance.HideInputWnd();
            hexGrid.Refresh();
            UIManage.instance.ShowTipLine("读取成功", 3f);
        }
        catch
        {
            UIManage.instance.ShowTipLine("读取失败", 3f);
        }
    }


}