using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveMapWnd : ScriptableWizard
{
    private string saveMapName = "";

    protected override bool DrawWizardGUI()
    {
        saveMapName = TerrainEditor.terrainName;
        saveMapName = EditorGUILayout.TextField("地形名称", saveMapName);
        if(string.IsNullOrEmpty(saveMapName))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void OnWizardCreate()
    {

        TerrainEditor.SaveHexChunkAsset(saveMapName);

    }

    private void OnWizardUpdate()
    {
        errorString = "";
        isValid = true;
        if (string.IsNullOrEmpty(saveMapName))
        {
            errorString = "请输入要保存的地形名称";
            isValid = false;
            return;
        }
    }


    private void OnWizardOtherButton()
    {
        Close();
    }


}
