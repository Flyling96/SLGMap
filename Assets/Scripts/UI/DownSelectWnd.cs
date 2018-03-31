using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DownSelectWndType
{
    terrainTexture,
}
public class DownSelectWnd : MonoBehaviour {


    public GameObject selectItem = null;

    Transform root = null;
    List<DownSelectWndItem> itemClass = null;

    private void Start()
    {
        root = transform.Find("ScrollView/Viewport/Content");
    }

    public void Init(List<BaseInfo> infoList, DownSelectWndType type)
    {
        for(int i=0;i<infoList.Count;i++)
        {
            GameObject item = GameObject.Instantiate(selectItem);
            item.transform.parent = root;
            ItemAddComponent(item, DownSelectWndType.terrainTexture);
        }
    } 

    void ItemAddComponent(GameObject item,DownSelectWndType type)
    {
        switch(type)
        {
            case DownSelectWndType.terrainTexture:
                item.AddComponent<TerrainSelectItem>();
                break;
        }
    }

    public void SetInfo(List<BaseInfo> infoList,List<DownSelectWndItem> itemList)
    {
        //for(int i)
    }
}
