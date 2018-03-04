using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolClass : Singleton<ToolClass> {

    //颜色转换
    public Color ConvertColor(int r, int g, int b, int a)
    {
        return new Color((float)r / 255, (float)g / 255, (float)b / 255, (float)a / 255);
    }

}
