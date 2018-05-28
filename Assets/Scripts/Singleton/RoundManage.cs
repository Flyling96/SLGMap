using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManage : Singleton<RoundManage> {

    public int curPower = 0;
    

    public void NewRound(int power)
    {
        curPower = power;
        List<BattleUnit> unit = GameUnitManage.instance.battleUnitPowerDic[power];
        GameUnitManage.instance.UnBlockRoad(power);
        for (int i=0;i<unit.Count;i++)
        {
            unit[i].NewRound();
        }
    }

    public void ExitRound()
    {
        GameUnitManage.instance.BlockRoad(curPower);
    }

    public void ChangePower()
    {
        List<int> powerList = GameUnitManage.instance.powerList;
        if (powerList.IndexOf(curPower) < powerList.Count - 1)
        {
            curPower = powerList[powerList.IndexOf(curPower) + 1];
        }
        else
        {
            curPower = powerList[0];
        }

        NewRound(curPower);
    }

}
