using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexWaterMesh : HexMesh {

    private void Awake()
    {
        meshClass = MeshClass.waterEdgeMesh;
    }
    private void Start()
    {
        hexMesh.name = "Hex Water Mesh";
    }

    public override void TrangulateByMeshClass(HexCell[] cells)
    {
        base.TrangulateByMeshClass(cells);

        Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            if(cells[i].isUnderWaterLevel)
            {
                TrangulateWater(cells[i]);
            }
        }
        Draw();
    }


    void TrangulateWater(HexCell cell)
    {
        Vector3 center = cell.Position;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            TrangulateWater(d, cell, center);
        }
    }
    void TrangulateWater(HexDirection direction, HexCell cell,Vector3 center)
    {
        center.y = cell.waterY;
        Vector3 v1 = center + HexMetrics.instance.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.instance.GetSecondSolidCorner(direction);


        HexCell neighbor = cell.GetNeighbor(direction);
        if (neighbor != null && !neighbor.isUnderWaterLevel)
        {
            TrangulateWaterNearEdge(direction, cell, neighbor, center);
        }
        else
        {
            AddTriangle(center, v1, v2);
        }


        if (direction <= HexDirection.SE)
        {
            if (neighbor == null || !neighbor.isUnderWaterLevel)
            {
                return;
            }
            Vector3 bridge = HexMetrics.instance.GetBridge(direction);
            Vector3 v3 = v1 + bridge;
            Vector3 v4 = v2 + bridge;
            AddQuad(v1, v2, v3, v4);
            HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
            if (direction <= HexDirection.E && nextNeighbor != null && nextNeighbor.isUnderWaterLevel)
            {
                AddTriangle(v2, v4, v2 + HexMetrics.instance.GetBridge(direction.Next()));
            }
        }
    }

    void TrangulateWaterNearEdge(HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center)
    {
        EdgeVertices e1 = new EdgeVertices(
            center + HexMetrics.instance.GetFirstSolidCorner(direction),
            center + HexMetrics.instance.GetSecondSolidCorner(direction)
        );
        AddTriangle(center, e1.v1, e1.v2);
        AddTriangle(center, e1.v2, e1.v3);
        AddTriangle(center, e1.v3, e1.v4);
    }
}
