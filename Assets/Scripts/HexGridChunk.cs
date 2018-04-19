using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridChunk : MonoBehaviour {

    int width = 5;
    int height = 5;

    public HexCell[] cells;

    public SceneObjectMgr sceneObjectMgr;

    public HexMesh waterMesh,waterEdgeMesh,terrainMesh;

    void OnEnable()
    {
        if (HexMetrics.instance != null)
        {
            width = HexMetrics.instance.chunkWidth;
            height = HexMetrics.instance.chunkHeight;
        }
        cells = new HexCell[width * height];
    }


    // Use this for initialization
    void Start()
    {
        terrainMesh.TrangulateByMeshClass(cells);
    }


    public void Clear()
    {
        waterMesh.Clear();
        waterEdgeMesh.Clear();
        terrainMesh.Clear();
        sceneObjectMgr.Clear();
    }

    public void Refresh()
    {
        waterMesh.TrangulateByMeshClass(cells);
        waterEdgeMesh.TrangulateByMeshClass(cells);
        terrainMesh.TrangulateByMeshClass(cells);
        sceneObjectMgr.Refresh();
    }

    //对mesh进行刷新
    public void Refresh(MeshClass meshClass)
    {
        switch (meshClass)
        {
            case MeshClass.terrainMesh:
                terrainMesh.TrangulateByMeshClass(cells);
                break;

            case MeshClass.waterMesh:
                waterMesh.TrangulateByMeshClass(cells);
                break;

            case MeshClass.waterEdgeMesh:
                waterEdgeMesh.TrangulateByMeshClass(cells);
                break;
        }
    }


    public void Refresh(HexMesh refreshMesh)
    {
        Refresh(refreshMesh.meshClass);
    }

    public void AddCell(int i,HexCell cell)
    {
        cells[i] = cell;
        cell.transform.SetParent(this.transform);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
