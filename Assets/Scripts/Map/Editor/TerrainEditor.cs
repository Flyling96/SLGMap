﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum EditorType
{
    HeightEditor,
    MaterialEditor,
    SceneObjEditor,
    WaterEditor,
}


[CustomEditor(typeof(HexMapEditor))]
public class TerrainEditor : Editor{

    public EditorType editorType = EditorType.HeightEditor;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        string[] captions = { "高度编辑","材质编辑","场景物体编辑","水体编辑"};
        editorType =  (EditorType)GUILayout.Toolbar((int)editorType, captions);
        switch(editorType)
        {
            case EditorType.HeightEditor:
                DrawHeightEditorGUI(captions[0]);
                break;
            case EditorType.MaterialEditor:
                DrawMaterialEditorGUI(captions[1]);
                break;
            case EditorType.SceneObjEditor:
                DrawSceneObjEditorGUI(captions[2]);
                break;
        }
    }

    private void DrawHeightEditorGUI(string caption)
    {
        if(EditorUtils.DrawHeader(caption,"TerrainEditor"))
        {
            EditorUtils.BeginContents();
            EditorUtils.EndContents();
        }
    }

    private void DrawMaterialEditorGUI(string caption)
    {
        if (EditorUtils.DrawHeader(caption, "TerrainEditor"))
        {
            EditorUtils.BeginContents();
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
