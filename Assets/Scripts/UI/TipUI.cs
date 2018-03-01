using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipUI : MonoBehaviour {



    public delegate void OnInputConfirm(Dictionary<string, InputField> inputDic);
    public delegate void OnInputCancel(Dictionary<string, InputField> inputDic);

    OnInputConfirm onConfirm = null;
    OnInputCancel onCancel = null;
    // Use this for initialization
 
    public GameObject inputItem = null;
    Dictionary<string,InputField> inputDic = new Dictionary<string, InputField>();
    public void ShowInputWnd(List<string> inputName, OnInputConfirm confirm, OnInputCancel cancel)
    {
        onConfirm = confirm;
        onCancel = cancel;
        inputDic.Clear();
        foreach (string name in inputName)
        {
            GameObject item = GameObject.Instantiate(inputItem);
            item.transform.Find("Text").GetComponent<Text>().text = name;
            inputDic.Add(name,item.transform.Find("InputField").GetComponent<InputField>());
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
                onConfirm(inputDic);
            }
        }
    }

    public void ClickCancel(bool isInput)
    {
        if(onCancel!=null)
        {
            if (isInput)
            {
                onConfirm(inputDic);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
