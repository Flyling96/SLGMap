using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TerrainTypes
{
    Grass,
    Mud,
    Snow,
    Sand,
    Stone
}

public class HexTerrainMesh : HexMesh {

    private void Awake()
    {
        meshClass = MeshClass.terrainMesh;
        Init();
    }
    private void Start()
    {
        if (hexMesh != null)
        {
            hexMesh.name = "Hex Terrain Mesh";
        }
    }

    public override void TrangulateByMeshClass(HexCell[] cells)
    {
        base.TrangulateByMeshClass(cells);

        Clear();
        if (!isUseMap)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Triangulate(cells[i]);
            }
        }
        else
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Triangulate(cells[i], isUseMap);
            }
        }
        Draw();
    }


    //绘制六边形
    public void Triangulate(HexCell cell,bool useMap)
    {
        if (!useMap)
            return;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, cell, isUseMap);
        }
    }

    void Triangulate(HexDirection direction, HexCell cell, bool useMap)
    {
        if (!useMap)
            return;

        if (cell == null)
        {
            return;
        }
        Vector3 center = cell.LocalPosition;
        //Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);   //形成面的内六边形顶点
        //Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

        EdgeVertices e = new EdgeVertices(
            center + HexMetrics.instance.GetFirstSolidCorner(direction),
            center + HexMetrics.instance.GetSecondSolidCorner(direction)
        );

        TriangulateEdgeFan(center, e, (float)cell.TerrainTypeIndex);

        if (direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, e);
        }
        else
        {
            SetBorderEdge(e);
        }
    }


    /// <summary>
    /// 绘制外梯形
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="cell"></param>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
	 void TriangulateConnection(
        HexDirection direction, HexCell cell, EdgeVertices e1
    )
    {
        HexCell neighbor = cell.GetNeighbor(direction);
        if (neighbor == null)
        {
            return;
        }

        Vector3 bridge = HexMetrics.instance.GetBridge(direction);
        bridge.y = neighbor.LocalPosition.y - cell.LocalPosition.y;
        EdgeVertices e2 = new EdgeVertices(e1.v1 + bridge, e1.v4 + bridge);
        SetBorderEdge(e2);

        //绘制阶梯
        if (cell.GetEdgeType(cell.isStepDirection[(int)direction], direction) == HexEdgeType.Step)
        {
            TriangulateEdgeTerraces(e1, cell, e2, neighbor);
        }
        //绘制平面或者陡坡
        else
        {
            TriangulateEdgeStrip(e1, cell.color, (float)cell.TerrainTypeIndex, e2, neighbor.color, (float)neighbor.TerrainTypeIndex);
        }

        //绘制中间三角
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (direction <= HexDirection.E && nextNeighbor != null)
        {
            Vector3 v5 = e1.v4 + HexMetrics.instance.GetBridge(direction.Next());//获取桥对面的点
            v5.y = nextNeighbor.LocalPosition.y;//设置桥对面的点的y

            if (cell.Elevation <= neighbor.Elevation)
            {
                if (cell.Elevation <= nextNeighbor.Elevation)
                {
                    TriangulateCorner(e1.v4, cell, e2.v4, neighbor, v5, nextNeighbor);
                }
                else
                {
                    TriangulateCorner(v5, nextNeighbor, e1.v4, cell, e2.v4, neighbor);
                }
            }
            else if (neighbor.Elevation <= nextNeighbor.Elevation)
            {
                TriangulateCorner(e2.v4, neighbor, v5, nextNeighbor, e1.v4, cell);
            }
            else
            {
                TriangulateCorner(v5, nextNeighbor, e1.v4, cell, e2.v4, neighbor);
            }
        }
    }


    public void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, float type)
    {
        AddTriangle(center, edge.v1, edge.v2);
        AddTriangle(center, edge.v2, edge.v3);
        AddTriangle(center, edge.v3, edge.v4);

        AddTriangleColor(HexMetrics.instance.splatColorR);
        AddTriangleColor(HexMetrics.instance.splatColorR);
        AddTriangleColor(HexMetrics.instance.splatColorR);
        Vector3 types;
        types.x = types.y = types.z = type;
        AddTriangleTerrain(types);
        AddTriangleTerrain(types);
        AddTriangleTerrain(types);
    }

    //不规则边的外梯形绘制
    protected void TriangulateEdgeStrip(
    EdgeVertices e1, Color c1, float type1,
    EdgeVertices e2, Color c2, float type2
)
    {
        AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);

        AddQuadColor(HexMetrics.instance.splatColorR, HexMetrics.instance.splatColorG);
        AddQuadColor(HexMetrics.instance.splatColorR, HexMetrics.instance.splatColorG);
        AddQuadColor(HexMetrics.instance.splatColorR, HexMetrics.instance.splatColorG);

        Vector3 types;
        types.x = types.z = type1;
        types.y = type2;
        AddQuadTerrain(types);
        AddQuadTerrain(types);
        AddQuadTerrain(types);

    }


    void TriangulateCorner(
    Vector3 bottom, HexCell bottomCell,
    Vector3 left, HexCell leftCell,
    Vector3 right, HexCell rightCell
)
    {
        HexEdgeType leftEdgeType = bottomCell.GetEdgeType(bottomCell.isStepDirection[(int)HexCoordinates.GetDirection(bottomCell, leftCell)], leftCell);
        HexEdgeType rightEdgeType = bottomCell.GetEdgeType(bottomCell.isStepDirection[(int)HexCoordinates.GetDirection(bottomCell, rightCell)], rightCell);

        //left-right-other
        if (leftEdgeType == HexEdgeType.Step)
        {
            //SSF 
            if (rightEdgeType == HexEdgeType.Step)
            {
                TriangulateCornerTerraces(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
            //SFS
            else if (rightEdgeType == HexEdgeType.Flat && leftCell.GetEdgeType(leftCell.isStepDirection[(int)HexCoordinates.GetDirection(leftCell, rightCell)], rightCell) == HexEdgeType.Step)
            {
                TriangulateCornerTerraces(
                    left, leftCell, right, rightCell, bottom, bottomCell
                );
            }
            //SCS SSC SCC 
            else
            {
                TriangulateCornerTerracesCliff(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
        }
        else if (rightEdgeType == HexEdgeType.Step)
        {
            //FSS
            if (leftEdgeType == HexEdgeType.Flat && leftCell.GetEdgeType(leftCell.isStepDirection[(int)HexCoordinates.GetDirection(leftCell, rightCell)], rightCell) == HexEdgeType.Step)
            {
                TriangulateCornerTerraces(
                    right, rightCell, bottom, bottomCell, left, leftCell
                );
            }
            //CSC CSS
            else
            {
                TriangulateCornerCliffTerraces(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
        }
        else if (leftCell.GetEdgeType(leftCell.isStepDirection[(int)HexCoordinates.GetDirection(leftCell, rightCell)], rightCell) == HexEdgeType.Step)
        {
            //CCS 
            if (leftCell.Elevation < rightCell.Elevation)
            {
                TriangulateCornerCliffTerraces(
                    right, rightCell, bottom, bottomCell, left, leftCell
                );
            }
            else
            {
                TriangulateCornerTerracesCliff(
                    left, leftCell, right, rightCell, bottom, bottomCell
                );
            }
        }
        //CCF CCC FCC CFC 
        else
        {
            AddTriangle(bottom, left, right);
            AddTriangleColor(HexMetrics.instance.splatColorR, HexMetrics.instance.splatColorG, HexMetrics.instance.splatColorB);
            Vector3 types;
            types.x = (float)bottomCell.TerrainTypeIndex;
            types.y = (float)leftCell.TerrainTypeIndex;
            types.z = (float)rightCell.TerrainTypeIndex;
            AddTriangleTerrain(types);
        }
    }

    //绘制阶梯
    void TriangulateEdgeTerraces(
        EdgeVertices begin, HexCell beginCell,
        EdgeVertices end, HexCell endCell
    )
    {
        EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
        Color c2 = HexMetrics.instance.TerraceLerp(beginCell.color, endCell.color, 1);

        TriangulateEdgeStrip(begin, beginCell.color, (float)beginCell.TerrainTypeIndex, e2, c2, (float)endCell.TerrainTypeIndex);

        for (int i = 2; i < HexMetrics.instance.terraceSteps; i++)
        {
            EdgeVertices e1 = e2;
            Color c1 = c2;
            e2 = EdgeVertices.TerraceLerp(begin, end, i);
            c2 = HexMetrics.instance.TerraceLerp(beginCell.color, endCell.color, i);
            TriangulateEdgeStrip(e1, c1, (float)beginCell.TerrainTypeIndex, e2, c2, (float)endCell.TerrainTypeIndex);
        }

        TriangulateEdgeStrip(e2, c2, (float)beginCell.TerrainTypeIndex, end, endCell.color, (float)endCell.TerrainTypeIndex);
    }

    //有出现平面的情况
    void TriangulateCornerTerraces(
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    )
    {
        Vector3 v3 = HexMetrics.instance.TerraceLerp(begin, left, 1);
        Vector3 v4 = HexMetrics.instance.TerraceLerp(begin, right, 1);

        AddTriangle(begin, v3, v4);
        AddTriangleColor(HexMetrics.instance.splatColorR, HexMetrics.instance.splatColorG, HexMetrics.instance.splatColorB);

        Vector3 types = new Vector3();
        types.x = (float)beginCell.TerrainTypeIndex;
        types.y = (float)leftCell.TerrainTypeIndex;
        types.z = (float)rightCell.TerrainTypeIndex;

        AddTriangleTerrain(types);

        for (int i = 2; i < HexMetrics.instance.terraceSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            v3 = HexMetrics.instance.TerraceLerp(begin, left, i);
            v4 = HexMetrics.instance.TerraceLerp(begin, right, i);
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(HexMetrics.instance.splatColorR, HexMetrics.instance.splatColorG);
            AddQuadTerrain(types);
        }

        AddQuad(v3, v4, left, right);
        AddQuadTerrain(types);
        AddQuadColor(HexMetrics.instance.splatColorR, HexMetrics.instance.splatColorG);
    }

    //有出现陡坡的情况
    //left为s,right为c
    void TriangulateCornerTerracesCliff(
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    )
    {
        float b = Mathf.Abs(1f / (rightCell.Elevation - beginCell.Elevation));
        Vector3 boundary = Vector3.Lerp(Perturb(begin), Perturb(right), b);
        Color boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);

        Vector3 types = new Vector3();
        types.x = (float)beginCell.TerrainTypeIndex;
        types.y = (float)leftCell.TerrainTypeIndex;
        types.z = (float)rightCell.TerrainTypeIndex;

        TriangulateBoundaryTriangle(
            begin, beginCell, left, leftCell, boundary, boundaryColor, types
        );

        //判断left和right之间的类型关系
        if (leftCell.GetEdgeType(leftCell.isStepDirection[(int)HexCoordinates.GetDirection(leftCell, rightCell)], rightCell) == HexEdgeType.Step)
        {
            TriangulateBoundaryTriangle(
                left, leftCell, right, rightCell, boundary, boundaryColor, types
            );
        }
        else
        {
            AddTriangleUnPerturb(Perturb(left), Perturb(right), boundary);
            AddTriangleColor(HexMetrics.instance.splatColorR, HexMetrics.instance.splatColorG, HexMetrics.instance.splatColorB);
            AddTriangleTerrain(types);
        }
    }
 

    //left为c，right为s 与上一种情况对称
    void TriangulateCornerCliffTerraces(
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    )
    {
        float b = Mathf.Abs(1f / (leftCell.Elevation - beginCell.Elevation));
        Vector3 boundary = Vector3.Lerp(Perturb(begin), Perturb(left), b);
        Color boundaryColor = Color.Lerp(beginCell.color, leftCell.color, b);

        Vector3 types = new Vector3();
        types.x = (float)beginCell.TerrainTypeIndex;
        types.y = (float)leftCell.TerrainTypeIndex;
        types.z = (float)rightCell.TerrainTypeIndex;

        TriangulateBoundaryTriangle(
            right, rightCell, begin, beginCell, boundary, boundaryColor, types
        );

        //判断left和right之间的类型关系
        if (leftCell.GetEdgeType(leftCell.isStepDirection[(int)HexCoordinates.GetDirection(leftCell, rightCell)], rightCell) == HexEdgeType.Step)
        {
            TriangulateBoundaryTriangle(
                left, leftCell, right, rightCell, boundary, boundaryColor, types
            );
        }
        else
        {
            AddTriangleUnPerturb(Perturb(left), Perturb(right), boundary);
            AddTriangleColor(HexMetrics.instance.splatColorR, HexMetrics.instance.splatColorG, HexMetrics.instance.splatColorB);
            AddTriangleTerrain(types);
        }
    }

    void TriangulateBoundaryTriangle(
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 boundary, Color boundaryColor, Vector3 types
    )
    {
        Vector3 v2 = Perturb(HexMetrics.instance.TerraceLerp(begin, left, 1));

        AddTriangleUnPerturb(Perturb(begin), v2, boundary);
        AddTriangleColor(HexMetrics.instance.splatColorR, HexMetrics.instance.splatColorG, HexMetrics.instance.splatColorB);
        AddTriangleTerrain(types);

        for (int i = 2; i < HexMetrics.instance.terraceSteps; i++)
        {
            Vector3 v1 = v2;
            v2 = Perturb(HexMetrics.instance.TerraceLerp(begin, left, i));
            AddTriangleUnPerturb(v1, v2, boundary);
            AddTriangleColor(HexMetrics.instance.splatColorR, HexMetrics.instance.splatColorG, HexMetrics.instance.splatColorB);
            AddTriangleTerrain(types);
        }

        AddTriangleUnPerturb(v2, Perturb(left), boundary);
        AddTriangleColor(HexMetrics.instance.splatColorR, HexMetrics.instance.splatColorG, HexMetrics.instance.splatColorB);
        AddTriangleTerrain(types);
    }


}
