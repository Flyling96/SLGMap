using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum EditorType
{
    HeightEditor,
    WaterEditor,
    EdgeEditor,
    MaterialEditor,
    SceneObjEditor,
}

public enum EditorTagType
{
    TerrainEditor,
    MaterialEditor,
    SceneObjEditor,
}


[CustomEditor(typeof(HexTerrain))]
public class TerrainEditor : Editor{

    public EditorType editorType = EditorType.HeightEditor;
    EditorTagType editorTagType = EditorTagType.TerrainEditor;

    string[] EDGE_BRUSEH_NAMES = { "斜边", "台阶" };
    int[] EDGE_BRUSH_VALUES = { 0, 1 };

    string[] MATERIAL_TEXTURE_NAMES = { "绿地", "泥地", "雪地", "沙地", "石地" };
    int[] MATERIAL_TEXTURE_VALUES = { 0, 1, 2, 3, 4, 5 };

    public HexEdgeMesh hexEdgeMesh;

    public HeightBrush heightBrush;
    public MaterialBrush materialBrush;
    public SceneObjBrush sceneObjBrush;
    public EdgeBrush edgeBrush;
    public WaterBrush waterBrush;

    TerrainBrush currentBrush;

    private void Awake()
    {
        heightBrush = new HeightBrush(HexGrid.instance.HexEditMesh);
        materialBrush = new MaterialBrush(HexGrid.instance.HexEditMesh);
        sceneObjBrush = new SceneObjBrush(HexGrid.instance.HexEditMesh);
        edgeBrush = new EdgeBrush( HexGrid.instance.HexEditMesh);
        waterBrush = new WaterBrush(HexGrid.instance.HexEditMesh);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        string[] captions = { "地形编辑","材质编辑","场景物体编辑"};
        editorTagType =  (EditorTagType)GUILayout.Toolbar((int)editorTagType, captions);
        switch(editorTagType)
        {
            case EditorTagType.TerrainEditor:
                DrawTerrainEditorGUI(captions[0]);
                break;
            case EditorTagType.MaterialEditor:
                editorType = EditorType.MaterialEditor;
                DrawMaterialEditorGUI(captions[1]);
                break;
            case EditorTagType.SceneObjEditor:
                editorType = EditorType.SceneObjEditor;
                DrawSceneObjEditorGUI(captions[2]);
                break;
        }
        
    }

    private void OnSceneGUI()
    {
        HandleUtility.AddDefaultControl(0);
        switch(editorType)
        {
            case EditorType.HeightEditor:
                currentBrush = heightBrush;
                MeshModifier.instance.m_brush = currentBrush;
                MeshModifier.instance.DoEvent();
                break;
            case EditorType.WaterEditor:
                currentBrush = waterBrush;
                MeshModifier.instance.m_brush = currentBrush;
                MeshModifier.instance.DoEvent();
                break;
            case EditorType.EdgeEditor:
                currentBrush = edgeBrush;
                MeshModifier.instance.m_brush = currentBrush;
                MeshModifier.instance.DoEvent();
                break;
            case EditorType.MaterialEditor:
                currentBrush = materialBrush;
                MaterialModifier.instance.m_brush = currentBrush;
                MaterialModifier.instance.DoEvent();
                break;
            case EditorType.SceneObjEditor:
                currentBrush = sceneObjBrush;
                SceneObjModifier.instance.m_brush = sceneObjBrush;
                SceneObjModifier.instance.DoEvent();
                break;

        }
    }


    private void DrawTerrainEditorGUI(string caption)
    {
        if (EditorUtils.DrawHeader("地形编辑","TerrainEditor"))
        {
            string[] captions = { "地形高度", "水体高度", "地形边界" };

            if (editorType > EditorType.EdgeEditor) editorType = EditorType.HeightEditor;
            editorType = (EditorType)GUILayout.Toolbar((int)editorType, captions);

            switch(editorType)
            {
                case EditorType.HeightEditor:
                    EditorUtils.BeginContents();
                    heightBrush.brushRange = EditorGUILayout.IntSlider("笔刷范围", heightBrush.brushRange, 1, 5);
                    heightBrush.Elevation = EditorGUILayout.IntSlider("地形高度", heightBrush.Elevation, 0, 5);
                    EditorUtils.EndContents();
                    break;
                case EditorType.WaterEditor:
                    EditorUtils.BeginContents();
                    waterBrush.brushRange = EditorGUILayout.IntSlider("笔刷范围", waterBrush.brushRange, 1, 5);
                    waterBrush.WaterLevel = EditorGUILayout.IntSlider("水体高度", waterBrush.WaterLevel, 0, 5);
                    EditorUtils.EndContents();
                    break;
                case EditorType.EdgeEditor:
                    EditorUtils.BeginContents();
                    edgeBrush.EditEdgeType = (EdgeBrush.EditorEdgeType)EditorGUILayout.IntPopup("边界类型", (int)edgeBrush.EditEdgeType, EDGE_BRUSEH_NAMES, EDGE_BRUSH_VALUES);
                    edgeBrush.IsWholeEditor = EditorGUILayout.Toggle("整个六边形编辑", edgeBrush.IsWholeEditor);
                    edgeBrush.brushRange = EditorGUILayout.IntSlider("笔刷范围", waterBrush.brushRange, 1, 5);
                    EditorUtils.EndContents();
                    break;

            }
        }
    }

    private void DrawMaterialEditorGUI(string caption)
    {
        if(EditorUtils.DrawHeader("材质笔刷", "TerrainEditor"))
        {
            EditorUtils.BeginContents();
            materialBrush.brushRange = EditorGUILayout.IntSlider("笔刷范围", materialBrush.brushRange, 1, 5);
            EditorUtils.EndContents();
        }
        if (EditorUtils.DrawHeader("材质编辑", "TerrainEditor"))
        {
            EditorUtils.BeginContents();
            if(HexMetrics.instance.isEditorTexture)
            {
                materialBrush.TerrainType = (TerrainTypes)EditorGUILayout.IntPopup("材质类型", (int)materialBrush.TerrainType, MATERIAL_TEXTURE_NAMES, MATERIAL_TEXTURE_VALUES);
                materialBrush.EditColor = EditorGUILayout.ColorField(materialBrush.EditColor);
            }
            else
            {
                materialBrush.EditColor = EditorGUILayout.ColorField(materialBrush.EditColor);
            }

            EditorUtils.EndContents();
        }
    }

    private void DrawSceneObjEditorGUI(string caption)
    {
        if (EditorUtils.DrawHeader(caption, "TerrainEditor"))
        {
            EditorUtils.BeginContents();
            EditorUtils.EndContents();
        }
    }



}
