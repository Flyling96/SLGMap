using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexRoadMesh : HexMesh {

    private void Awake()
    {
        meshClass = MeshClass.roadMesh;
    }
    private void Start()
    {
        hexMesh.name = "Hex Road Mesh";
    }

    public override void TrangulateByMeshClass(HexCell[] cells)
    {
        Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].isRoad)
            {
               TrangulateRoad(cells[i]);
            }
        }
        Draw();
    }

    void TrangulateRoad(HexCell cell)
    {
        Vector3 center = cell.Position;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            if(cell.IsDrawPreviousRoadMesh(d))
            {
                TrangulatePreviousRoad(d, cell, center);
            }
            if(cell.IsDrawNextRoadMesh(d))
            {
                TrangulateNextRoad(d, cell, center);
            }
        }
    }

    //补充边缘
    void TrangulatePreviousRoad(HexDirection direction, HexCell cell, Vector3 center)
    {
        HexCell previous = cell.GetNeighbor(direction.Previous());
        center.y = cell.waterY;
    }

    void TrangulateNextRoad(HexDirection direction, HexCell cell, Vector3 center)
    {

    }

    //绘制六边形
    public void Triangulate(HexCell cell)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, cell);
        }
    }

    void Triangulate(HexDirection direction, HexCell cell)
    {
        if (cell == null)
        {
            return;
        }
        Vector3 center = cell.Position;
        //Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);   //形成面的内六边形顶点
        //Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

        EdgeVertices e = new EdgeVertices(
            center + HexMetrics.instance.GetFirstSolidCorner(direction),
            center + HexMetrics.instance.GetSecondSolidCorner(direction)
        );

        TriangulateEdgeUnPerturb(center, e, cell.color);

        if (direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, e);
        }
    }
}
