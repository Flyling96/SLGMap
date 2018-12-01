using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[ExecuteInEditMode]
public class HexTerrain : MonoBehaviour
{

    void Awake()
    {
        HexMetrics.instance.isEditor = true;
        ConfigDateManage.instance.InitData();
        HexMetrics.instance.Init();
        LoadMap("map001");
    }

    void LoadMap(string fileName)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(Application.streamingAssetsPath + "/" + fileName, FileMode.Open)))
        {
            HexGrid.instance.Load(reader);
        }

        HexGrid.instance.Refresh();
    }
}

