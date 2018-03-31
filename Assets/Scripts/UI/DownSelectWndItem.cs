using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownSelectWndItem : MonoBehaviour {

    public int id;
    public string iconPath;
    public string iconName;

    void Start()
    {
        gameObject.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool isOn) { OnItemClick(isOn); });
    }

    public virtual void OnItemClick(bool isOn)
    {

    }

    public virtual void Init()
    {
        Sprite itemSprite = (Sprite)Resources.Load(iconPath);
        if(transform.Find("Image")!=null)
        {
            transform.Find("Image").GetComponent<Image>().sprite = itemSprite;
        }
        if(transform.Find("Text")!=null)
        {
            transform.Find("Text").GetComponent<Text>().text = iconName;
        }
    }

    public virtual void SetInfo(BaseInfo info)
    {
        id = info.id;
        iconPath = info.iconPath;
        iconName = info.iconName;
    }
}
