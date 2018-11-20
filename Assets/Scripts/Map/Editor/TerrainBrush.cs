using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#region 笔刷显示相关
//地形笔刷
public class TerrainBrush
{
    public List<HexCell> m_refreshCellList;
    public EditorType m_editorType = EditorType.HeightEditor;
    public HexEdgeMesh m_hexEdgeMesh = null;
    public int brushRange = 1;
    public UndoRedoOperation.UndoRedoInfo undoRedoInfo;

    public TerrainBrush(EditorType type, HexEdgeMesh hexEdgeMesh)
    {
        m_editorType = type;
        m_hexEdgeMesh = hexEdgeMesh;
        m_refreshCellList = new List<HexCell>();
    }

    public TerrainBrush()
    {
        m_refreshCellList = new List<HexCell>();
    }

    public void RefreshBrush()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            RefreshBrush(hit.point, HexGrid.instance.GetCell(hit.point));
        }
    }

    public virtual void RefreshBrush(Vector3 pos, HexCell cell)
    {
        RefreshCellList(cell);
    }

    public virtual void RefreshTarget(HexDirection clickDir, HexCell cell)
    {

    }

    public virtual void RefreshTarget(HexCell cell)
    {

    }

    public void RefreshCellList(HexCell cell)
    {
        m_refreshCellList.Clear();
        int centerX = cell.coordinates.X;
        int centerZ = cell.coordinates.Z;
        for (int l = 0, z = centerZ; z >= centerZ - brushRange + 1; l++, z--)
        {
            for (int x = centerX - brushRange + 1 + l; x <= centerX + brushRange - 1; x++)
            {
                if (HexGrid.instance.GetCell(new HexCoordinates(x, z)) != null)
                {
                    m_refreshCellList.Add(HexGrid.instance.GetCell(new HexCoordinates(x, z)));
                }
            }
        }

        for (int l = 1, z = centerZ + 1; z <= centerZ + brushRange - 1; l++, z++)
        {
            for (int x = centerX - brushRange + 1; x <= centerX + brushRange - 1 - l; x++)
            {
                m_refreshCellList.Add(HexGrid.instance.GetCell(new HexCoordinates(x, z)));
            }
        }
    }


}

//高度笔刷
public class HeightBrush:TerrainBrush
{
    int m_elevation = 1;

    public int Elevation
    {
        get
        {
            return m_elevation;
        }
        set
        {
            m_elevation = value;
        }
    }


    public HeightBrush(HexEdgeMesh hexEdgeMesh) : base(EditorType.HeightEditor, hexEdgeMesh) { }

    public override void RefreshBrush(Vector3 pos, HexCell cell)
    {
        base.RefreshBrush(pos, cell);

        if (HexMetrics.instance.isEditorTexture)
        {
            m_hexEdgeMesh.Triangulate(m_refreshCellList, new Color(0.18f, 1, 0.18f, 1f));
        }
        else
        {
            m_hexEdgeMesh.Triangulate(m_refreshCellList, HexMetrics.instance.editorColor);
        }

    }

    public override void RefreshTarget(HexDirection clickDir, HexCell cell)
    {
        base.RefreshTarget(clickDir, cell);
        cell.Elevation = m_elevation;

    }

}

//水面笔刷
public class WaterBrush : TerrainBrush
{

    int m_waterLevel = 1;

    public int WaterLevel
    {
        get
        {
            return m_waterLevel;
        }
        set
        {
            m_waterLevel = value;
        }
    }

    public WaterBrush(HexEdgeMesh hexEdgeMesh) : base(EditorType.WaterEditor, hexEdgeMesh) { }

    public override void RefreshBrush(Vector3 pos, HexCell cell)
    {
        base.RefreshBrush(pos, cell);

        m_hexEdgeMesh.Triangulate(m_refreshCellList, ToolClass.instance.ConvertColor("#2FD4FFFF"));

    }

    public override void RefreshTarget(HexDirection clickDir, HexCell cell)
    {
        base.RefreshTarget(clickDir, cell);
        cell.WaterLevel = m_waterLevel;
    }

}

//边界笔刷
public class EdgeBrush : TerrainBrush
{
    public enum EditorEdgeType
    {
        Slope,
        Step,
    }

    EditorEdgeType m_editorEdgeType = EditorEdgeType.Slope;
    bool m_isWholeEditor = false;

    public EditorEdgeType EditEdgeType
    {
        get
        {
            return m_editorEdgeType;
        }
        set
        {
            m_editorEdgeType = value;
        }
    }

    public bool IsWholeEditor
    {
        get
        {
            return m_isWholeEditor;
        }
        set
        {
            m_isWholeEditor = value;
        }
    }

    public EdgeBrush(HexEdgeMesh hexEdgeMesh, EditorEdgeType edgeType = EditorEdgeType.Slope, bool isWholeEditor = false) : base(EditorType.EdgeEditor, hexEdgeMesh)
    {
        EditEdgeType = edgeType;
        IsWholeEditor = isWholeEditor;
    }

    public EdgeBrush(EditorType type, HexEdgeMesh hexEdgeMesh) : base(type, hexEdgeMesh) { }

    //对整个六边形的边进行编辑
    void EditorWholeCell(HexCell cell)
    {
        if (m_editorEdgeType == EditorEdgeType.Step)
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
        else if (m_editorEdgeType == EditorEdgeType.Slope
)
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
    void EditorEdge(HexCell cell, HexDirection clickDir)
    {
        if (m_editorEdgeType == EditorEdgeType.Step)
        {
            cell.isStepDirection[(int)clickDir] = true;
            if (cell.GetNeighbor(clickDir) != null)
                cell.GetNeighbor(clickDir).isStepDirection[(int)clickDir.Opposite()] = true;
        }
        else if (m_editorEdgeType == EditorEdgeType.Slope)
        {
            cell.isStepDirection[(int)clickDir] = false;
            if (cell.GetNeighbor(clickDir) != null)
                cell.GetNeighbor(clickDir).isStepDirection[(int)clickDir.Opposite()] = false;
        }
    }

    public override void RefreshBrush(Vector3 pos, HexCell cell)
    {
        base.RefreshBrush(pos, cell);

        HexCell cellChildren = null;
        RefreshCellList(cell);
        HexDirection clickDir = HexGrid.instance.GetPointDirection(new Vector2(pos.x - cell.transform.position.x, pos.z - cell.transform.position.z));

        if (m_editorEdgeType == EditorEdgeType.Step)
        {
            if (!m_isWholeEditor)
            {
                m_hexEdgeMesh.Clear();
                for (int i = 0; i < m_refreshCellList.Count; i++)
                {
                    cellChildren = m_refreshCellList[i];
                    if (cellChildren == null)
                    {
                        continue;
                    }
                    if (cellChildren.GetEdgeType(cellChildren.isStepDirection[(int)clickDir], clickDir) == HexEdgeType.Slope)
                    {
                        m_hexEdgeMesh.TriangulateItem(cellChildren, cellChildren.GetNeighbor(clickDir), clickDir, true, new Color(0.18f, 1, 0.18f, 0.5f));
                    }
                    else
                    {
                        m_hexEdgeMesh.TriangulateItem(cellChildren, cellChildren.GetNeighbor(clickDir), clickDir, true, new Color(1, 0.18f, 0.18f, 0.5f));
                    }
                }
                m_hexEdgeMesh.InputMeshInfo();
            }
            else
            {
                m_hexEdgeMesh.Triangulate(m_refreshCellList, new Color(0.18f, 1, 0.18f, 0.5f));
            }

        }
        else if (m_editorEdgeType == EditorEdgeType.Slope)
        {
            if (!m_isWholeEditor)
            {
                m_hexEdgeMesh.Clear();
                for (int i = 0; i < m_refreshCellList.Count; i++)
                {
                    cellChildren = m_refreshCellList[i];
                    if (cellChildren == null)
                    {
                        continue;
                    }
                    if (cellChildren.GetEdgeType(cellChildren.isStepDirection[(int)clickDir], clickDir) == HexEdgeType.Step)
                    {
                        m_hexEdgeMesh.TriangulateItem(cellChildren, cellChildren.GetNeighbor(clickDir), clickDir, false, new Color(0.18f, 1, 0.18f, 0.5f));
                    }
                    else
                    {
                        m_hexEdgeMesh.TriangulateItem(cellChildren, cellChildren.GetNeighbor(clickDir), clickDir, false, new Color(1, 0.18f, 0.18f, 0.5f));
                    }
                }
                m_hexEdgeMesh.InputMeshInfo();
            }
            else
            {
                m_hexEdgeMesh.Triangulate(m_refreshCellList, new Color(0.18f, 1, 0.18f, 0.5f));
            }
        }
    }

    public override void RefreshTarget(HexDirection clickDir, HexCell cell)
    {
        base.RefreshTarget(clickDir, cell);
        if (!m_isWholeEditor)
        {
            EditorEdge(cell, clickDir);
        }
        else
        {
            EditorWholeCell(cell);
        }
    }

}


//材质笔刷
public class MaterialBrush:TerrainBrush
{
    public Color m_editColor;

    public TerrainTypes m_terrainTextureIndex = TerrainTypes.Grass;

    public Color EditColor
    {
        get
        {
            return m_editColor;
        }
        set
        {
            m_editColor = value;
        }
    }

    public TerrainTypes TerrainType
    {
        get
        {
            return m_terrainTextureIndex;
        }
        set
        {
            m_terrainTextureIndex = value;
        }
    }

    public MaterialBrush(HexEdgeMesh hexEdgeMesh) : base(EditorType.MaterialEditor, hexEdgeMesh) { }

    public override void RefreshBrush(Vector3 pos, HexCell cell)
    {
        base.RefreshBrush(pos, cell);

        if (HexMetrics.instance.isEditorTexture)
        {
            m_hexEdgeMesh.Triangulate(m_refreshCellList, new Color(0.18f, 1, 0.18f, 1f));
        }
        else
        {
            m_hexEdgeMesh.Triangulate(m_refreshCellList, HexMetrics.instance.editorColor);
        }
    }

    public override void RefreshTarget(HexCell cell)
    {
        base.RefreshTarget(cell);

        if(HexMetrics.instance.isEditorTexture)
        {
            cell.TerrainTypeIndex = TerrainType;
        }
        else
        {
            cell.color = EditColor;
        }

    }
}

//场景物体笔刷
public class SceneObjBrush:TerrainBrush
{
    public enum OperationType
    {
        Add,
        Delete,
        Refresh,
    }

    bool m_isBrushSceneObject = false;

    int m_sceneObjectDensity = 1;

    OperationType m_operationType = OperationType.Add;

    public bool IsBrushSceneObject
    {
        get
        {
            return m_isBrushSceneObject;
        }
        set
        {
            m_isBrushSceneObject = value;
        }
    }

    public int SceneObjectDensity
    {
        get
        {
            return m_sceneObjectDensity;
        }
        set
        {
            m_sceneObjectDensity = value;
        }
    }

    public OperationType SceneObjOperationType
    {
        get
        {
            return m_operationType;
        }
        set
        {
            m_operationType = value;
        }
    }

    public GameObject SceneObj;

    float m_size = 1.0f;

    public float ObjSize
    {
        get
        {
            return m_size; 
        }
        set
        {
            m_size = value;
        }
    }

    public SceneObjBrush(HexEdgeMesh hexEdgeMesh) : base(EditorType.SceneObjEditor, hexEdgeMesh) { }

    public override void RefreshBrush(Vector3 pos, HexCell cell)
    {
        base.RefreshBrush(pos, cell);

        if (m_operationType == OperationType.Delete && IsBrushSceneObject)
        {
            m_hexEdgeMesh.Triangulate(m_refreshCellList, ToolClass.instance.ConvertColor("#FF2020FF"));
        }
        else if (m_operationType != OperationType.Delete && IsBrushSceneObject)
        {
            m_hexEdgeMesh.Clear();
            Vector2 max = new Vector2(pos.x + 8 * brushRange, pos.z + 8 * brushRange);
            Vector2 min = new Vector2(pos.x - 8 * brushRange, pos.z - 8 * brushRange);
            m_hexEdgeMesh.AddQuad
                (
                    new Vector3(min.x, pos.y + 2, min.y),
                    new Vector3(max.x, pos.y + 2, min.y),
                    new Vector3(min.x, pos.y + 2, max.y),
                    new Vector3(max.x, pos.y + 2, max.y)
                );
            m_hexEdgeMesh.AddQuadColor
                (
                    new Color(0.18f, 1, 0.18f, 1f),
                    new Color(0.18f, 1, 0.18f, 1f)
                );
            m_hexEdgeMesh.InputMeshInfo();
        }
        else
        {
            m_hexEdgeMesh.Clear();
        }

    }

    public override void RefreshTarget(HexCell cell)
    {
        base.RefreshTarget(cell);

        if(SceneObj!=null)
        {
            SceneObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f) * ObjSize;
            HexMetrics.instance.editorSceneObject = SceneObj;
        }
    }
}

#endregion


//Mesh调节器(高度、水体、边界相关)
public class MeshModifier:SingletonDestory<MeshModifier>
{

    public TerrainBrush m_brush;
    List<HexGridChunk> m_refreshChunkList = new List<HexGridChunk>();//mesh基于chunk刷新
    List<HexCell> m_refreshCellList = new List<HexCell>();//sceneObject基于cell刷新,刷新mesh的时候也会刷新sceneObject

    public void DoEvent()
    {
        Event e = Event.current;
        switch(e.type)
        {
            case EventType.MouseDown:
                if(e.button == 0)//鼠标左键
                RefreshMesh(e);
                break;
            case EventType.MouseMove:
                RePaint(e);
                break;
            case EventType.Repaint:
                RePaint(e);
                break;
        }
    }

    void RePaint(Event e)
    {
        Ray inputRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hit;
        HexCell centerCell = null;
        if (Physics.Raycast(inputRay,out hit))
        {
            centerCell = HexGrid.instance.GetCell(hit.point);
            m_brush.RefreshBrush(hit.point, centerCell);
        }
    }

    //刷新地形mesh
    void RefreshMesh(Event e)
    {
        //Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Ray inputRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Debug.Log(inputRay.direction);
        RaycastHit hit;
        HexCell centerCell = null;
        int centerX = 0;
        int centerZ = 0;

        m_refreshChunkList.Clear();
        m_refreshCellList.Clear();
        if (Physics.Raycast(inputRay, out hit))
        {
            centerCell = HexGrid.instance.GetCell(hit.point);
            TerrainEditor.UndoAdd(centerCell, m_brush);
            m_refreshChunkList.Add(centerCell.chunkParent);
            m_refreshCellList.Add(centerCell);
            centerX = centerCell.coordinates.X;
            centerZ = centerCell.coordinates.Z;
            Vector3 pos = hit.point;
            HexDirection clickDir = HexGrid.instance.GetPointDirection(new Vector2(pos.x - centerCell.transform.position.x, pos.z - centerCell.transform.position.z));
            for (int l = 0, z = centerZ; z >= centerZ - m_brush.brushRange + 1; l++, z--)
            {
                for (int x = centerX - m_brush.brushRange + 1 + l; x <= centerX + m_brush.brushRange - 1; x++)
                {
                    EditCell(clickDir, HexGrid.instance.GetCell(new HexCoordinates(x, z)), m_brush);
                }
            }

            for (int l = 1, z = centerZ + 1; z <= centerZ + m_brush.brushRange - 1; l++, z++)
            {
                for (int x = centerX - m_brush.brushRange + 1; x <= centerX + m_brush.brushRange - 1 - l; x++)
                {
                    EditCell(clickDir, HexGrid.instance.GetCell(new HexCoordinates(x, z)), m_brush);
                }
            }
            for (int i = 0; i < m_refreshChunkList.Count; i++)
            {
                if (m_brush.m_editorType == EditorType.WaterEditor)
                {
                    m_refreshChunkList[i].Refresh(MeshClass.waterMesh);
                    m_refreshChunkList[i].Refresh(MeshClass.waterEdgeMesh);
                }
                else
                {
                    m_refreshChunkList[i].Refresh();
                    m_refreshChunkList[i].sceneObjectMgr.Refresh();
                    // refreshChunkList[i].sceneObjectMgr.Refresh(refreshCellList);
                    //StartCoroutine(WaitMesh(m_refreshChunkList[i]));
                }
            }


        }
    }

    void EditCell(HexDirection clickDir, HexCell cell,TerrainBrush brush)
    {

        if (cell == null)
            return;

        brush.RefreshTarget(clickDir, cell);

        List<HexGridChunk> neigborChunk = cell.NeighorChunk();
        if (neigborChunk.Count > 0)
        {
            for (int i = 0; i < neigborChunk.Count; i++)
            {
                if (!m_refreshChunkList.Contains(neigborChunk[i]))
                {
                    m_refreshChunkList.Add(neigborChunk[i]);
                }
            }
        }

    }


}

public class MaterialModifier:SingletonDestory<MaterialModifier>
{
    public TerrainBrush m_brush;

    List<HexGridChunk> m_refreshChunkList = new List<HexGridChunk>();//mesh基于chunk刷新

    public void DoEvent()
    {
        Event e = Event.current;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)//鼠标左键
                    RefreshMesh(e);
                break;
            case EventType.MouseMove:
                RePaint(e);
                break;
            case EventType.Repaint:
                RePaint(e);
                break;
        }
    }

    void RePaint(Event e)
    {
        Ray inputRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hit;
        HexCell centerCell = null;
        if (Physics.Raycast(inputRay, out hit))
        {
            centerCell = HexGrid.instance.GetCell(hit.point);
            m_brush.RefreshBrush(hit.point, centerCell);
        }
    }


    //刷新地形mesh
    void RefreshMesh(Event e)
    {
        //Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Ray inputRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Debug.Log(inputRay.direction);
        RaycastHit hit;
        HexCell centerCell = null;
        int centerX = 0;
        int centerZ = 0;

        m_refreshChunkList.Clear();
        if (Physics.Raycast(inputRay, out hit))
        {
            centerCell = HexGrid.instance.GetCell(hit.point);
            m_refreshChunkList.Add(centerCell.chunkParent);
            centerX = centerCell.coordinates.X;
            centerZ = centerCell.coordinates.Z;
            Vector3 pos = hit.point;
            for (int l = 0, z = centerZ; z >= centerZ - m_brush.brushRange + 1; l++, z--)
            {
                for (int x = centerX - m_brush.brushRange + 1 + l; x <= centerX + m_brush.brushRange - 1; x++)
                {
                    EditCell( HexGrid.instance.GetCell(new HexCoordinates(x, z)), m_brush);
                }
            }

            for (int l = 1, z = centerZ + 1; z <= centerZ + m_brush.brushRange - 1; l++, z++)
            {
                for (int x = centerX - m_brush.brushRange + 1; x <= centerX + m_brush.brushRange - 1 - l; x++)
                {
                    EditCell(HexGrid.instance.GetCell(new HexCoordinates(x, z)), m_brush);
                }
            }
            for (int i = 0; i < m_refreshChunkList.Count; i++)
            {
                m_refreshChunkList[i].Refresh();
            }

        }
    }

    void EditCell(HexCell cell, TerrainBrush brush)
    {
        if (cell == null)
            return;

        brush.RefreshTarget(cell);

        List<HexGridChunk> neigborChunk = cell.NeighorChunk();
        if (neigborChunk.Count > 0)
        {
            for (int i = 0; i < neigborChunk.Count; i++)
            {
                if (!m_refreshChunkList.Contains(neigborChunk[i]))
                {
                    m_refreshChunkList.Add(neigborChunk[i]);
                }
            }
        }

    }


}

public class SceneObjModifier:SingletonDestory<SceneObjModifier>
{

    public bool isBrushSceneObject = true;

    public SceneObjBrush m_brush;

    List<HexGridChunk> m_refreshChunkList = new List<HexGridChunk>();//mesh基于chunk刷新

    public void DoEvent()
    {
        Event e = Event.current;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)//鼠标左键
                    RefreshSceneObj(e);
                break;
            case EventType.MouseMove:
                RePaint(e);
                break;
            case EventType.Repaint:
                RePaint(e);
                break;
        }
    }

    void RePaint(Event e)
    {
        Ray inputRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hit;
        HexCell centerCell = null;
        if (Physics.Raycast(inputRay, out hit))
        {
            centerCell = HexGrid.instance.GetCell(hit.point);
            m_brush.RefreshBrush(hit.point, centerCell);
        }
    }

    void RefreshSceneObj(Event e)
    {
        Ray inputRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hit;
        HexCell centerCell = null;
        if (Physics.Raycast(inputRay, out hit))
        {
            centerCell = HexGrid.instance.GetCell(hit.point);
            m_brush.RefreshTarget(centerCell);
            if (m_brush.SceneObjOperationType == SceneObjBrush.OperationType.Add)
            {
                if (m_brush.IsBrushSceneObject)//大量随机
                {
                    if (HexMetrics.instance.editorSceneObject == null)
                    {
                        EditorUtility.DisplayDialog("错误","所编辑的场景物体为空","确认");
                        return;
                    }

                    Vector2 max = new Vector2(hit.point.x + 8 * m_brush.brushRange, hit.point.z + 8 * m_brush.brushRange);
                    Vector2 min = new Vector2(hit.point.x - 8 * m_brush.brushRange, hit.point.z - 8 * m_brush.brushRange);
                    float px, pz, py = 0;
                    for (int i = 0; i < m_brush.SceneObjectDensity * m_brush.brushRange; i++)
                    {
                        px = UnityEngine.Random.Range(min.x, max.x);
                        pz = UnityEngine.Random.Range(min.y, max.y);
                        if (Physics.Raycast(new Vector3(px, 35, pz), Vector3.down, out hit))
                        {
                            if (hit.collider.gameObject.tag != "Mesh")
                                continue;
                            py = hit.point.y;
                        }
                        Vector3 tPosition = new Vector3(px, py, pz);
                        centerCell = HexGrid.instance.GetCell(tPosition);
                        if (centerCell.isUnderWaterLevel || centerCell == null)
                        {
                            continue;
                        }
                        GameObject tSceneObject = GameObjectPool.instance.GetPoolChild(HexMetrics.instance.editorSceneObject.name, HexMetrics.instance.editorSceneObject);
                        tSceneObject.transform.SetParent(centerCell.chunkParent.sceneObjectMgr.transform);
                        tSceneObject.transform.position = tPosition;
                        tSceneObject.transform.rotation = Quaternion.Euler(0f, 360f * UnityEngine.Random.value, 0f);
                        tSceneObject.SetActive(true);
                        HexDirection clickDir = HexGrid.instance.GetPointDirection(new Vector2(tPosition.x - centerCell.transform.position.x, tPosition.z - centerCell.transform.position.z));
                        if (tSceneObject.GetComponent<SceneObjectClass>() == null) tSceneObject.AddComponent<SceneObjectClass>();
                        tSceneObject.GetComponent<SceneObjectClass>().SetInfo(tSceneObject.transform.localPosition, tSceneObject.transform.localRotation, clickDir, centerCell);
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
                            if(HexMetrics.instance.editorSceneObject == null)
                            {
                                EditorUtility.DisplayDialog("错误", "所编辑的场景物体为空", "确认");
                                return;
                            }
                            GameObject tSceneObject = GameObjectPool.instance.GetPoolChild(HexMetrics.instance.editorSceneObject.name, HexMetrics.instance.editorSceneObject);
                            tSceneObject.transform.SetParent(centerCell.chunkParent.sceneObjectMgr.transform);
                            tSceneObject.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                            tSceneObject.transform.rotation = Quaternion.Euler(0f, 360f * UnityEngine.Random.value, 0f);
                            tSceneObject.SetActive(true);
                            HexDirection clickDir = HexGrid.instance.GetPointDirection(new Vector2(hit.point.x - centerCell.transform.position.x, hit.point.z - centerCell.transform.position.z));
                            if (tSceneObject.GetComponent<SceneObjectClass>() == null) tSceneObject.AddComponent<SceneObjectClass>();
                            tSceneObject.GetComponent<SceneObjectClass>().SetInfo(tSceneObject.transform.localPosition, tSceneObject.transform.localRotation, clickDir, centerCell);
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
            else if (m_brush.SceneObjOperationType == SceneObjBrush.OperationType.Delete)
            {
                int centerX = centerCell.coordinates.X;
                int centerZ = centerCell.coordinates.Z;
                if (m_brush.IsBrushSceneObject && m_brush.SceneObjOperationType == SceneObjBrush.OperationType.Delete)
                {
                    for (int l = 0, z = centerZ; z >= centerZ - m_brush.brushRange + 1; l++, z--)
                    {
                        for (int x = centerX - m_brush.brushRange + 1 + l; x <= centerX + m_brush.brushRange - 1; x++)
                        {
                            HexCell cell = HexGrid.instance.GetCell(new HexCoordinates(x, z));
                            cell.chunkParent.sceneObjectMgr.MinusSceneObject(cell);
                        }
                    }
                    for (int l = 1, z = centerZ + 1; z <= centerZ + m_brush.brushRange - 1; l++, z++)
                    {
                        for (int x = centerX - m_brush.brushRange + 1; x <= centerX + m_brush.brushRange - 1 - l; x++)
                        {
                            HexCell cell = HexGrid.instance.GetCell(new HexCoordinates(x, z));
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

}