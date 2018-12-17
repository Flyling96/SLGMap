using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


public class CreateMapWnd : ScriptableWizard
{

    string mapName = "";
    int chunkCountX = 0;
    int chunkCountZ = 0;
    bool isUseTexture = true;
    bool isSaveAfterCreate = true;

    protected override bool DrawWizardGUI()
    {
        mapName = EditorGUILayout.TextField("地形名称", mapName);
        chunkCountX = EditorGUILayout.IntField("地形宽度", chunkCountX);
        chunkCountZ = EditorGUILayout.IntField("地形长度", chunkCountZ);
        isUseTexture = EditorGUILayout.Toggle("地形是否使用纹理", isUseTexture);
        isSaveAfterCreate = EditorGUILayout.Toggle("创建完是否保存", isSaveAfterCreate);
        return true;
    }

    private void OnWizardCreate()
    {
        TerrainEditor.ClearUndoRedoStack();

        if (TerrainEditor.TerrainParent.transform != null)
        {
            foreach (Transform child in TerrainEditor.TerrainParent.transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        TerrainEditor.terrainName = mapName;
        HexGrid.instance.ChangeSize(chunkCountX, chunkCountZ);
        HexMetrics.instance.isEditorTexture = isUseTexture;
        HexGrid.instance.isLoadPrefab = false;
        HexGrid.instance.NewMap(mapName);

        if (isSaveAfterCreate)
        {
            TerrainEditor.SaveHexChunkAsset(mapName);
        }

    }

    private void OnWizardUpdate()
    {
        errorString = "";
        isValid = true;
        if (string.IsNullOrEmpty(mapName))
        {
            errorString = "请输入要新建的地形名称";
            isValid = false;
            return;
        }
        if (chunkCountX < 1 || chunkCountX > 30 || chunkCountZ < 1 || chunkCountZ > 30)
        {
            errorString = "宽高须是[1,30]范围内的整数";
            isValid = false;
            return;
        }

        string mapFilePath = Application.dataPath + "/" + EditorConfig.instance.mapFileDirectory + "/" + mapName;
        if(Directory.Exists(mapFilePath))
        {
            errorString = "已存在该地形，若保存则会将其覆盖";
            return;
        }
    }

    private void OnWizardOtherButton()
    {
        Close();
    }
}
