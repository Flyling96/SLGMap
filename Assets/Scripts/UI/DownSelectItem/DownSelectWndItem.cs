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
        gameObject.GetComponentInChildren<Toggle>().onValueChanged.AddListener(delegate(bool isOn) { OnItemClick(isOn); });
    }

    public virtual void OnItemClick(bool isOn)
    {
        if(isOn)
        {
           transform.Find("IconItem/Background").GetComponent<Image>().color = ToolClass.instance.ConvertColor(51, 255, 77, 255);
        }
        else
        {
            transform.Find("IconItem/Background").GetComponent<Image>().color = ToolClass.instance.ConvertColor(255, 255, 255, 255);
        }
    }

    public virtual void Init()
    {
        if (iconPath != "")
        {
            Texture2D itemTexture = (Texture2D)Resources.Load(iconPath);
            Sprite itemSprite = Sprite.Create(itemTexture, new Rect(0, 0, 64, 64), Vector2.zero);
            if (transform.Find("IconItem/Image") != null)
            {
                transform.Find("IconItem/Image").GetComponent<Image>().sprite = itemSprite;
            }
        }
        if(transform.Find("IconItem/Text") !=null)
        {
            transform.Find("IconItem/Text").GetComponent<Text>().text = iconName;
        }
    }

    public virtual void SetInfo(BaseInfo info)
    {
        id = info.id;
    }
}
