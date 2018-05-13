using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ActionWnd : MonoBehaviour {

    public delegate void OnClickButton(int index);
    public event OnClickButton ClickButton;

    Button button;
    private void Awake()
    {
        button = transform.Find("Button").GetComponent<Button>();
    }

    public void ShowActionWnd(OnClickButton cb,List<string> buttonNames)
    {
        ClickButton = cb;
        for(int i=0;i<buttonNames.Count;i++)
        {
            GameObject item = GameObject.Instantiate(button.gameObject);
            item.transform.parent = this.transform;
            item.transform.Find("Text").GetComponent<Text>().text = buttonNames[i];
            item.name = "Button" + i;
            item.SetActive(true);
            item.GetComponent<Button>().onClick.AddListener(delegate() { ClickButton(item.name[item.name.Length-1]-'0'); });
        }
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(95, buttonNames.Count * 42);

    }

    public void HideActionWnd()
    {
        foreach(Transform child in transform)
        {
            if(child.name!="Button")
            {
                Destroy(child.gameObject);
            }
        }
    }

    

}
