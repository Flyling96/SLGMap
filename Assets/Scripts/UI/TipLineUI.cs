using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipLineUI : MonoBehaviour
{
    Color contentColor;
    Color backgroundColor;
    private void Awake()
    {
        contentColor = ToolClass.instance.ConvertColor(50, 50, 50, 255);
        backgroundColor = ToolClass.instance.ConvertColor(255, 255, 255, 255);
    }

    public void ShowTipLine(string content, float time)
    {
        transform.Find("Content").GetComponent<Text>().text = content;
        transform.Find("Content").GetComponent<Text>().color = contentColor;
        transform.GetComponent<Image>().color = backgroundColor;
        StartCoroutine(HideTipLine(time));
    }

    IEnumerator HideTipLine(float hideTime)
    {
        float time = hideTime;
        Color contentColor = transform.Find("Content").GetComponent<Text>().color;
        Color backgroundColor = transform.GetComponent<Image>().color;
        Color contentColorEnd = new Color(contentColor.r, contentColor.g, contentColor.b, 0);
        Color backgroundColorEnd = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0);
        while (time > 0)
        {
            time -= Time.deltaTime;
            transform.Find("Content").GetComponent<Text>().color = Color.Lerp(contentColor, contentColorEnd, (hideTime - time) / hideTime);
            transform.GetComponent<Image>().color = Color.Lerp(backgroundColor, backgroundColorEnd, (hideTime - time) / hideTime);
            yield return null;
        }
        gameObject.SetActive(false);
    }

}
