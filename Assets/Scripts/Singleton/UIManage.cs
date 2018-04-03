using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

public class UIManage : Singleton<UIManage> {

    public GameObject tipWnd = null;
    public GameObject uiRoot = null;
    public TipUI inputWnd = null;
    public TipLineUI tipLine = null;
    public DownSelectWnd downSelectWnd = null;


    public GameObject UIRoot
    {
        get
        {
            if (uiRoot == null)
                uiRoot = HexMapEditor.uiRoot;
            return uiRoot;
        }
    }

    public void ShowDownSelectWnd(List<BaseInfo> info, DownSelectWndType type)
    {
        if(downSelectWnd==null)
        {
            GameObject temp = GameObject.Instantiate(Resources.Load("Prefabs/UIPrefabs/DownSelectWnd") as GameObject);
            temp.transform.SetParent(UIRoot.transform);
            temp.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            temp.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            downSelectWnd = temp.GetComponent<DownSelectWnd>();
            downSelectWnd.ShowDownSelectWnd(info, type);
        }
        else
        {
            downSelectWnd.gameObject.SetActive(true);
            downSelectWnd.ShowDownSelectWnd(info, type);
        }
    }

    public void HideDownSelectWnd(string name)
    {
        if (downSelectWnd != null)
        {
            HexMetrics.instance.isEditorTerrain = false;
            AddClearPool(name, downSelectWnd.transform.Find("ScrollView/Viewport/Content"));
            downSelectWnd.gameObject.SetActive(false);
        }
    }

    public void ShowInputWnd(List<string> inputName, TipUI.OnInputConfirm confirm, TipUI.OnInputCancel cancel,string title,List<InputType> inputType)
    {
        if(inputWnd==null)
        {
            GameObject temp =  GameObject.Instantiate(Resources.Load("Prefabs/UIPrefabs/InputWnd")as GameObject);
            temp.transform.SetParent(UIRoot.transform);
            temp.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            temp.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            inputWnd = temp.GetComponent<TipUI>();
            inputWnd.transform.Find("Title").GetComponent<Text>().text = title;
            inputWnd.ShowInputWnd(inputName, confirm, cancel, inputType);
        }
        else
        {
            inputWnd.transform.Find("Title").GetComponent<Text>().text = title;
            inputWnd.gameObject.SetActive(true);
            inputWnd.ShowInputWnd(inputName, confirm, cancel, inputType);
        }
    }

    public void HideInputWnd(string name)
    {
        if(inputWnd!=null)
        {
            AddPool(name, inputWnd.transform.Find("ScrollView/Viewport/Content"));
            inputWnd.gameObject.SetActive(false);
        }
    }

    public void HideInputWnd()
    {
        if (inputWnd != null)
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
            if (child.gameObject.activeSelf == false)
            {
                continue;
            }

            GameObjectPool.instance.InsertChild(name, child.gameObject);
        }
    }

    public void AddPool(string name, GameObject item)
    {
        if(item.activeSelf==false)
        {
            return;
        }
        GameObjectPool.instance.InsertChild(name, item);
    }

    public void AddPool(string name ,List<GameObject> item)
    {
        for(int i=0;i<item.Count;i++)
        {
            if(item[i].activeSelf==false)
            {
                continue;
            }
            GameObjectPool.instance.InsertChild(name, item[i]);
        }
    }

    public void AddClearPool(string name, Transform parent)
    {
        foreach (Transform child in parent)
        {
            if(child.gameObject.activeSelf == false)
            {
                continue;
            }
            foreach (var component in child.GetComponents<MonoBehaviour>())
            {
                Destroy(component);
            }
            GameObjectPool.instance.InsertChild(name, child.gameObject);
        }
    }

}
