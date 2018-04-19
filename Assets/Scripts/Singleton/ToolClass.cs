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

    //判断一个点是否在多边形内  射线交点
    public  bool IsInside(Vector2 checkPoint, List<Vector2> polygonPoints)
    {
        int count = 0;
        double shadow = 0 ;
        Vector2 p1, p2;
        int pointCount = polygonPoints.Count;
        p1 = polygonPoints[0];
        for (int i = 1; i <= pointCount; i++)
        {
            p2 = polygonPoints[i % pointCount];
            if (checkPoint.y > Math.Min(p1.y, p2.y)
                && checkPoint.y <= Math.Max(p1.y, p2.y))
            {
                if (checkPoint.x <= Math.Max(p1.x, p2.x))
                {
                    if (p1.y != p2.y)//线段不平行于x轴  
                    {
                        shadow = (checkPoint.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;
                        if (p1.x == p2.x || checkPoint.x <= shadow)
                        {
                            count++;
                        }
                    }
                }

            }
            p1 = p2;
        }
        if (count % 2 == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // 计算两点之间的距离      
    double PointDistance(double x1, double y1, double x2, double y2)
    {
       return  Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }
    //点到线段距离    
    public double PointToLine(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        double dis = 0;
        double a, b, c;
        a = PointDistance(p1.x, p1.y, p2.x, p2.y);// 线段的长度      
        b = PointDistance(p1.x, p1.y, p0.x, p0.y);// p1到p0的距离      
        c = PointDistance(p2.x, p2.y, p0.x, p0.y);// p2到p0的距离 
        
        //在一条直线上的情况
        if (c == a + b)
        {
            dis = b;
            return dis;
        }
        if (b  == a + c)
        {
            dis = c;
            return dis;
        }
        double p = (a + b + c) / 2;// 半周长      
        double s = Math.Sqrt(p * (p - a) * (p - b) * (p - c));// 海伦公式求面积      
        dis = 2 * s / a;// 返回点到线的距离（利用三角形面积公式求高）      
        return dis;
    }

}
