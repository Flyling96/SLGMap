using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainColorSelectItem : DownSelectWndItem
{
    public Color color = new Color(1, 1, 1, 1);
    public override void OnItemClick(bool isOn)
    {
        base.OnItemClick(isOn);
        if (isOn)
        {
            HexMetrics.instance.editorColor = color;
        }
    }

    public override void Init()
    {
        base.Init();
        if (transform.Find("IconItem/Image") != null)
        {
            transform.Find("IconItem/Image").GetComponent<Image>().color = color;
        }
    }

    public override void SetInfo(BaseInfo baseInfo)
    {
        TerrainColorInfo info = (TerrainColorInfo)baseInfo;
        base.SetInfo(info);
        iconName = info.iconName;
        color = info.itemColor;
    }
}
