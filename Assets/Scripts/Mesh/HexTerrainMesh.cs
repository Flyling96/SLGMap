using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTerrainMesh : HexMesh {


    private void Awake()
    {
        meshClass = MeshClass.terrainMesh;
    }
    private void Start()
    {
        hexMesh.name = "Hex Terrain Mesh";
    }

    public override void TrangulateByMeshClass(HexCell[] cells)
    {
        Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        Draw();
    }
}
