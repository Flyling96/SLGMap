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

    public static Camera mainCamera = null;

    List<HexCell> canDrawCellList = new List<HexCell>();

    public bool isMyRound = true;

    private void Awake()
    {
        mainCamera = transform.Find("Main Camera").GetComponent<Camera>();
    }
    // Use this for initialization
    void Start () {
        canDrawCellList.Clear();
    }

	// Update is called once per frame
	void Update () {

        isMyRound = GameUnitManage.instance.myPower == RoundManage.instance.curPower;

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


            if (startCell != null && endCell != null)
            {
                if (startCell.unit != null && endCell.unit != null)
                {
                    switch (UIManage.instance.actionType)
                    {
                        case UnitActionEnum.Attack:
                            {
                                ShowAttackRoad();
                                break;
                            }
                        case UnitActionEnum.Move:
                            {
                                ShowMoveRoad();
                                break;
                            }
                    }
                }
            }
        }



    }

    void ExitFindRoad()
    {
        if (startCell.unit != null)
        {
            GameUnitManage.instance.BlockRoad(startCell.unit.power);
        }
        if (startCell != null)
        {
            for (int i = 0; i < canDrawCellList.Count; i++)
            {
                canDrawCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = false;
                canDrawCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FFFFFFFF");
            }
            for(int i=0;i<canAttackUnit.Count;i++)
            {
                canAttackUnit[i].Cell.label.transform.Find("Hightlight").GetComponent<Image>().enabled = false;
                canAttackUnit[i].Cell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FFFFFFFF");
            }
            canDrawCellList.Clear();
            startCell = null;
            endCell = null;
            move = null;
            attack = null;
        }
    }

    HexCell startCell = null;
    HexCell endCell = null;
    List<HexCell> road = new List<HexCell>();
    bool isClickEndCell = false;


    void ShowMoveRoad()
    {
        if (endCell != null)
        {
            List<HexCell> nowCanGoList = FindRoad.instance.FindCanGoList(startCell, HexGrid.instance.AllCellList, distanceInOneRound);
            for (int i = canDrawCellList.Count - 1; i >= 0; i--)
            {
                canDrawCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FFFFFFFF");
                if (!nowCanGoList.Contains(canDrawCellList[i]))
                {
                    canDrawCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = false;
                    canDrawCellList.RemoveAt(i);
                }
            }

            for (int i = 0; i < road.Count; i++)
            {
                if (canDrawCellList.Contains(road[i]))
                {
                    road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00FF76FF");
                }
                else
                {
                    road[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
                    road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#0080FFFF");
                    canDrawCellList.Add(road[i]);
                }
            }

            if (!canDrawCellList.Contains(endCell))
            {
                endCell.label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
                endCell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FF4040FF");
                canDrawCellList.Add(endCell);
            }

        }
    }

    void ShowAttackRoad()
    {
        if (endCell != null)
        {
            int moveCount = attack.NeedMoveCount(endCell.unit);
            for (int i = 0; i < road.Count; i++)
            {
                if (canDrawCellList.Contains(road[i]))
                {
                    if (i < moveCount)
                    {
                        road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00FF76FF");
                    }
                    else
                    {
                        road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FF4040FF");
                    }
                }
                else
                {
                    road[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
                    if (i < moveCount)
                    {
                        road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00FF76FF");
                    }
                    else
                    {
                        road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FF4040FF");
                    }
                    canDrawCellList.Add(road[i]);
                }
            }
        }
    }

    void ShowAttackeUnit()
    {
        for (int i = 0; i < canAttackUnit.Count; i++)
        {
            canAttackUnit[i].Cell.label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
            canAttackUnit[i].Cell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FF4040FF");
        }
    }


    //注册单位行动信息
    void RegisterAction()
    {
        Dictionary<int, string> buttonNames = new Dictionary<int, string>();
        Vector3 position;
        if (endCell.unit != null)
        {
            position = mainCamera.WorldToScreenPoint(endCell.unit.transform.position);
            if (endCell.unit.power != startCell.unit.power)
            {
                buttonNames.Add(0, "Move");
                buttonNames.Add(1, "Attack");
            }
            else
            {
                buttonNames.Add(0, "Move");
            }
        }
        else
        {
            position = mainCamera.WorldToScreenPoint(endCell.transform.position);
            buttonNames.Add(0, "Move");
        }
        UIManage.instance.ShowActionWnd(UnitAction, buttonNames);
        UIManage.instance.actionWnd.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(position.x + UIManage.instance.actionWnd.GetComponent<RectTransform>().rect.width / 2,
            position.y - UIManage.instance.actionWnd.GetComponent<RectTransform>().rect.height / 2);
    }

    void ClickEndCell()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            endCell = HexGrid.instance.GetCell(hit.point);
            isClickEndCell = true;

            //获取
            if(endCell.unit!=null)
            {
                if(endCell.unit.power != startCell.unit.power)
                {
                    FindRoad.instance.UnBlockRoad(endCell,endCell.unit.power);
                }
            }

            road = FindRoad.instance.AStar(startCell, endCell, HexGrid.instance.AllCellList);

            if (endCell.unit != null)
            {
                if (endCell.unit.power != startCell.unit.power)
                {
                    FindRoad.instance.BlockRoad(endCell);
                }
            }

            if (road[road.Count-1]!=endCell)
            {
                UIManage.instance.ShowTipLine("该地点无法到达", 3);
            }

            RegisterAction();
            ShowMoveRoad();
        }

      
        //canDrawCellList.Add(endCell);
        //Debug.Log(canDrawCellList.IndexOf(endCell));


    }


    IMove move;
    IAttack attack;
    BattleUnit battleUnit;
    List<BattleUnit> canAttackUnit = new List<BattleUnit>();
    void ClickStartCell()
    {
        canAttackUnit.Clear();
        canDrawCellList.Clear();
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            startCell = HexGrid.instance.GetCell(hit.point);
            if(startCell==null)
            {
                return;
            }

            if (startCell.unit != null)
            {
                if(startCell.unit.isMove||startCell.unit.power!=GameUnitManage.instance.myPower)
                {
                    startCell = null;
                    return;
                }
                battleUnit = startCell.unit;
                move = startCell.unit;
                attack = startCell.unit;
                if (!startCell.unit.isAttack)
                {
                    GameUnitManage.instance.UnBlockRoad(startCell.unit.power);
                    //GameUnitManage.instance.UnBlockRoadAll();
                    //如果startCell现在没有还未行径的路径
                    if (!startCell.unit.isMove)
                    {
                        if (startCell.unit.isMoveComplete)
                        {
                            distanceInOneRound = startCell.unit.battleUnitProperty.actionPower;
                            canDrawCellList = FindRoad.instance.FindCanGoList
                                (startCell, HexGrid.instance.AllCellList, distanceInOneRound);
                            for (int i = 0; i < canDrawCellList.Count; i++)
                            {
                                canDrawCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
                            }
                            startCell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00FF76FF");
                        }
                        else
                        {
                            startCell.unit.ShowRoad(ref canDrawCellList);
                        }
                    }
                    else
                    {
                        startCell.unit.ShowRoad(ref canDrawCellList);
                    }

                    canAttackUnit = GameUnitManage.instance.FindCanAttack(startCell.unit);
                    ShowAttackeUnit();
                }
            }
            else
            {
                ExitFindRoad();
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
                }
                break;
            case (int)UnitActionEnum.Attack:
                {
                    UnitAttack();
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
            if(endCell.unit!=null)
            {
                road.Remove(endCell);
            }
            move.SetRoad(road);
            move.MoveInRound();
        }
    }

    void UnitMove(List<HexCell> roadList)
    {
        if (startCell.unit != null)
        {
            move.SetRoad(roadList);
            move.MoveInRound();
        }
    }

  
    void UnitAttack()
    {
        if(startCell.unit != null)
        {
            if(!startCell.unit.isMove)
            {
                int needMoveCount = 0;
                //如果射程不足，需要先移动一定单位
                if (!attack.IsInAttackDis(endCell.unit))
                {
                     needMoveCount = 0;
                    if(attack.NeedMoveCount(endCell.unit)>road.Count)
                    {
                        UIManage.instance.ShowTipLine("射程不足", 3);
                        return;
                    }
                    else
                    {
                        needMoveCount = attack.NeedMoveCount(endCell.unit);
                    }
                    List<HexCell> MoveList = new List<HexCell>();
                    for(int i=0;i< needMoveCount; i++)
                    {
                        MoveList.Add(road[i]);
                    }
                    UnitMove(MoveList);
                }
            }

            if(!battleUnit.isAttack)
            {
                switch(endCell.unit.unityType)
                {
                    case UnitType.Soldier:
                        {
                            attack.AttackSoldier(endCell.unit);
                            break;
                        }
                    case UnitType.Buide:
                        {
                            attack.AttackBuilder(endCell.buildUnit);
                            break;
                        }
                }
            }

        }
    }



}
