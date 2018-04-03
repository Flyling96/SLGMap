using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolClass : Singleton<ToolClass> {

    //颜色转换
    public Color ConvertColor(int r, int g, int b, int a)
    {
        return new Color((float)r / 255, (float)g / 255, (float)b / 255, (float)a / 255);
    }

    public Color ConvertColor(string code)
    {
        if(code[0]!='#'&&code.Length!=7)
        {
            return new Color(1, 1, 1, 1);
        }
        int r = ColorNumber(code[1], code[2]);
        int g = ColorNumber(code[3], code[4]);
        int b = ColorNumber(code[5], code[6]);
        int a = ColorNumber(code[7], code[8]);
        return new Color(r/255.0f, g/ 255.0f, b/ 255.0f, a/ 255.0f);
    }

    int ColorNumber(char a, char b)
    {
        return ColorNumber(a) * 16 + ColorNumber(b);
    }

    int ColorNumber(char a)
    {
        if (a >= '0' && a <= '9')
        {
            return a-'0';
        }
        else if(a>='A' && a<='F')
        {
            return 10 + a - 'A';
        }
        else if(a>'a' && a<'f')
        {
            return 10 + a - 'a';
        }
        else
        {
            return 15;
        }
    }

}
