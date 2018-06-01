using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManage : Singleton<RoundManage> {

    public int curPower = 0;
    public int RoundCount = 1;


    public void NewRound(int power)
    {
        curPower = power;
        UIManage.instance.roundButton.interactable = false;

        List<BattleUnit> unit = GameUnitManage.instance.battleUnitPowerDic[curPower];
        if (power == GameUnitManage.instance.myPower)
        {
            UIManage.instance.roundButton.interactable = true;
            for (int i = 0; i < unit.Count; i++)
            {
                unit[i].NewRound();
                unit[i].NewRoundRefresh();
            }
            RoundCount++;
        }
        else
        {
            GameUnitManage.instance.UnBlockRoad(curPower);
            for(int i=0;i<unit.Count;i++)
            {
                unit[i].NewRound();
                AIManage.instance.SetTarget(unit[i]);
                unit[i].NewRoundRefresh();
            }
            ChangePower();
        }


    }

    private void Update()
    {

    }



    public void Init()
    {
        RoundCount = 1;
        curPower = 0;
    }

    void ExitRound()
    {
        GameUnitManage.instance.BlockRoad(curPower);
    }

    IEnumerator waitUnitAction()
    {
        List<BattleUnit> unit = GameUnitManage.instance.battleUnitPowerDic[curPower];
        for (int i = 0; i < unit.Count; i++)
        {
            unit[i].RemoveEndCell();
            if (unit[i].AttackTarget != null)
            {
                unit[i].AutoAttack();
                while(!unit[i].isMoveAnimFinish)
                {
                    yield return null;
                }
            }
            else
            {
                unit[i].AutoMove();
                while (!unit[i].isMoveAnimFinish)
                {
                    yield return null;
                }
            }
        }
        List<BuildUnit> buildUnit = GameUnitManage.instance.buildUnitPowerDic[curPower];
        for (int i = 0; i < buildUnit.Count; i++)
        {
            buildUnit[i].AutoAttack();
        }
        List<int> powerList = GameUnitManage.instance.powerList;
        ExitRound();
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

    
    public void ChangePower()
    {
        StartCoroutine(waitUnitAction());
    }

}
