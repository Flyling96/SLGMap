using System.Collections;
using System.Collections.Generic;
using System;
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

    public  bool IsInside(Vector2 checkPoint, List<Vector2> polygonPoints)
    {
        int counter = 0;
        int i;
        double xinters;
        Vector2 p1, p2;
        int pointCount = polygonPoints.Count;
        p1 = polygonPoints[0];
        for (i = 1; i <= pointCount; i++)
        {
            p2 = polygonPoints[i % pointCount];
            if (checkPoint.y > Math.Min(p1.y, p2.y)//校验点的y大于线段端点的最小y  
                && checkPoint.y <= Math.Max(p1.y, p2.y))//校验点的y小于线段端点的最大y  
            {
                if (checkPoint.x <= Math.Max(p1.x, p2.x))//校验点的x小于等线段端点的最大x(使用校验点的左射线判断).  
                {
                    if (p1.y != p2.y)//线段不平行于x轴  
                    {
                        xinters = (checkPoint.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;
                        if (p1.x == p2.x || checkPoint.x <= xinters)
                        {
                            counter++;
                        }
                    }
                }

            }
            p1 = p2;
        }

        if (counter % 2 == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}
