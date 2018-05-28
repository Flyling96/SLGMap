using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    Slider slider = null;
    public Text data = null;

	// Use this for initialization
	void Start () {
        slider = gameObject.GetComponent<Slider>();
        data = transform.Find("Data").GetComponent<Text>();
	}

    bool isChange = false;
    public void RefreshHUD(int now,int all,Vector3 position,float high,ref bool isRefreshInjuryHUD)
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(position.x, position.y + high);
        if (isChange)
            return;
        if((isRefreshInjuryHUD))
        {
            StartCoroutine(ValueChange(slider.value, (now / (all * 1.0f))));
            isRefreshInjuryHUD = false;
        }
    }

    public bool isDie()
    {
        return slider.value <= 0;
    }

    IEnumerator ValueChange(float old,float now)
    {
        isChange = true;
        float progress = 0;
        data.gameObject.SetActive(true);
        Vector2 dataStart = data.GetComponent<RectTransform>().anchoredPosition;
        Vector2 dataEnd = new Vector2(dataStart.x, dataStart.y + 50);
        while (progress<1)
        {
            progress += 0.02f;
            slider.value = Mathf.Lerp(old, now, progress);
            data.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(dataStart, dataEnd, progress);
            data.color = Color.Lerp(ToolClass.instance.ConvertColor("#FF4B4BFF"), ToolClass.instance.ConvertColor("#FF4B4B00"), progress);
            yield return null;
        }
        data.gameObject.SetActive(false);
        data.GetComponent<RectTransform>().anchoredPosition = dataStart;
        data.color = ToolClass.instance.ConvertColor("#FF4B4BFF");
        isChange = false;
    }
}
