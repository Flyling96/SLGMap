using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EditorEdgeType
{
    Slope,
    Step,
}

public interface IEditorEdge
{
    void RefreshEdgeBrush();

}

public class TerrainBrush : IEditorEdge
{
    EditorType m_editorType = EditorType.HeightEditor;
    EditorEdgeType m_editorEdgeType = EditorEdgeType.Slope;
    public HexEdgeMesh hexEdgeMesh = null;
    int brushRange = 1;

    public TerrainBrush(EditorType type, EditorEdgeType edgeType = EditorEdgeType.Slope)
    {
        m_editorType = type;
        m_editorEdgeType = edgeType;
    }

    public void RefreshEdgeBrush()
    {

    }

    public void RefreshBrush()
    {
        switch (m_editorType)
        {
            case EditorType.HeightEditor:

                break;
            case EditorType.MaterialEditor:
                break;
            case EditorType.SceneObjEditor:
                break;
            case EditorType.WaterEditor:
                break;
        }
    }

    //刷新编辑指引的mesh
    void EditMeshRefresh(Vector3 pos, HexCell cell,bool isWholeEditor = false)
    {
        List<HexCell> cells = new List<HexCell>();
        HexCell cellChildren = null;
        int centerX = cell.coordinates.X;
        int centerZ = cell.coordinates.Z;
        for (int l = 0, z = centerZ; z >= centerZ - brushRange + 1; l++, z--)
        {
            for (int x = centerX - brushRange + 1 + l; x <= centerX + brushRange - 1; x++)
            {
                if (HexGrid.instance.GetCell(new HexCoordinates(x, z)) != null)
                {
                    cells.Add(HexGrid.instance.GetCell(new HexCoordinates(x, z)));
                }
            }
        }

        for (int l = 1, z = centerZ + 1; z <= centerZ + brushRange - 1; l++, z++)
        {
            for (int x = centerX - brushRange + 1; x <= centerX + brushRange - 1 - l; x++)
            {
                cells.Add(HexGrid.instance.GetCell(new HexCoordinates(x, z)));
            }
        }

        HexDirection clickDir = HexGrid.instance.GetPointDirection(new Vector2(pos.x - cell.transform.position.x, pos.z - cell.transform.position.z));
        if (m_editorEdgeType == EditorEdgeType.Step)
        {
            if (!isWholeEditor)
            {
                hexEdgeMesh.Clear();
                for (int i = 0; i < cells.Count; i++)
                {
                    cellChildren = cells[i];
                    if (cellChildren == null)
                    {
                        continue;
                    }
                    if (cellChildren.GetEdgeType(cellChildren.isStepDirection[(int)clickDir], clickDir) == HexEdgeType.Slope)
                    {
                        hexEdgeMesh.TriangulateItem(cellChildren, cellChildren.GetNeighbor(clickDir), clickDir, true, new Color(0.18f, 1, 0.18f, 0.5f));
                    }
                    else
                    {
                        hexEdgeMesh.TriangulateItem(cellChildren, cellChildren.GetNeighbor(clickDir), clickDir, true, new Color(1, 0.18f, 0.18f, 0.5f));
                    }
                }
                hexEdgeMesh.InputMeshInfo();
            }
            else
            {
                hexEdgeMesh.Triangulate(cells, new Color(0.18f, 1, 0.18f, 0.5f));
            }

        }
        else if (m_editorEdgeType == EditorEdgeType.Slope)
        {
            if (!isWholeEditor)
            {
                hexEdgeMesh.Clear();
                for (int i = 0; i < cells.Count; i++)
                {
                    cellChildren = cells[i];
                    if (cellChildren == null)
                    {
                        continue;
                    }
                    if (cellChildren.GetEdgeType(cellChildren.isStepDirection[(int)clickDir], clickDir) == HexEdgeType.Step)
                    {
                        hexEdgeMesh.TriangulateItem(cellChildren, cellChildren.GetNeighbor(clickDir), clickDir, false, new Color(0.18f, 1, 0.18f, 0.5f));
                    }
                    else
                    {
                        hexEdgeMesh.TriangulateItem(cellChildren, cellChildren.GetNeighbor(clickDir), clickDir, false, new Color(1, 0.18f, 0.18f, 0.5f));
                    }
                }
                hexEdgeMesh.InputMeshInfo();
            }
            else
            {
                hexEdgeMesh.Triangulate(cells, new Color(0.18f, 1, 0.18f, 0.5f));
            }
        }

    }
}