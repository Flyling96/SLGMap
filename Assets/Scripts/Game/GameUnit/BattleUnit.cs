using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour, IBattleUnitFire, IMove
{
    int fireDistance;
    int moveSpeed;
    HexCell cell;

    public void FireSoldier(BattleUnit hiter)
    {

    }

    public void FireBuilder(BuildUnit hiter)
    {

    }

    public void Move(List<HexCell> cells)
    {

    }

}
