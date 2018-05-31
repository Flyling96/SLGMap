using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BuildUnitProperty
{
    public string name;
    public int unitHP;
    public int nowHP;
    public int unitMP;
    public int nowMp;
    public int attack;
    public int defence;
    public int attackDistance;
    public int range;
    public string atlasPath;
    public string iconName;
}


public class BuildUnit : Unit, IAttack
{
    void Start()
    {
        unitType = UnitType.Buide;
    }
    int fireDistance;
    bool isMainBuider = false;
    public BuildUnitProperty property;

    public void AutoAttack()
    {
        List<Unit> CanAttackUnitList = GameUnitManage.instance.FindCanAttack(this);
        for(int i=0;i<CanAttackUnitList.Count;i++)
        {
            CanAttackUnitList[i].Hit(this);
        }
    }

    public void AttackSoldier(BattleUnit hiter)
    {

    }


    public void AttackBuilder(BuildUnit hiter)
    {

    }

    bool isRefreshInjuryHUD = false;
    public override void Hit(BattleUnit attacker)
    {
        isRefreshInjuryHUD = true;
        attacker.AttackTarget = null;
        int injury = CalculationOfInjury(attacker);
        hud.data.text = injury.ToString();
        property.nowHP -= injury;
        if (property.nowHP <= 0)
        {
            StartCoroutine(WaitHUDAnimDie());
        }
    }

    int CalculationOfInjury(BattleUnit attacker)
    {
        int result = attacker.battleUnitProperty.attack * 2 - property.defence;
        return result;
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

    public void Die()
    {
        isDie = true;
        gameObject.SetActive(false);
        Cell = null;
        GameObjectPool.instance.InsertChild("UnitHUD", hud.gameObject);
        GameUnitManage.instance.UnitDie(this);
    }

    public override void Hit(BuildUnit attacker)
    {

    }

    public bool IsInAttackDis(Unit hiter)
    {
        return true;
    }

    public int NeedMoveCount(Unit hiter)
    {
        return 0;
    }
    public bool CanAttackInRound(Unit hiter)
    {
        return false;
    }

    public void SetAttackTarget(Unit target)
    {

    }

    public override void RefreshHUD()
    {
        base.RefreshHUD();
        Vector3 position = GameControl.mainCamera.WorldToScreenPoint(transform.position);
        hud.RefreshHUD(property.nowHP, property.unitHP, position, 40, ref isRefreshInjuryHUD);
    }

    public override List<HexCell> GetCell()
    {
        return HexGrid.instance.GetRangeCells(Cell, property.range);

    }

}
