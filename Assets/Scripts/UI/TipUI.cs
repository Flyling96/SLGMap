using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipUI : MonoBehaviour {


    public delegate void OnInputConfirm(Dictionary<string, InputField> inputDic,ref bool isSuccessful);
    public delegate void OnInputCancel(Dictionary<string, InputField> inputDic,ref bool isSuccessful);

    OnInputConfirm onConfirm = null;
    OnInputCancel onCancel = null;
    // Use this for initialization
 
    public GameObject inputItem = null;
    Dictionary<string,InputField> inputDic = new Dictionary<string, InputField>();
    public void ShowInputWnd(List<string> inputName, OnInputConfirm confirm, OnInputCancel cancel)
    {
        UIManage.instance.AddPool(inputItem.name, transform.Find("ScrollView/Viewport/Content"));
        onConfirm = confirm;
        onCancel = cancel;
        inputDic.Clear();
        int i = 1;
        GameObject item = null;
        foreach (string name in inputName)
        {
            item = GameObjectPool.instance.GetPoolChild(inputItem.name, inputItem);
            item.transform.Find("Text").GetComponent<Text>().text = name;
            inputDic.Add(name,item.transform.Find("InputField").GetComponent<InputField>());
            item.gameObject.transform.SetParent(transform.Find("ScrollView/Viewport/Content"));
            item.SetActive(true);
            i++;
        }
    }

    public void ClickConfirm(bool isInput)
    {
        if(onConfirm!=null)
        {
            if(isInput)
            {
                bool isSuccessful = false;
                onConfirm(inputDic, ref isSuccessful);
                if (isSuccessful == true)
                {
                    UIManage.instance.AddPool(inputItem.name, transform.Find("ScrollView/Viewport/Content"));
                    UIManage.instance.HideInputWnd();
                }
            }
        }
    }

    public void ClickCancel(bool isInput)
    {
        if (onCancel != null)
        {
            if (isInput)
            {
                bool isSuccessful = false;
                onConfirm(inputDic, ref isSuccessful);
                if (isSuccessful == true)
                {
                    UIManage.instance.AddPool(inputItem.name, transform.Find("ScrollView/Viewport/Content"));
                    UIManage.instance.HideInputWnd();
                }
            }
        }
        else
        {
            if (isInput)
            {
                UIManage.instance.AddPool(inputItem.name, transform.Find("ScrollView/Viewport/Content"));
                UIManage.instance.HideInputWnd();
            }
            gameObject.SetActive(false);
        }
    }



}

