using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct BattleUnitProperty
{
    public int unitHP;
    public int nowHP;
    public int unitMP;
    public int nowMp;
    public int actionPower;
    public int attack;
    public int defanse;
    public int attackDistance;
}

public enum UnitType
{
    Soldier,
    Buide,
}

public class BattleUnit : Unit, IAttack, IMove, IHit
{
    float moveSpeed = 2.5f;
    float rotateSpeed = 180f;


    void Start()
    {
        unityType = UnitType.Soldier;
    }

    public bool isMoveComplete = true;

    public BattleUnitProperty battleUnitProperty;//属性


    public void NewRound()
    {
        isMove = false;
        isAttack = false;
    }

    public void AttackSoldier(BattleUnit hiter)
    {
        StartCoroutine(WaitForMove(hiter));
        isAttack = true;
    }

    IEnumerator WaitForMove(BattleUnit hiter)
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

    }

    public void AttackBuilder(BuildUnit hiter)
    {
    }

    public void Hit(BattleUnit attacker)
    {
        battleUnitProperty.nowHP -= CalculationOfInjury(attacker);
    }

    int CalculationOfInjury(BattleUnit attacker)
    {
        int result = attacker.battleUnitProperty.attack * 2 - battleUnitProperty.defanse;
        return result;
    }

    public void Hit(BuildUnit attacker)
    {

    }


    public bool IsInAttackDis(Unit hiter)
    {
        if (Cell.coordinates.DistanceToOther(hiter.Cell.coordinates)
            <= battleUnitProperty.attackDistance + (Cell.Elevation - hiter.Cell.Elevation))
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
        return Cell.coordinates.DistanceToOther(hiter.Cell.coordinates) - (battleUnitProperty.attackDistance + (Cell.Elevation - hiter.Cell.Elevation));
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

    public void MoveInRound()
    {
        isMoveAnimFinish = false;
        roadInRound.Clear();
        int canMoveDistance = battleUnitProperty.actionPower;
        if (road.Count > canMoveDistance)
        {
            isMoveComplete = false;
            for (int i = 0; i < canMoveDistance; i++)
            {
                roadInRound.Add(road[0]);
                road.RemoveAt(0);
            }
            Move(roadInRound);
        }
        else
        {
            isMoveComplete = true;
            Move(road);
        }
    }

    public void Move(List<HexCell> cells)
    {
        isMove = true;
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
        cells.Clear();
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
        hud.RefreshHUD(battleUnitProperty.nowHP, battleUnitProperty.unitHP, position,40);
    }

}
