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
        int index = EditorGUILayout.Popup("地图列表", selectIndex, mapName.ToArray());
        bool change = selectIndex != index;
        selectIndex = index;
        return change;
    }

    private void OnWizardCreate()
    {
        Debug.Log(1);
    }

    private void OnWizardOtherButton()
    {
        Debug.Log(2);
    }

    private void OnWizardUpdate()
    {
        helpString = "请选择要打开的地图";
        isValid = (selectIndex != -1);
    }

}
