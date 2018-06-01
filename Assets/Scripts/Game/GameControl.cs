using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitActionEnum
{
    Move,
    Attack,
    UnitInfo,
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
    void Start() {
        canDrawCellList.Clear();
    }

    UnitActionEnum actionType = UnitActionEnum.Move;
    // Update is called once per frame
    void Update() {

        if(GameTimeFlow.isGameOver)
        {
            return;
        }

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
                if (UIManage.instance.unitInfoWnd != null)
                {
                    if (UIManage.instance.unitInfoWnd.activeSelf)
                    {
                        UIManage.instance.HideUnitInfoWnd();
                    }
                }
                if(UIManage.instance.actionWnd!=null)
                {
                    if (UIManage.instance.actionWnd.gameObject.activeSelf)
                    {
                        UIManage.instance.HideActionWnd();
                    }
                }
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
                if ((startCell.unit != null && endCell.unit != null|| endCell.buildUnit!=null))
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

    public void ExitFindRoad()
    {
        if (startCell == null)
            return;
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
            for (int i = 0; i < canAttackUnit.Count; i++)
            {
                if (canAttackUnit[i].Cell == null)
                    continue;
                if (canAttackUnit[i].unitType == UnitType.Buide)
                {
                    List<HexCell> cells = canAttackUnit[i].GetCell();
                    for(int j=0;j<cells.Count;j++)
                    {
                        cells[j].label.transform.Find("Hightlight").GetComponent<Image>().enabled = false;
                        cells[j].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FFFFFFFF");
                    }
                }
                else
                {
                    canAttackUnit[i].Cell.label.transform.Find("Hightlight").GetComponent<Image>().enabled = false;
                    canAttackUnit[i].Cell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FFFFFFFF");
                }
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
    List<HexCell> nowCanGoList = null;
    bool isClickEndCell = false;


    void ShowMoveRoad()
    {
        if (endCell != null)
        {
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

            for(int i=road.Count-1;i>=0;i--)
            {
                if(road[i].unit!=null||road[i].buildUnit!=null)
                {
                    road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#454545FF");
                }
                else
                {
                    break;
                }
            }

            if (!canDrawCellList.Contains(endCell))
            {
                endCell.label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
                endCell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FF4040FF");
                canDrawCellList.Add(endCell);
            }

            ShowAttackeUnit();

        }
    }

    void ShowAttackRoad()
    {
        if (endCell != null)
        {
            if (endCell.unit != null)
            {
                if (endCell.unit.isDie)
                {
                    return;
                }
            }

            if(endCell.buildUnit!=null)
            {
                if (endCell.buildUnit.isDie)
                {
                    return;
                }
            }
            int moveCount=0;

            if (endCell.unit != null)
            {
                 moveCount = attack.NeedMoveCount(endCell.unit);
            }
            else if(endCell.buildUnit!=null)
            {
                moveCount = attack.NeedMoveCount(endCell.buildUnit);
            }

            for (int i = 0; i < road.Count; i++)
            {
                if (canDrawCellList.Contains(road[i]))
                {
                    if (i - 1 < moveCount)
                    {
                        if (nowCanGoList.Contains(road[i]))
                        {
                            road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00FF76FF");
                        }
                        else
                        {
                            road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#0080FFFF");
                        }

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
                        if (nowCanGoList.Contains(road[i]))
                        {
                            road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00FF76FF");
                        }
                        else
                        {
                            road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#0080FFFF");
                        }
                    }
                    else
                    {
                        road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FF4040FF");
                    }
                    canDrawCellList.Add(road[i]);
                }
            }

            //因为有单位而不能到达的用黑色
            for (int i = moveCount; i > 0; i--)
            {
                if (road[i].unit != null || road[i].buildUnit != null)
                {
                    road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#454545FF");
                }
                else
                {
                    break;
                }
            }
        }
    }

    void ShowAttackeUnit()
    {
        for (int i = 0; i < canAttackUnit.Count; i++)
        {
            if (canAttackUnit[i].unitType == UnitType.Buide)
            {
                List<HexCell> cells = canAttackUnit[i].GetCell();
                for (int j = 0; j < cells.Count; j++)
                {
                    cells[j].label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
                    cells[j].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FF4040FF");
                }
            }
            else
            {
                canAttackUnit[i].Cell.label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
                canAttackUnit[i].Cell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FF4040FF");
            }
        }
    }


    //注册单位行动信息
    void RegisterAction()
    {
        Dictionary<int, string> buttonNames = new Dictionary<int, string>();
        Vector3 position;

        if (startCell.unit.power != GameUnitManage.instance.myPower)
        {
            position = mainCamera.WorldToScreenPoint(startCell.unit.transform.position);
            buttonNames.Add(2, "UnitInfo");
        }
        else if ((startCell == endCell && startCell.unit == endCell.unit) || startCell.unit.isAttack)
        {
            position = mainCamera.WorldToScreenPoint(startCell.unit.transform.position);
            buttonNames.Add(2, "UnitInfo");
        }
        else if (endCell.unit != null)
        {
            position = mainCamera.WorldToScreenPoint(endCell.unit.transform.position);
            if (endCell.unit.power != startCell.unit.power)
            {
                if (!battleUnit.isMove)
                {
                    buttonNames.Add(0, "Move");
                    buttonNames.Add(1, "Attack");
                }
                else
                {
                    buttonNames.Add(1, "Attack");
                }
            }
            else
            {
                if (!battleUnit.isMove)
                {
                    buttonNames.Add(0, "Move");
                }
            }
        }
        //建筑相关
        else if (startCell.buildUnit!=null)
        {
            position = mainCamera.WorldToScreenPoint(startCell.buildUnit.transform.position);
            buttonNames.Add(2, "UnitInfo");
        }
        else if(endCell.buildUnit!=null)
        {
            position = mainCamera.WorldToScreenPoint(endCell.buildUnit.transform.position);
            if (endCell.buildUnit.power != startCell.unit.power)
            {
                if (!battleUnit.isMove)
                {
                    buttonNames.Add(0, "Move");
                    buttonNames.Add(1, "Attack");
                }
                else
                {
                    buttonNames.Add(1, "Attack");
                }
            }
            else
            {
                if (!battleUnit.isMove)
                {
                    buttonNames.Add(0, "Move");
                }
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
        if (!isMyPowerUnit)
        {
            return;
        }
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            endCell = HexGrid.instance.GetCell(hit.point);
            isClickEndCell = true;

            if (endCell.buildUnit != null)
            {
                if (endCell.buildUnit.power != startCell.unit.power)
                {
                    GameUnitManage.instance.UnBlockRoad(endCell.buildUnit);
                }
                road = FindRoad.instance.AStar(startCell, endCell, HexGrid.instance.AllCellList);

                if (endCell.buildUnit.power != startCell.unit.power)
                {
                    GameUnitManage.instance.BlockRoad(endCell.buildUnit);
                }
                if (road[road.Count - 1] != endCell)
                {
                    UIManage.instance.ShowTipLine("该地点无法到达", 3);
                }

                RegisterAction();
                ShowMoveRoad();

            }
            else
            {
                //获取
                if (endCell.unit != null)
                {
                    if (endCell.unit.power != startCell.unit.power)
                    {
                        FindRoad.instance.UnBlockRoad(endCell, endCell.unit.power);
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

                if (road[road.Count - 1] != endCell)
                {
                    UIManage.instance.ShowTipLine("该地点无法到达", 3);
                }

                RegisterAction();
                ShowMoveRoad();
            }
        }

    }

      
        //canDrawCellList.Add(endCell);
        //Debug.Log(canDrawCellList.IndexOf(endCell));


    IMove move;
    IAttack attack;
    BattleUnit battleUnit;
    List<Unit> canAttackUnit = new List<Unit>();
    bool isMyPowerUnit = true;
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
                isMyPowerUnit = true;
                if (startCell.unit.isAttack||startCell.unit.power!=GameUnitManage.instance.myPower)
                {
                    isMyPowerUnit = false;
                    RegisterAction();
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
                            nowCanGoList = FindRoad.instance.FindCanGoList
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
                    else if(!startCell.unit.isAttack)
                    {

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
            case (int)UnitActionEnum.UnitInfo:
                {
                    UnitInfo();
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
            if (startCell.unit.isMove)
            {
                UIManage.instance.ShowTipLine("该单位本回合已经移动过了", 3);
                ExitFindRoad();
                return;
            }

            //if (endCell.unit!=null)
            //{
            //    for (int i = road.Count - 1; i >= 0; i--)
            //    {
            //        if (road[i].unit!=null||road[i].buildUnit!=null)
            //        {
            //            road.RemoveAt(i);
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }
            //}
            //else if(endCell.buildUnit!=null&& endCell.buildUnit.power!=battleUnit.power)
            //{
            //    for(int i=road.Count-1;i>=0;i--)
            //    {
            //        if(road[i].buildUnit == endCell.buildUnit)
            //        {
            //            road.RemoveAt(i);
            //        }
            //        else if(road[i].unit != null || road[i].buildUnit != null)
            //        {
            //            road.RemoveAt(i);
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }
            //}

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

    List<HexCell> MoveList = new List<HexCell>();

    void UnitAttack()
    {
        if(startCell.unit != null)
        {
            if(!startCell.unit.isMove)
            {
                int needMoveCount = 0;
                //如果射程不足，需要先移动一定单位
                if (endCell.unit!=null&&!attack.IsInAttackDis(endCell.unit))
                {
                    bool canAttack = attack.CanAttackInRound(endCell.unit);
                    needMoveCount = attack.NeedMoveCount(endCell.unit);
                    MoveList.Clear();
                    //MoveList.Add(startCell);
                    for (int i=0;i< needMoveCount+1; i++)
                    {
                        MoveList.Add(road[i]);
                    }
                    UnitMove(MoveList);
                    if (!canAttack)
                    {
                        attack.SetAttackTarget(endCell.unit);
                        //UIManage.instance.ShowTipLine("射程不足", 3);
                        return;
                    }
                }
                else if(endCell.buildUnit!=null && !attack.IsInAttackDis(endCell.buildUnit))
                {
                    bool canAttack = attack.CanAttackInRound(endCell.buildUnit);
                    needMoveCount = attack.NeedMoveCount(endCell.buildUnit);
                    MoveList.Clear();
                    //MoveList.Add(startCell);
                    for (int i = 0; i < needMoveCount+1; i++)
                    {
                        MoveList.Add(road[i]);
                    }
                    UnitMove(MoveList);
                    canAttack = attack.CanAttackInRound(endCell.buildUnit);
                    if (!canAttack)
                    {
                        attack.SetAttackTarget(endCell.buildUnit);
                        //UIManage.instance.ShowTipLine("射程不足", 3);
                        return;
                    }
                }
            }

            if(!battleUnit.isAttack)
            {
                if(endCell.unit!=null)
                {
                    attack.AttackSoldier(endCell.unit);
                }
                else if(endCell.buildUnit!=null)
                {
                    attack.AttackBuilder(endCell.buildUnit);
                }
            }

        }
    }

    void UnitInfo()
    {
        if (startCell == null)
            return;
        if (startCell.unit != null)
        {
            UIManage.instance.ShowUnitInfoWnd(startCell.unit);
        }
    }


}
