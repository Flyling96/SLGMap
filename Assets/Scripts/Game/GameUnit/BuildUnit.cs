using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildUnit : Unit, IAttack
{
    void Start()
    {
        unityType = UnitType.Buide;
    }
    int fireDistance;


    public void AttackSoldier(BattleUnit hiter)
    {

    }

    public void AttackBuilder(BuildUnit hiter)
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
}
