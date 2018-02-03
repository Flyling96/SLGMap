using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridChunk : MonoBehaviour {

    int width = 6;
    int height = 6;

    public HexCell[] cells;
    HexMesh hexMesh;

    void Awake()
    {
        width = HexMetrics.chunkWidth;
        height = HexMetrics.chunkHeight;
        cells = new HexCell[width * height];
        hexMesh = GetComponentInChildren<HexMesh>();
    }


    // Use this for initialization
    void Start ()
    {
        hexMesh.Triangulate(cells);
    }

    public void Refresh()
    {
        hexMesh.Triangulate(cells);
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
