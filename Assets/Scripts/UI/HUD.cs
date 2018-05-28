using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    Slider slider = null;

	// Use this for initialization
	void Start () {
        slider = gameObject.GetComponent<Slider>();
		
	}
	
    public void RefreshHUD(int now,int all,Vector3 position,float high)
    {
        slider.value = now / (all * 1.0f);
        gameObject.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(position.x , position.y+high);
    }
}
