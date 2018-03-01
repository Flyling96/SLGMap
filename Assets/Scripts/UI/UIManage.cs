using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManage : Singleton<UIManage> {

    public GameObject tipWnd = null;
    public GameObject uiRoot = null;
    public TipUI inputWnd = null;

    public GameObject UIRoot
    {
        get
        {
            if (uiRoot == null)
                uiRoot = HexMapEditor.uiRoot;
            return uiRoot;
        }
    }

    public void ShowInputWnd(List<string> inputName, TipUI.OnInputConfirm confirm, TipUI.OnInputCancel cancel)
    {
        if(inputWnd==null)
        {
            GameObject temp =  GameObject.Instantiate(Resources.Load("Prefabs/UIPrefabs/InputWnd")as GameObject);
            temp.transform.SetParent(UIRoot.transform);
            temp.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            temp.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            inputWnd = temp.GetComponent<TipUI>();   
            inputWnd.ShowInputWnd(inputName, confirm, cancel);
        }
        else
        {
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

}
