using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexMap : MonoBehaviour {

    [SerializeField]
    [HideInInspector]
    public HexCell[] cells;

    [SerializeField]
    public HexGridChunk[] chunks;


    public int chunkCountX = 0;
    public int chunkCountZ = 0;
    public int cellCountWidth = 0;
    public int cellCountHeight = 0;
    int width = 0;
    int height = 0;

    Canvas gridCanvas;

    public void SetMapSize(int x,int z)
    {
        chunkCountX = x;
        chunkCountZ = z;
    }

    //新建地图
    public void NewMap(HexCell cellPrefab,Text cellLabelPrefab, HexGridChunk gridChunkPerfab,bool isNewMaterial = false, Texture2D defaultTerrainTex = null, int chunkWidth = 0, int chunkHeight = 0)
    {
        if (!HexMetrics.instance.isEditor)
        {
            gridCanvas = (Instantiate(Resources.Load("Prefabs/UIPrefabs/Hex Grid Canvas") as GameObject)).GetComponent<Canvas>();
            gridCanvas.transform.SetParent(transform);
        }

        //chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        chunks = new HexGridChunk[chunkCountX * chunkCountZ];
        if (chunkWidth == 0 || chunkHeight == 0)
        {
            width = HexMetrics.instance.chunkWidth;
            height = HexMetrics.instance.chunkHeight;
        }
        else
        {
            width = chunkWidth;
            height = chunkHeight;
        }
        cellCountWidth = width * chunkCountX;
        cellCountHeight = height * chunkCountZ;

        cells = new HexCell[cellCountWidth * cellCountHeight];

        for (int j = 0, i = 0; j < chunkCountZ; j++)
        {
            for (int k = 0; k < chunkCountX; k++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(gridChunkPerfab) as HexGridChunk;
                chunk.CreateCells(chunkWidth, chunkHeight);
                //chunk.transform.SetParent(GameObject.Find("Map").transform);
                chunk.name = "HexChunk_" + k.ToString("000") + "_" + j.ToString("000");
                chunk.transform.SetParent(transform);
                chunk.transform.localPosition = new Vector3(chunk.transform.localPosition.x, 0, chunk.transform.localPosition.z);
                if(isNewMaterial)
                {
                    chunk.CreateMaterial(defaultTerrainTex);
                }
            }
        }


        for (int j = 0, i = 0; j < cellCountHeight; j++)
        {
            for (int k = 0; k < cellCountWidth; k++)
            {
                CreateCell(k, j, i++, cellPrefab, cellLabelPrefab);
            }
        }

        Refresh();

    }

    public void Refresh()
    {
        for (int j = 0, i = 0; j < chunkCountZ; j++)
        {
            for (int k = 0; k < chunkCountX; k++)
            {
                chunks[i++].Refresh();
            }

        }
        // hexMesh.Triangulate(cells);
    }

    void CreateCell(int x, int z, int i,HexCell cellPrefab, Text cellLabelPrefab)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.instance.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.instance.outerRadius * 1.5f);


        HexCell cell = cells[i] = Instantiate(cellPrefab) as HexCell;
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = HexMetrics.instance.defaultColor;


        //设置邻居
        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountWidth]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountWidth - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountWidth]);
                if (x < cellCountWidth - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountWidth + 1]);
                }
            }
        }

        if (!HexMetrics.instance.isEditor)
        {
            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition3D = new Vector3(position.x, position.z, cell.transform.position.y);
            cell.label = label;
            cell.uiRect = label.rectTransform;
        }

        cell.isStepDirection = new bool[] { false, false, false, false, false, false };
        CellToChunk(x, z, cell);
        cell.Elevation = 0;

    }

    void CellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / width;
        int chunkZ = z / height;

        HexGridChunk tmpChunk = chunks[chunkZ * chunkCountX + chunkX];

        int cellOfChunkX = x - x / width * width;
        int cellOfChunkZ = z - z / height * height;
        if (cellOfChunkZ * width + cellOfChunkX == 0)
        {
            tmpChunk.transform.position = cell.transform.position;
        }
        tmpChunk.AddCell(cellOfChunkZ * width + cellOfChunkX, cell);
        cell.SetParentChunk(tmpChunk);
    }
}
