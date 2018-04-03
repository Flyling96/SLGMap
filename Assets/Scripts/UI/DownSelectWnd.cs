using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DownSelectWndType
{
    terrainTexture,
    terrainColor,
}
public class DownSelectWnd : MonoBehaviour {


    public GameObject selectItem = null;
    Button backBtn = null;


    List<DownSelectWndItem> itemClass = new List<DownSelectWndItem>();

    private void Start()
    {
        backBtn = transform.Find("Back").GetComponent<Button>();
        backBtn.onClick.AddListener(BackButton);
    }

    void BackButton()
    {
        UIManage.instance.HideDownSelectWnd(selectItem.name);
    }

    public void ShowDownSelectWnd(List<BaseInfo> infoList, DownSelectWndType type)
    {
        itemClass.Clear();
        UIManage.instance.AddClearPool(selectItem.name, transform.Find("ScrollView/Viewport/Content"));
        for (int i=0;i<infoList.Count;i++)
        {
            GameObject item = GameObjectPool.instance.GetPoolChild(selectItem.name, selectItem);
            item.transform.SetParent(transform.Find("ScrollView/Viewport/Content"));
            ItemAddComponent(item, type);
        }
        SetInfo(infoList, itemClass);
    } 

    void ItemAddComponent(GameObject item,DownSelectWndType type)
    {
        switch(type)
        {
            case DownSelectWndType.terrainTexture:
                {
                    if (item.GetComponent<TerrainSelectItem>() == null)
                    {
                        item.AddComponent<TerrainSelectItem>();
                    }
                    itemClass.Add(item.GetComponent<TerrainSelectItem>());
                    break;
                }
            case DownSelectWndType.terrainColor:
                {
                    if (item.GetComponent<TerrainColorSelectItem>() == null)
                    {
                        item.AddComponent<TerrainColorSelectItem>();
                    }
                    itemClass.Add(item.GetComponent<TerrainColorSelectItem>());
                    break;
                }
        }
    }

    void SetInfo(List<BaseInfo> infoList,List<DownSelectWndItem> itemList)
    {
        for(int i=0;i<itemList.Count;i++)
        {
            itemList[i].SetInfo(infoList[i]);
            itemList[i].Init();
        }
    }
}
