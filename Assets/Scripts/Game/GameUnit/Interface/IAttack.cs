using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack
{

    bool CanAttackInRound(Unit hiter);

    bool IsInAttackDis(Unit hiter);

    int NeedMoveCount(Unit hiter);

    void SetAttackTarget(Unit target);

    /// <summary>
    /// 作战单位攻击作战单位
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="hiter"></param>
    void AttackSoldier( BattleUnit hiter);


    /// <summary>
    /// 作战单位攻击建筑
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="hiter"></param>
    void AttackBuilder(BuildUnit hiter);


}


public interface IHit
{
    /// <summary>
    /// 作战单位受击
    /// </summary>
    /// <param name="attacker"></param>
    void Hit(BattleUnit attacker);

    void Hit(BuildUnit attacker);
}
