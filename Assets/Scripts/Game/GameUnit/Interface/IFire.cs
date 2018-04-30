using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleUnitFire
{
    /// <summary>
    /// 作战单位攻击作战单位
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="hiter"></param>
    void FireSoldier( BattleUnit hiter);


    /// <summary>
    /// 作战单位攻击建筑
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="hiter"></param>
    void FireBuilder(BuildUnit hiter);

}

public interface IBuildUnitFire
{
    /// <summary>
    /// 建筑攻击作战单位
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="hiter"></param>
    void FireSoldier(BattleUnit hiter);


}
