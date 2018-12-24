using UnityEngine;

public struct EdgeVertices
{

    public Vector3 v1, v2, v3, v4;

    public EdgeVertices(Vector3 corner1, Vector3 corner2)
    {
        v1 = corner1;
        v2 = Vector3.Lerp(corner1, corner2, 1f / 3f);
        v3 = Vector3.Lerp(corner1, corner2, 2f / 3f);
        v4 = corner2;
    }

    public static EdgeVertices TerraceLerp(
    EdgeVertices a, EdgeVertices b, int step)
    {
        EdgeVertices result;
        result.v1 = HexMetrics.instance.TerraceLerp(a.v1, b.v1, step);
        result.v2 = HexMetrics.instance.TerraceLerp(a.v2, b.v2, step);
        result.v3 = HexMetrics.instance.TerraceLerp(a.v3, b.v3, step);
        result.v4 = HexMetrics.instance.TerraceLerp(a.v4, b.v4, step);
        return result;
    }

    public bool isSame(EdgeVertices e)
    {
        //由于点位置存在精度问题，所以用距离来判断是否共线
        if(Vector3.Distance(v1, e.v1) < 0.1 && Vector3.Distance(v4, e.v4) < 0.1||
           Vector3.Distance(v1, e.v4) < 0.1 && Vector3.Distance(v4, e.v1) < 0.1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isEmpty()
    {
        if(v1 == new Vector3() && v2 == new Vector3() && v3 == new Vector3() && v4 == new Vector3())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}