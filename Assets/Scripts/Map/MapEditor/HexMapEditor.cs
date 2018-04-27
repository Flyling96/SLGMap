using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using System.Text.RegularExpressions;


public class HexMapEditor : MonoBehaviour {

	//public Color[] colors;
    public static GameObject uiRoot;

	public HexGrid hexGrid;

    public HexEdgeMesh hexEdgeMesh;

    int elevation = 0;
    int waterLevel = 0;
    int brushRange = 1;
    int sceneObjectDensity = 1;

    //Color cellColor;

    public Toggle isDeleteSceneObject;
    public Toggle isBrushSceneObject;

    public Toggle isStepEditorToggle;
    public Toggle isSlopeEditorToggle;
    public Toggle isStepWholeEditorToggle;


    Button terrainTextureBtn;
    Button sceneObjectBtn;


    bool IsDeleteSceneObject
    {
        get
        {
            return isDeleteSceneObject.isOn;
        }
    }

    bool IsBrushSceneObject
    {
        get
        {
            return isBrushSceneObject.isOn;
        }
    }



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

	//public void SelectColor (int index)
 //   {
	//	cellColor = colors[index];
	//}

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

    public void SetSceneObjectBrush(float sliderValue)
    {
        sceneObjectDensity = (int)sliderValue;
    }

    private void Awake()
    {
        terrainTextureBtn = transform.Find("Terrain Panel/TerrainTextureBtn").GetComponent<Button>();
        terrainTextureBtn.onClick.AddListener(ShowTerrainTextureWnd);
        sceneObjectBtn = transform.Find("Terrain Panel/SceneObjectBtn").GetComponent<Button>();
        sceneObjectBtn.onClick.AddListener(ShowSceneObjectWnd);
    }

    void OnEnable()
    {
        uiRoot = this.gameObject;
        //SelectColor(0);
        brushRange = 1;
    }

	void Update () {

        if (HexMetrics.instance.IsEditorTerrain)
        {
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                HandleInput();
            }
        }
        else if (HexMetrics.instance.IsEditorSceneObject)
        {
            if(IsBrushSceneObject&&!hexEdgeMesh.gameObject.activeSelf)
            {
                hexEdgeMesh.gameObject.SetActive(true);
            }
            if(!IsBrushSceneObject&&hexEdgeMesh.gameObject.activeSelf)
            {
                hexEdgeMesh.gameObject.SetActive(false);
            }
           if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                HandleInputDown();
            }
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
            if(HexMetrics.instance.IsEditorTerrain||(HexMetrics.instance.IsEditorSceneObject &&IsBrushSceneObject))
            {
                if (IsEditorStep() || IsEditorSlope())
                {
                    EditMeshRefresh(hit.point, hexGrid.GetCell(hit.point));
                }
                else
                {
                    //if (!HexMetrics.instance.isEditorTerrain)
                    //    return;
                    centerCell = hexGrid.GetCell(hit.point);
                    centerX = centerCell.coordinates.X;
                    centerZ = centerCell.coordinates.Z;
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
                    if (!isEditorWater)
                    {
                        if (HexMetrics.instance.isEditorTexture)
                        {
                            if (HexMetrics.instance.IsEditorSceneObject && IsDeleteSceneObject && IsBrushSceneObject)
                            {
                                hexEdgeMesh.Triangulate(cells, ToolClass.instance.ConvertColor("#FF2020FF"));
                            }
                            else if(HexMetrics.instance.IsEditorSceneObject && !IsDeleteSceneObject && IsBrushSceneObject)
                            {
                                hexEdgeMesh.Clear();
                                 Vector2 max = new Vector2(hit.point.x + 8 * brushRange, hit.point.z + 8 * brushRange);
                                Vector2 min = new Vector2(hit.point.x - 8 * brushRange, hit.point.z - 8 * brushRange);
                                hexEdgeMesh.AddQuad
                                    (
                                        new Vector3(min.x, hit.point.y + 2, min.y),
                                        new Vector3(max.x, hit.point.y + 2, min.y),
                                        new Vector3(min.x, hit.point.y + 2, max.y),
                                        new Vector3(max.x, hit.point.y + 2, max.y)
                                    );
                                hexEdgeMesh.AddQuadColor
                                    (
                                        new Color(0.18f, 1, 0.18f, 1f),
                                        new Color(0.18f, 1, 0.18f, 1f)
                                    );
                                hexEdgeMesh.InputMeshInfo();
                            }
                            else
                            {
                                hexEdgeMesh.Triangulate(cells, new Color(0.18f, 1, 0.18f, 1f));
                            }
                        }
                        else
                        {
                            if (HexMetrics.instance.IsEditorSceneObject && IsDeleteSceneObject)
                            {
                                hexEdgeMesh.Triangulate(cells, ToolClass.instance.ConvertColor("#FF2020FF"));
                            }
                            else
                            {
                                hexEdgeMesh.Triangulate(cells, HexMetrics.instance.editorColor);
                            }
                        }
                    }
                    else
                    {
                        if (!HexMetrics.instance.IsEditorSceneObject)
                        {
                            hexEdgeMesh.Triangulate(cells, ToolClass.instance.ConvertColor("#2FD4FFFF"));
                        }
                    }
                }
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

    List<HexGridChunk> refreshChunkList = new List<HexGridChunk>();//mesh基于chunk刷新
    List<HexCell> refreshCellList = new List<HexCell>();//sceneObject基于cell刷新

    //场景物体
    void HandleInputDown()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        HexCell centerCell = null;
        if (Physics.Raycast(inputRay, out hit))
        {
            centerCell = hexGrid.GetCell(hit.point);
            if (!IsDeleteSceneObject)
            {
                if (IsBrushSceneObject)//大量随机
                {
                    Vector2 max = new Vector2(hit.point.x + 8 * brushRange, hit.point.z + 8 * brushRange);
                    Vector2 min = new Vector2(hit.point.x - 8 * brushRange, hit.point.z - 8 * brushRange);
                    float px, pz,py=0;
                    for(int i=0;i<sceneObjectDensity * brushRange;i++)
                    {
                        px = UnityEngine.Random.Range(min.x, max.x);
                        pz = UnityEngine.Random.Range(min.y, max.y);
                        if(Physics.Raycast(new Vector3(px, 35, pz),Vector3.down,out hit))
                        {
                            if (hit.collider.gameObject.tag != "Mesh")
                                continue;
                            py = hit.point.y;
                        }
                        Vector3 tPosition = new Vector3(px, py, pz);
                        centerCell = hexGrid.GetCell(tPosition);
                        if(centerCell.isUnderWaterLevel|| centerCell == null)
                        {
                            continue;
                        }
                        GameObject tSceneObject = GameObjectPool.instance.GetPoolChild(HexMetrics.instance.editorSceneObject.name, HexMetrics.instance.editorSceneObject);
                        tSceneObject.transform.SetParent(centerCell.chunkParent.sceneObjectMgr.transform);
                        tSceneObject.transform.position = tPosition;
                        tSceneObject.transform.rotation = Quaternion.Euler(0f, 360f * UnityEngine.Random.value, 0f);
                        tSceneObject.SetActive(true);
                        HexDirection clickDir = hexGrid.GetPointDirection(new Vector2(tPosition.x - centerCell.transform.position.x, tPosition.z - centerCell.transform.position.z));
                        tSceneObject.GetComponent<SceneObjectClass>().SetInfo(tSceneObject.transform.localPosition, tSceneObject.transform.localRotation, clickDir, centerCell);
                        tSceneObject.GetComponent<SceneObjectClass>().sceneObjectInfo = HexMetrics.instance.editorSceneObjectInfo;
                        tSceneObject.GetComponent<SceneObjectClass>().Refresh(true);
                        tSceneObject.AddComponent<BoxCollider>();
                        tSceneObject.GetComponent<BoxCollider>().size = new Vector3(150, 150, 150);
                        tSceneObject.GetComponent<BoxCollider>().center = new Vector3(0, 150, 0);
                        tSceneObject.tag = "SceneObject";
                        centerCell.chunkParent.sceneObjectMgr.AddSceneObject(tSceneObject.GetComponent<SceneObjectClass>());
                    }
                }
                else
                {
                    if (hit.collider.gameObject.tag == "Mesh")
                    {
                        if (!centerCell.isUnderWaterLevel && centerCell != null)
                        {
                            GameObject tSceneObject = GameObjectPool.instance.GetPoolChild(HexMetrics.instance.editorSceneObject.name, HexMetrics.instance.editorSceneObject);
                            tSceneObject.transform.SetParent(centerCell.chunkParent.sceneObjectMgr.transform);
                            tSceneObject.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                            tSceneObject.transform.rotation = Quaternion.Euler(0f, 360f * UnityEngine.Random.value, 0f);
                            tSceneObject.SetActive(true);
                            HexDirection clickDir = hexGrid.GetPointDirection(new Vector2(hit.point.x - centerCell.transform.position.x, hit.point.z - centerCell.transform.position.z));
                            tSceneObject.GetComponent<SceneObjectClass>().SetInfo(tSceneObject.transform.localPosition, tSceneObject.transform.localRotation, clickDir, centerCell);
                            tSceneObject.GetComponent<SceneObjectClass>().sceneObjectInfo = HexMetrics.instance.editorSceneObjectInfo;
                            tSceneObject.GetComponent<SceneObjectClass>().Refresh(true);
                            tSceneObject.AddComponent<BoxCollider>();
                            tSceneObject.GetComponent<BoxCollider>().size = new Vector3(150, 150, 150);
                            tSceneObject.GetComponent<BoxCollider>().center = new Vector3(0, 150, 0);
                            tSceneObject.tag = "SceneObject";
                            centerCell.chunkParent.sceneObjectMgr.AddSceneObject(tSceneObject.GetComponent<SceneObjectClass>());
                        }
                    }
                }
            }
            else
            {
                int centerX = centerCell.coordinates.X;
                int centerZ = centerCell.coordinates.Z;
                if (IsBrushSceneObject&&IsDeleteSceneObject)
                {
                    for (int l = 0, z = centerZ; z >= centerZ - brushRange + 1; l++, z--)
                    {
                        for (int x = centerX - brushRange + 1 + l; x <= centerX + brushRange - 1; x++)
                        {
                            HexCell cell =  hexGrid.GetCell(new HexCoordinates(x, z));
                            cell.chunkParent.sceneObjectMgr.MinusSceneObject(cell);
                        }
                    }
                    for (int l = 1, z = centerZ + 1; z <= centerZ + brushRange - 1; l++, z++)
                    {
                        for (int x = centerX - brushRange + 1; x <= centerX + brushRange - 1 - l; x++)
                        {
                            HexCell cell = hexGrid.GetCell(new HexCoordinates(x, z));
                            cell.chunkParent.sceneObjectMgr.MinusSceneObject(cell);
                        }
                    }

                }
                else
                {
                    if (hit.collider.gameObject.tag == "SceneObject")
                    {
                        hit.collider.gameObject.GetComponent<SceneObjectClass>().cell.chunkParent.sceneObjectMgr.MinusSceneObject(hit.collider.gameObject.GetComponent<SceneObjectClass>());
                    }
                }
            }
        }
    }

    //地形
	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
        HexCell centerCell = null;
        int centerX = 0;
        int centerZ = 0;

        refreshChunkList.Clear();
        refreshCellList.Clear();
        if (Physics.Raycast(inputRay, out hit)) {
            centerCell = hexGrid.GetCell(hit.point);
            refreshChunkList.Add(centerCell.chunkParent);
            refreshCellList.Add(centerCell);
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

            for (int l = 1, z = centerZ + 1; z <= centerZ + brushRange - 1; l++, z++)
            {
                for (int x = centerX - brushRange + 1; x <= centerX + brushRange - 1 - l; x++)
                {
                    EditCell(clickDir, hexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }
            for (int i = 0; i < refreshChunkList.Count; i++)
            {
                if (isEditorWater)
                {
                    refreshChunkList[i].Refresh(MeshClass.waterMesh);
                    refreshChunkList[i].Refresh(MeshClass.waterEdgeMesh);
                }
                else
                {
                    refreshChunkList[i].Refresh();
                    // refreshChunkList[i].sceneObjectMgr.Refresh(refreshCellList);
                    StartCoroutine(WaitMesh(refreshChunkList[i]));
                }
            }

        }
	}

    IEnumerator WaitMesh(HexGridChunk chunk)
    {
        yield return new WaitForSeconds(0.3f);
        chunk.sceneObjectMgr.Refresh(refreshCellList);
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
                cell.color = HexMetrics.instance.editorColor;
                cell.TerrainTypeIndex = HexMetrics.instance.editorTerrainType;
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

    List<string> inputString = new List<string>();
    List<InputType> inputTypeList = new List<InputType>();

    #region 保存、加载、新建地图
    public void ShowSave()
    {
        inputString.Clear();
        inputTypeList.Clear();
        inputString.Add("文件名");
        inputTypeList.Add(InputType.InputField);
        UIManage.instance.ShowInputWnd(inputString,Save,null,"保存地图", inputTypeList);
    }

    public void ShowLoad()
    {
        inputString.Clear();
        inputTypeList.Clear();
        inputString.Add("文件名");
        inputTypeList.Add(InputType.InputField);
        UIManage.instance.ShowInputWnd(inputString, Load, null,"加载地图", inputTypeList);
    }

    public void ShowNewMapWnd()
    {
        inputString.Clear();
        inputTypeList.Clear();
        inputString.Add("宽度");
        inputString.Add("长度");
        inputTypeList.Add(InputType.InputField);
        inputTypeList.Add(InputType.InputField);
        inputString.Add("纹理");
        inputTypeList.Add(InputType.Toggle);
        UIManage.instance.ShowInputWnd(inputString, NewMap, null, "新建地图", inputTypeList);
    }

    public void Save(Dictionary<string, string> inputDic,ref bool isSuccessful)
    {
        try
        {
            //StartCoroutine(WaitSave(Application.persistentDataPath + "/" + inputDic["文件名"]));
            WaitSave(Application.persistentDataPath + "/" + inputDic["文件名"]);
        }
        catch(Exception e)
        {
            Debug.Log(e);
            UIManage.instance.ShowTipLine("保存失败", 3f);
        }
    }

    void WaitSave(string path)
    {
        //bool isBreak = false;
        //new System.Threading.Thread(() =>
        //{
        //    System.Threading.Thread.Sleep(100);
        //    using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        //    {
        //        hexGrid.Save(writer);
        //    }
        //    isBreak = true;
        //}).Start();

        //while(!isBreak)
        //{
        //    yield return null;
        //}

         using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
              hexGrid.Save(writer);
         }

        if (File.Exists(path))
        {
            UIManage.instance.ShowTipLine("保存成功", 3f);
            UIManage.instance.HideInputWnd(UIManage.instance.inputWnd.inputItem.name);
        }
        else
        {
            UIManage.instance.ShowTipLine("保存失败", 3f);
        }
    }

    public void Load(Dictionary<string, string> inputDic,ref bool isSuccessful)
    {
        if(!File.Exists(Application.persistentDataPath + "/" + inputDic["文件名"]))
        {
            UIManage.instance.ShowTipLine("读取文件不存在", 3f);
            return;
        }
        try
        {
            using (BinaryReader reader = new BinaryReader(File.Open(Application.persistentDataPath + "/" + inputDic["文件名"], FileMode.Open)))
            {
                hexGrid.Load(reader);
            }

            hexGrid.Refresh();
            UIManage.instance.ShowTipLine("读取成功", 3f);
            isSuccessful = true;
        }
        catch(Exception e)
        {
            Debug.Log(e);
            UIManage.instance.ShowTipLine("读取失败", 3f);
        }
    }

    public void NewMap(Dictionary<string, string> inputDic, ref bool isSuccessful)
    {
        if (Regex.IsMatch(inputDic["宽度"], "^[0-9]*$")==false||
            Regex.IsMatch(inputDic["长度"], "^[0-9]*$") == false)
        {
            UIManage.instance.ShowTipLine("请输入[1,50]范围内的整数", 3f);
            isSuccessful = false;
        }
        else
        {
            int width = int.Parse(inputDic["宽度"]);
            int height = int.Parse(inputDic["长度"]);
            if(width>50||height>50||width<1||height<1)
            {
                UIManage.instance.ShowTipLine("请输入[1,50]范围内的整数", 3f);
                isSuccessful = false;
            }
            else
            {
                hexGrid.ChangeSize(width, height);
                HexMetrics.instance.isEditorTexture = bool.Parse(inputDic["纹理"]);
                hexGrid.NewMap();
                isSuccessful = true;
                UIManage.instance.ShowTipLine("新建地图成功", 3f);
            }
        }
    }
    #endregion

    #region 地形纹理编辑相关
    void ShowTerrainTextureWnd()
    {
        hexEdgeMesh.gameObject.SetActive(true);
        HexMetrics.instance.IsEditorTerrain = true;
        if (HexMetrics.instance.isEditorTexture)
        {
            UIManage.instance.ShowDownSelectWnd(FileManage.instance.CSVTable["terrainTexture"], DownSelectWndType.terrainTexture);
        }
        else
        {
            UIManage.instance.ShowDownSelectWnd(FileManage.instance.CSVTable["terrainColor"], DownSelectWndType.terrainColor);
        }
    }

    void ShowSceneObjectWnd()
    {
        HexMetrics.instance.IsEditorSceneObject = true;
        UIManage.instance.ShowDownSelectWnd(FileManage.instance.CSVTable["sceneObject"], DownSelectWndType.sceneObject);
        hexEdgeMesh.gameObject.SetActive(false);
    }

    #endregion

    ////获取SceneObject的y
    //public float GetSceneObjectY(Vector3 point, HexCell cell)
    //{
    //    //获取方向
    //    HexDirection clickDir = hexGrid.GetPointDirection(new Vector2(point.x - cell.transform.position.x, point.z - cell.transform.position.z));
    //    //不在边缘
    //    if()


    //    //斜面
    //    if(cell.isStepDirection[(int)clickDir])
    //    {

    //    }
    //    else
    //    {

    //    }
    //}

}