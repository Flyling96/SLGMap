using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexEdgeMesh : MonoBehaviour {

    Mesh hexEdgeMesh;
    List<Vector3> vertices;
    List<Color> colors;
    List<int> triangles;


    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexEdgeMesh = new Mesh();
        hexEdgeMesh.name = "Hex Edge Mesh";
        vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();
    }

    public void Clear()
    {
        hexEdgeMesh.Clear();
        vertices.Clear();
        colors.Clear();
        triangles.Clear();
    }

    public void Triangulate(HexCell cell,HexCell neighbor, HexDirection direction,bool isStep)
    {
        Clear();

        Vector3 center = cell.transform.localPosition;
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);   //形成面的内六边形顶点
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);
        v1.y += 2;
        v2.y += 2;

        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        v3.y = v4.y = neighbor.Elevation * HexMetrics.elevationStep+2;

        if (isStep)
        {
            TriangulateEdgeTerraces(v1, v2, cell, v3, v4, neighbor);
        }
        else
        {
            Color c = new Color(0.18f, 1, 0.18f, 0.5f);
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(c, c);
        }

        hexEdgeMesh.vertices = vertices.ToArray();
        hexEdgeMesh.colors = colors.ToArray();
        hexEdgeMesh.triangles = triangles.ToArray();
        hexEdgeMesh.RecalculateNormals();
    }


    //绘制阶梯
    void TriangulateEdgeTerraces(
    Vector3 beginLeft, Vector3 beginRight, HexCell beginCell,
    Vector3 endLeft, Vector3 endRight, HexCell endCell
)
    {
        Vector3 v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(beginRight, endRight, 1);
        Color c2 = new Color(0.18f, 1, 0.18f,0.5f);

        AddQuad(beginLeft, beginRight, v3, v4);
        AddQuadColor(c2, c2);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c2;
            v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, i);
            v4 = HexMetrics.TerraceLerp(beginRight, endRight, i);
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(c1, c2);
        }

        AddQuad(v3, v4, endLeft, endRight);
        AddQuadColor(c2, c2);
    }


    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }

    // 添加四边形四个点的颜色
    void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }
}
