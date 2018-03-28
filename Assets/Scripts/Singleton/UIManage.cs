using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIManage : Singleton<UIManage> {

    public GameObject tipWnd = null;
    public GameObject uiRoot = null;
    public TipUI inputWnd = null;
    public TipLineUI tipLine = null;


    public GameObject UIRoot
    {
        get
        {
            if (uiRoot == null)
                uiRoot = HexMapEditor.uiRoot;
            return uiRoot;
        }
    }

    public void ShowInputWnd(List<string> inputName, TipUI.OnInputConfirm confirm, TipUI.OnInputCancel cancel,string title)
    {
        if(inputWnd==null)
        {
            GameObject temp =  GameObject.Instantiate(Resources.Load("Prefabs/UIPrefabs/InputWnd")as GameObject);
            temp.transform.SetParent(UIRoot.transform);
            temp.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            temp.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            inputWnd = temp.GetComponent<TipUI>();
            inputWnd.transform.Find("Title").GetComponent<Text>().text = title;
            inputWnd.ShowInputWnd(inputName, confirm, cancel);
        }
        else
        {
            inputWnd.transform.Find("Title").GetComponent<Text>().text = title;
            inputWnd.gameObject.SetActive(true);
            inputWnd.ShowInputWnd(inputName, confirm, cancel);
        }
    }

    public void HideInputWnd()
    {
        if(inputWnd!=null)
        {
            inputWnd.gameObject.SetActive(false);
        }
    }

    public void ShowTipLine(string content,float time)
    {
        if (tipLine == null)
        {
            GameObject temp = GameObject.Instantiate(Resources.Load("Prefabs/UIPrefabs/TipLine") as GameObject);
            temp.transform.SetParent(UIRoot.transform);
            temp.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            temp.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            tipLine = temp.GetComponent<TipLineUI>();
            tipLine.ShowTipLine(content, time);
        }
        else
        {
            tipLine.gameObject.SetActive(true);
            tipLine.ShowTipLine(content, time);
        }
    }

    public void AddPool(string name, Transform parent)
    {
        foreach (Transform child in parent)
        {
            GameObjectPool.instance.InsertChild(name, child.gameObject);
        }
    }

}
