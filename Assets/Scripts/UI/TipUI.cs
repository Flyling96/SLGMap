using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InputType
{
    InputField,
    Toggle,
}

public class TipUI : MonoBehaviour {

    public delegate void OnInputConfirm(Dictionary<string, string> inputDic,ref bool isSuccessful);
    public delegate void OnInputCancel(Dictionary<string, string> inputDic,ref bool isSuccessful);

    OnInputConfirm onConfirm = null;
    OnInputCancel onCancel = null;
    // Use this for initialization
 
    public GameObject inputItem = null;
    public GameObject toggleItem = null;
    Dictionary<string,string> inputDic = new Dictionary<string, string>();
    Dictionary<GameObject, InputType> inputItemType = new Dictionary<GameObject, InputType>();

    public void ShowInputWnd(List<string> inputName, OnInputConfirm confirm, OnInputCancel cancel,List<InputType> inputType)
    {
        inputItemType.Clear();
        AddPool(transform.Find("ScrollView/Viewport/Content"));

        onConfirm = confirm;
        onCancel = cancel;
        inputDic.Clear();
        GameObject item = null;
        for(int i=0;i<inputName.Count;i++)
        {
            if (inputType[i] == InputType.InputField)
            {
                item = GameObjectPool.instance.GetPoolChild(inputItem.name, inputItem);
            }
            else if(inputType[i] == InputType.Toggle)
            {
                item = GameObjectPool.instance.GetPoolChild(toggleItem.name, toggleItem);
            }

            item.transform.Find("Text").GetComponent<Text>().text = inputName[i];
            inputItemType.Add(item, inputType[i]);
            item.gameObject.transform.SetParent(transform.Find("ScrollView/Viewport/Content"));
            item.SetActive(true);
        }
    }

    public void ClickConfirm(bool isInput)
    {
        if(onConfirm!=null)
        {
            if(isInput)
            {
                foreach (KeyValuePair<GameObject, InputType> value in inputItemType)
                {
                    if (value.Value == InputType.InputField)
                    {
                        inputDic.Add(value.Key.transform.Find("Text").GetComponent<Text>().text, value.Key.transform.Find("InputField").GetComponent<InputField>().text);
                    }
                    else if (value.Value == InputType.Toggle)
                    {
                        inputDic.Add(value.Key.transform.Find("Text").GetComponent<Text>().text, value.Key.transform.GetComponentInChildren<Toggle>().isOn.ToString());
                    }
                }
                bool isSuccessful = false;
                onConfirm(inputDic, ref isSuccessful);
                if (isSuccessful == true)
                {
                    AddPool(transform.Find("ScrollView/Viewport/Content"));
                    UIManage.instance.HideInputWnd();
                }
                else
                {
                    inputDic.Clear();
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
                onCancel(inputDic, ref isSuccessful);
                if (isSuccessful == true)
                {
                    AddPool(transform.Find("ScrollView/Viewport/Content"));
                    UIManage.instance.HideInputWnd();
                }
                else
                {
                    inputDic.Clear();
                }
            }
        }
        else
        {
            if (isInput)
            {
                AddPool(transform.Find("ScrollView/Viewport/Content"));
                UIManage.instance.HideInputWnd();
            }
            gameObject.SetActive(false);
        }
    }


    void AddPool(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.GetComponentInChildren<InputField>())
            {
                UIManage.instance.AddPool(inputItem.name, child.gameObject);
            }
            else if (child.GetComponentInChildren<Toggle>())
            {
                UIManage.instance.AddPool(toggleItem.name, child.gameObject);
            }
        }
    }


}

