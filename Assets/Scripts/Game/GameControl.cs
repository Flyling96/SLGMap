using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitActionEnum
{
    Move,
    Attack,
}


public class GameControl : MonoBehaviour {

    //public HexGrid hexGrid;

    int distanceInOneRound = 3;

    List<HexCell> canGotoCellList = new List<HexCell>();

    public bool isMyRound = true;

    // Use this for initialization
    void Start () {
        canGotoCellList.Clear();
    }

	// Update is called once per frame
	void Update () {
        if (isMyRound)
        {
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
            else if (Input.GetMouseButtonDown(1))
            {
                if (isClickEndCell)
                {
                    isClickEndCell = false;
                    endCell = null;
                    UIManage.instance.HideActionWnd();
                }
                else
                {
                    ExitFindRoad();
                }
            }
        }

    }

    void ExitFindRoad()
    {
        if (battleUnit != null)
        {
            GameUnitManage.instance.BlockRoad(battleUnit.power);
        }
        if (startCell != null)
        {
            for (int i = 0; i < canGotoCellList.Count; i++)
            {
                canGotoCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = false;
                canGotoCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FFFFFFFF");
            }
            canGotoCellList.Clear();
            startCell = null;
            endCell = null;
        }
    }

    HexCell startCell = null;
    HexCell endCell = null;
    List<HexCell> road = new List<HexCell>();
    bool isClickEndCell = false;


    void ClickEndCell()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            endCell = HexGrid.instance.GetCell(hit.point);
            Vector3 position;
            isClickEndCell = true;
            List<string> buttonNames = new List<string>();
            road = FindRoad.instance.AStar(startCell, endCell, HexGrid.instance.AllCellList);

            if (!FindRoad.instance.isCanGoNeighbor(startCell, endCell, battleUnit.battleUnitProperty.actionPower) && road.Count == 2)
            {
                UIManage.instance.ShowTipLine("该地点无法到达", 3);
                endCell = null;
                return;
            }

            if (endCell.unit!=null)
            {
                position = transform.Find("Main Camera").GetComponent<Camera>().WorldToScreenPoint(endCell.unit.transform.position);
                buttonNames.Add("移动");
                buttonNames.Add("攻击");
            }
            else
            {
                position = transform.Find("Main Camera").GetComponent<Camera>().WorldToScreenPoint(endCell.transform.position);
                buttonNames.Add("移动");
            }
            UIManage.instance.ShowActionWnd(UnitAction, buttonNames);
            UIManage.instance.actionWnd.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(position.x + UIManage.instance.actionWnd.GetComponent<RectTransform>().rect.width / 2,
                position.y - UIManage.instance.actionWnd.GetComponent<RectTransform>().rect.height / 2);

           
        }
        if (endCell != null)
        {
           
            endCell.label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
            endCell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#0080FFFF");

            List<HexCell> nowCanGoList = FindRoad.instance.FindCanGoList(startCell, HexGrid.instance.AllCellList, distanceInOneRound);
            for (int i = canGotoCellList.Count - 1; i >= 0; i--)
            {
                canGotoCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FFFFFFFF");
                if (!nowCanGoList.Contains(canGotoCellList[i]))
                {
                    canGotoCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = false;
                    canGotoCellList.RemoveAt(i);
                }
            }


            for (int i = 0; i < road.Count; i++)
            {
                if (canGotoCellList.Contains(road[i]))
                {
                    road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00FF76FF");
                }
                else
                {
                    road[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
                    road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#0080FFFF");
                    canGotoCellList.Add(road[i]);
                }
            }
        }
        //canGotoCellList.Add(endCell);
        //Debug.Log(canGotoCellList.IndexOf(endCell));


    }

    BattleUnit battleUnit = null;
    void ClickStartCell()
    {
        canGotoCellList.Clear();
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            startCell = HexGrid.instance.GetCell(hit.point);

            if (startCell.unit != null)
            {
                battleUnit = startCell.unit;
                GameUnitManage.instance.UnBlockRoad(battleUnit.power);
                //如果startCell现在没有还未行径的路径
                if (startCell.unit.isMoveComplete)
                {
                    distanceInOneRound = startCell.unit.battleUnitProperty.actionPower;
                    canGotoCellList = FindRoad.instance.FindCanGoList(startCell, HexGrid.instance.AllCellList, distanceInOneRound);
                    for (int i = 0; i < canGotoCellList.Count; i++)
                    {
                        canGotoCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
                    }
                    startCell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00FF76FF");
                }
                else
                {
                    startCell.unit.ShowRoad(ref canGotoCellList);
                }
            }

        }
    }

    //单位行动相关
    public void UnitAction(int index)
    {
        switch(index)
        {
            case (int)UnitActionEnum.Move:
                {
                    UnitMove();
                    Debug.Log(0);
                }
                break;
            case (int)UnitActionEnum.Attack:
                {
                    Debug.Log(1);
                }
                break;
        }
        isClickEndCell = false;
        UIManage.instance.HideActionWnd();
        ExitFindRoad();
    }

    void UnitMove()
    {

        if (startCell.unit != null)
        {
            battleUnit.SetRoad(road);
            battleUnit.MoveInRound();
        }
    }


}
