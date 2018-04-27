using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainSelectItem : DownSelectWndItem {

    TerrainTypes terrainType = 0;


    public override void OnItemClick(bool isOn)
    {
        base.OnItemClick(isOn);
        if(isOn)
        {
            HexMetrics.instance.editorTerrainType = terrainType;
        }
    }

    public override void Init()
    {
        base.Init();
        if (transform.Find("IconItem/Image") != null)
        {
            transform.Find("IconItem/Image").GetComponent<Image>().color = new Color(1,1,1,1);
        }
    }

    public override void SetInfo(BaseInfo baseInfo)
    {
        TerrainTextureInfo info = (TerrainTextureInfo)baseInfo;
        base.SetInfo(info);
        iconPath = info.iconPath;
        iconName = info.iconName;
        terrainType = info.terrainType;
    }


}
