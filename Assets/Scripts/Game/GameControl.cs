using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {

    //public HexGrid hexGrid;

    int distanceInOneRound = 3;

    List<HexCell> canGotoCellList = new List<HexCell>();
    List<HexCell> cantGotoNowList = new List<HexCell>();
    List<HexCell> allCellList = new List<HexCell>();
	// Use this for initialization
	void Start () {
        canGotoCellList.Clear();
        for(int i=0;i<HexGrid.instance.cells.Length;i++)
        {
            allCellList.Add(HexGrid.instance.cells[i]);
        }
        FindRoad.instance.Init(allCellList);
    }

	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            if (startCell == null)
            {
                ClickStartCell();
            }
            else if (endCell == null)
            {
                ClickEndCell();
            }
        }
        else if(Input.GetMouseButtonDown(1))
        {
            for(int i=0;i<canGotoCellList.Count;i++)
            {
                canGotoCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = false;
                canGotoCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FFFFFFFF");
            }
            for(int i=0;i<cantGotoNowList.Count;i++)
            {
                cantGotoNowList[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = false;
                cantGotoNowList[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FFFFFFFF");
            }
            canGotoCellList.Clear();
            cantGotoNowList.Clear();
            startCell = null;
            endCell = null;
        }

    }

    HexCell startCell = null;
    HexCell endCell = null;
    void ClickEndCell()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            endCell = HexGrid.instance.GetCell(hit.point);
            endCell.label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
            endCell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#0080FFFF");
        }
        //canGotoCellList.Add(endCell);
        //Debug.Log(canGotoCellList.IndexOf(endCell));
        List<HexCell> road = new List<HexCell>();
        road = FindRoad.instance.AStar(startCell, endCell, allCellList);

        for(int i=0;i<road.Count;i++)
        {
            if (canGotoCellList.Contains(road[i]))
            {
                road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00FF76FF");
            }
            else
            {
                road[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
                road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#0080FFFF");
                cantGotoNowList.Add(road[i]);
            }
        }
    }

    void ClickStartCell()
    {
        canGotoCellList.Clear();
        HexCell NeighborCell = null;
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int centerX;
        int centerZ;
        if (Physics.Raycast(inputRay, out hit))
        {
            startCell = HexGrid.instance.GetCell(hit.point);
            //hexGrid.DistanceToOther(startCell);

            centerX = startCell.coordinates.X;
            centerZ = startCell.coordinates.Z;
            for (int l = 0, z = centerZ; z >= centerZ - distanceInOneRound + 1; l++, z--)
            {
                for (int x = centerX - distanceInOneRound + 1 + l; x <= centerX + distanceInOneRound - 1; x++)
                {
                    if (HexGrid.instance.GetCell(new HexCoordinates(x, z)) != null)
                    {
                        NeighborCell = HexGrid.instance.GetCell(new HexCoordinates(x, z));
                        if (startCell.coordinates.DistanceToOther(NeighborCell.coordinates)<=distanceInOneRound)
                        {
                            NeighborCell.label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
                            canGotoCellList.Add(NeighborCell);
                        }
                    }
                }
            }

            for (int l = 1, z = centerZ + 1; z <= centerZ + distanceInOneRound - 1; l++, z++)
            {
                for (int x = centerX - distanceInOneRound + 1; x <= centerX + distanceInOneRound - 1 - l; x++)
                {
                    if (HexGrid.instance.GetCell(new HexCoordinates(x, z)) != null)
                    {
                        NeighborCell = HexGrid.instance.GetCell(new HexCoordinates(x, z));
                        if (startCell.coordinates.DistanceToOther(NeighborCell.coordinates) <= distanceInOneRound)
                        {
                            NeighborCell.label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
                            canGotoCellList.Add(NeighborCell);
                        }
                    }
                }
            }

            startCell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00FF76FF");

        }
    }
}
