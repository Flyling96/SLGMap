using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {

    public HexGrid hexGrid;

    int distanceInOneRound = 5;

    List<HexCell> showHightlightCellList = new List<HexCell>();
	// Use this for initialization
	void Start () {
        showHightlightCellList.Clear();
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
            for(int i=0;i<showHightlightCellList.Count;i++)
            {
                showHightlightCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = false;
            }
            showHightlightCellList.Clear();
            startCell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FFFFFFFF");
            endCell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FFFFFFFF");
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
            endCell = hexGrid.GetCell(hit.point);
            endCell.label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
            endCell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#0080FFFF");
        }
        showHightlightCellList.Add(endCell);
    }

    void ClickStartCell()
    {
        HexCell NeighborCell = null;
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int centerX;
        int centerZ;
        if (Physics.Raycast(inputRay, out hit))
        {
            startCell = hexGrid.GetCell(hit.point);
            hexGrid.DistanceToOther(startCell);

            centerX = startCell.coordinates.X;
            centerZ = startCell.coordinates.Z;
            for (int l = 0, z = centerZ; z >= centerZ - distanceInOneRound + 1; l++, z--)
            {
                for (int x = centerX - distanceInOneRound + 1 + l; x <= centerX + distanceInOneRound - 1; x++)
                {
                    if (hexGrid.GetCell(new HexCoordinates(x, z)) != null)
                    {
                        NeighborCell = hexGrid.GetCell(new HexCoordinates(x, z));
                        if (startCell.coordinates.DistanceToOther(NeighborCell.coordinates)<=distanceInOneRound)
                        {
                            NeighborCell.label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
                            showHightlightCellList.Add(NeighborCell);
                        }
                    }
                }
            }

            for (int l = 1, z = centerZ + 1; z <= centerZ + distanceInOneRound - 1; l++, z++)
            {
                for (int x = centerX - distanceInOneRound + 1; x <= centerX + distanceInOneRound - 1 - l; x++)
                {
                    if (hexGrid.GetCell(new HexCoordinates(x, z)) != null)
                    {
                        NeighborCell = hexGrid.GetCell(new HexCoordinates(x, z));
                        if (startCell.coordinates.DistanceToOther(NeighborCell.coordinates) <= distanceInOneRound)
                        {
                            NeighborCell.label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
                            showHightlightCellList.Add(NeighborCell);
                        }
                    }
                }
            }

            startCell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00FF76FF");

        }
    }
}
