using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct BattleUnitProperty
{
    public string name;
    public int unitHP;
    public int nowHP;
    public int unitMP;
    public int nowMp;
    public int actionPower;
    public int attack;
    public int defence;
    public int attackDistance;
    public string atlasPath;
    public string iconName;
}

public enum UnitType
{
    Soldier,
    Buide,
}

public class BattleUnit : Unit, IAttack, IMove,IDie
{
    float moveSpeed = 2.5f;
    float rotateSpeed = 180f;


    void Start()
    {
        unitType = UnitType.Soldier;
    }

    public bool isMoveComplete = true;

    public BattleUnitProperty battleUnitProperty;//属性


    public void NewRound()
    {
        isMove = false;
        isAttack = false;
        RefreshAttackRoad();
        RefreshRoadInRound();
    }

    public void AttackSoldier(BattleUnit hiter)
    {
        StartCoroutine(WaitForMove(hiter));
        isAttack = true;
    }

    IEnumerator WaitForMove(Unit hiter)
    {
        int count = 0;
        while(count<50)
        {
            if(!isMoveAnimFinish)
            {
                yield return new WaitForSeconds(0.1f);
                count++;
            }
            else
            {
                break;
            }
        }

        StartCoroutine(LookAt(hiter.transform.position));
        hiter.Hit(this);
        road.Clear();

    }

    public void AttackBuilder(BuildUnit hiter)
    {
        StartCoroutine(WaitForMove(hiter));
        isAttack = true;
    }

    bool isRefreshInjuryHUD = false;
    public override void Hit(BattleUnit attacker)
    {
        isRefreshInjuryHUD = true;
        attacker.AttackTarget = null;
        int injury = CalculationOfInjury(attacker);
        hud.data.text = injury.ToString();
        battleUnitProperty.nowHP -= injury;
        if(battleUnitProperty.nowHP<=0)
        {
            StartCoroutine(WaitHUDAnimDie());
        }
    }
    IEnumerator WaitHUDAnimDie()
    {
        int count = 0;
        while (count < 50)
        {
            if (!hud.isDie())
            {
                yield return new WaitForSeconds(0.1f);
                count++;
            }
            else
            {
                break;
            }
        }
        Die();
    }

    int CalculationOfInjury(BattleUnit attacker)
    {
        int result = attacker.battleUnitProperty.attack * 2 - battleUnitProperty.defence;
        return result;
    }

    int CalculationOfInjury(BuildUnit attacker)
    {
        int result = attacker.property.attack * 2 - battleUnitProperty.defence;
        if (result < 0) result = 0;
        return result;
    }

    public override void Hit(BuildUnit attacker)
    {
        isRefreshInjuryHUD = true;
        int injury = CalculationOfInjury(attacker);
        hud.data.text = injury.ToString();
        battleUnitProperty.nowHP -= injury;
        if (battleUnitProperty.nowHP <= 0)
        {
            StartCoroutine(WaitHUDAnimDie());
        }
    }

    public void Die()
    { 
        isDie = true;
        gameObject.SetActive(false);
        Cell = null;
        GameObjectPool.instance.InsertChild("UnitHUD", hud.gameObject);
        GameUnitManage.instance.UnitDie(this);
    }

    public bool IsInAttackDis(Unit hiter)
    {
        if (Cell.coordinates.DistanceToOther(hiter.Cell.coordinates)
            <= battleUnitProperty.attackDistance )//+(Cell.Elevation - hiter.Cell.Elevation))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int NeedMoveCount(Unit hiter)
    {
        return Cell.coordinates.DistanceToOther(hiter.Cell.coordinates) - battleUnitProperty.attackDistance ;//(Cell.Elevation - hiter.Cell.Elevation));
    }

    public bool CanAttackInRound(Unit hiter)
    {
        if(NeedMoveCount(hiter)>battleUnitProperty.actionPower)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public Unit AttackTarget = null;
    public void SetAttackTarget(Unit target)
    {
        AttackTarget = target;
    }

    void RefreshAttackRoad()
    {
        if (AttackTarget!=null)
        {
            isMoveComplete = false;
            //重新计算位置
            FindRoad.instance.UnBlockRoad(AttackTarget.cell, AttackTarget.power);
            SetRoad(FindRoad.instance.AStar(Cell, AttackTarget.Cell, HexGrid.instance.AllCellList));
            FindRoad.instance.BlockRoad(AttackTarget.cell);
        }
        else
        {
            isMoveComplete = true;
        }
    }
    void RefreshRoadInRound()
    {
        roadInRound.Clear();
        if (AttackTarget != null)
        {
            if (road.Count <= battleUnitProperty.attackDistance)
            {
                return;
            }
            else
            {
                int need = NeedMoveCount(AttackTarget)+1;
                for (int i = 0; i < need; i++)
                {
                    if (road.Count > 0)
                    {
                        roadInRound.Add(road[0]);
                    }
                    else
                    {
                        break;
                    }
                }
                RemoveEndCell(roadInRound);
            }
        }
        else
        {
            int canMoveDistance = battleUnitProperty.actionPower;
            roadInRound.Add(Cell);
            for (int i = 0; i < canMoveDistance; i++)
            {
                if (road.Count > 0)
                {
                    roadInRound.Add(road[0]);
                }
                else
                {
                    break;
                }
            }
            RemoveEndCell(roadInRound);
        }

    }
    public void AutoAttack()
    {
        if (AttackTarget == null||isAttack)
            return;
        if (!isMove)
        {
            if (CanAttackInRound(AttackTarget))
            {
                if (!IsInAttackDis(AttackTarget))
                {
                    AutoMove();
                }

                switch (AttackTarget.unitType)
                {
                    case UnitType.Soldier:
                        {
                            AttackSoldier((BattleUnit)AttackTarget);
                            break;
                        }
                    case UnitType.Buide:
                        {
                            AttackBuilder((BuildUnit)AttackTarget);
                            break;
                        }
                }
            }
            else
            {
                AutoMove();
            }
        }
        else
        {
            if (IsInAttackDis(AttackTarget))
            {
                switch (AttackTarget.unitType)
                {
                    case UnitType.Soldier:
                        {
                            AttackSoldier((BattleUnit)AttackTarget);
                            break;
                        }
                    case UnitType.Buide:
                        {
                            AttackBuilder((BuildUnit)AttackTarget);
                            break;
                        }
                }
            }

        }
    }


    List<HexCell> road = new List<HexCell>();
    List<HexCell> roadInRound = new List<HexCell>();
    List<HexCell> canGotoCellList = new List<HexCell>();


    public void ShowRoad(ref List<HexCell> canGoCellList)
    {
        canGotoCellList.Clear();
        int distanceInOneRound = battleUnitProperty.actionPower;
        canGotoCellList = FindRoad.instance.FindCanGoList(cell, HexGrid.instance.AllCellList, distanceInOneRound);
        for (int i = 0; i < canGotoCellList.Count; i++)
        {
            canGotoCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
        }
        cell.label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00FF76FF");

        for(int i=0;i< roadInRound.Count;i++)
        {
            roadInRound[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00FF76FF");
        }

        for(int i=0;i<road.Count;i++)
        {
            road[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = true;
            road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#0080FFFF");
            canGotoCellList.Add(road[i]);
        }

        if(AttackTarget!=null)
        {
            int need = NeedMoveCount(AttackTarget)+1;
            if(need>roadInRound.Count)
            {
                need -= roadInRound.Count;
                for(int i=need;i<road.Count;i++)
                {
                    road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FF4040FF");
                }
            }
            else
            {
                for(int i=need;i<roadInRound.Count;i++)
                {
                    roadInRound[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FF4040FF");
                }
                for(int i=0;i<road.Count;i++)
                {
                    road[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FF4040FF");
                }
            }
        }
        canGoCellList = canGotoCellList;
    }

    public void HideRoad()
    {
        for(int i=0;i<canGotoCellList.Count;i++)
        {
            canGotoCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().enabled = false;
            canGotoCellList[i].label.transform.Find("Hightlight").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#FFFFFFFF");
        }
    }

    public void SetRoad(List<HexCell> roadCells)
    {
        road = roadCells;
    }

    public void AutoMove()
    {
        if(!isMove&& roadInRound.Count>1)
        {
            AutoMoveInRound();
        }
    }

    void AutoMoveInRound()
    {
        isMove = true;
        isMoveAnimFinish = false;
        if (road.Count > 0)
        {
            isMoveComplete = false;
        }
        else
        {
            isMoveComplete = true;
        }
        Move(roadInRound);
    }

    public void MoveInRound()
    {
        AttackTarget = null;
        isMove = true;
        isMoveAnimFinish = false;
        roadInRound.Clear();
        int canMoveDistance = battleUnitProperty.actionPower;
        if (road.Count > canMoveDistance+1)
        {
            isMoveComplete = false;
            for (int i = 0; i < canMoveDistance+1; i++)
            {
                roadInRound.Add(road[i]);
            }

            //终点处有单位
            RemoveEndCell(roadInRound);
            Move(roadInRound);
        }
        else
        {
            isMoveComplete = true;
            for (int i = road.Count - 1; i >= 0; i--)
            {
                if (road[i].unit != null || road[i].buildUnit != null)
                {
                    road.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
            Move(road);
        }
    }
    public void RemoveEndCell(List<HexCell> roadCells)
    {
        //终点处有单位
        for (int i = roadCells.Count - 1; i >= 0; i--)
        {
            if (roadCells[i].unit != null || roadCells[i].buildUnit != null)
            {
                roadCells.RemoveAt(i);
            }
            else
            {
                break;
            }
        }
        for (int i = 0; i < roadCells.Count; i++)
        {
            road.Remove(roadCells[i]);
        }
    }

    public void Move(List<HexCell> cells)
    {
        if(cells.Count<=1)
        {
            return;
        }
        Cell.unit = null;
        cells[cells.Count - 1].unit = this;
        Cell = cells[cells.Count - 1];
        StartCoroutine(MoveAnim(cells));
    }

    bool isMoveAnimFinish = true;
    IEnumerator MoveAnim(List<HexCell> cells)
    {
        Vector3 a, b, c = cells[0].transform.position;
        yield return LookAt(cells[1].transform.position);

        for (int i = 1; i < cells.Count + 1; i++)
        {
            a = c;
            if (i != cells.Count)
            {
                b = cells[i - 1].transform.position;
                c = (b + cells[i].transform.position) * 0.5f;
            }
            else
            {
                b = cells[cells.Count - 1].transform.position;
                c = b;
            }
            for (float t = Time.deltaTime * moveSpeed; t < 1f; t += Time.deltaTime * moveSpeed)
            {
                //transform.position = Vector3.Lerp(start, end, t);
                transform.position = ToolClass.instance.GetBezierPoint(a, b, c, t);
                Vector3 dir = -ToolClass.instance.GetBezierDerivative(a, b, c, t);
                dir.y = 0f;
                transform.localRotation = Quaternion.LookRotation(dir);
                yield return null;
            }
        }
        isMoveAnimFinish = true;
       // cells.Clear();

    }

    IEnumerator LookAt(Vector3 point)
    {
        point.y = transform.position.y;
        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation =Quaternion.LookRotation(transform.position - point);
        float speed = rotateSpeed / Quaternion.Angle(fromRotation, toRotation);

        for (float t = Time.deltaTime * speed;t < 1f;t += Time.deltaTime * speed)
        {
            transform.localRotation =Quaternion.Slerp(fromRotation, toRotation, t);
            yield return null;
        }

    }

    public override void RefreshHUD()
    {
        base.RefreshHUD();
        Vector3 position = GameControl.mainCamera.WorldToScreenPoint(transform.position);
        hud.RefreshHUD(battleUnitProperty.nowHP, battleUnitProperty.unitHP, position,40,ref isRefreshInjuryHUD);
    }



}
