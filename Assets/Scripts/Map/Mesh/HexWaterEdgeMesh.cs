using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexWaterEdgeMesh : HexMesh
{

    private void Awake()
    {
        meshClass = MeshClass.waterMesh;
        Init();
    }
    private void Start()
    {
        if(hexMesh!=null)
        hexMesh.name = "Hex Water Edge Mesh";
    }

    public override void TrangulateByMeshClass(HexCell[] cells)
    {
        base.TrangulateByMeshClass(cells);

        Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].isUnderWaterLevel)
            {
                TrangulateWaterEdge(cells[i]);
                cells[i].chunkParent.sceneObjectMgr.MinusSceneObject(cells[i]);
            }
        }
        Draw();
    }

    void TrangulateWaterEdge(HexCell cell)
    {
        Vector3 center = cell.LocalPosition;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = cell.GetNeighbor(d);
            if (neighbor != null && !neighbor.isUnderWaterLevel)
            {
                TrangulateWaterEdge(d, cell, neighbor, center);
            }
        }
    }
    void TrangulateWaterEdge(HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center)
    {
        center.y = cell.waterY;
        EdgeVertices e1 = new EdgeVertices(
            center + HexMetrics.instance.GetFirstSolidCorner(direction),
            center + HexMetrics.instance.GetSecondSolidCorner(direction)
        );
        Vector3 bridge = HexMetrics.instance.GetBridge(direction);
        EdgeVertices e2 = new EdgeVertices(
            e1.v1 + bridge,
            e1.v4 + bridge
        );
        AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        AddQuadUV(0f, 0f, 0f, 1f);
        AddQuadUV(0f, 0f, 0f, 1f);
        AddQuadUV(0f, 0f, 0f, 1f);

        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (nextNeighbor != null)
        {
            AddTriangle(
                e1.v4, e2.v4, e1.v4 + HexMetrics.instance.GetBridge(direction.Next())
            );
            AddTriangleUV(
                new Vector2(0f, 0f),
                new Vector2(0f, 1f),
                new Vector2(0f, nextNeighbor.isUnderWaterLevel ? 0f : 1f)
            );
        }
    }
}

