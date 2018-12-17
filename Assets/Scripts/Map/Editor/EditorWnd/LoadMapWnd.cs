using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LoadMapWnd : ScriptableWizard {

    private List<string> mapName;
    private int selectIndex = -1;

    protected override bool DrawWizardGUI()
    {
        mapName = TerrainEditor.GetLoadMapName();
        if (mapName.Count > 0 && selectIndex == -1) selectIndex = 0;
        int index = EditorGUILayout.Popup("地形列表", selectIndex, mapName.ToArray());
        selectIndex = index;
        return selectIndex != -1; 
    }

    private void OnWizardCreate()
    {
        if (mapName.Count > selectIndex)
        {
            TerrainEditor.ClearUndoRedoStack();

            if (TerrainEditor.TerrainParent.transform != null)
            {
                foreach (Transform child in TerrainEditor.TerrainParent.transform)
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            HexGrid.instance.isLoadPrefab = true;
            TerrainEditor.LoadMapAsset(mapName[selectIndex]);
        }
    }


    private void OnWizardOtherButton()
    {
        Close();
    }

    private void OnWizardUpdate()
    {
        helpString = "请选择要加载的地形";
        isValid = (selectIndex != -1);
    }

}
