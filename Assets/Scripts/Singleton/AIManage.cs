using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManage : Singleton<AIManage> {

	// Use this for initialization
	void Start () {
		
	}
	

    public void Init()
    {

    }

    public BattleUnit nowAIUnit = null;

    public void SetTarget(BattleUnit AIUnit)
    {
        nowAIUnit = AIUnit;
        List<Unit> CanAttack = GameUnitManage.instance.FindCanAttack(AIUnit);
        CanAttack.Sort(new UnitCompare());

        if(CanAttack.Count==0)
        {
            List<BuildUnit> mainBuildList = GameUnitManage.instance.MainBuildList;
            mainBuildList.Sort(new BuildCompare());
            if(mainBuildList[0].power!=AIUnit.power)
            {
                AIUnit.SetAttackTarget(mainBuildList[0]);
            }
            else
            {
                AIUnit.SetAttackTarget(mainBuildList[1]);
            }
            
        }
        else
        {
            AIUnit.SetAttackTarget(CanAttack[0]);
        }

        
    }

    public int CalculationOfAttackWeight(Unit unit,BattleUnit AIUnit)
    {
        int injury = unit.CalculationOfInjury(AIUnit);
        IDie die ;
        if (unit.unitType == UnitType.Buide)
            die = (BuildUnit)unit;
        else if(unit.unitType == UnitType.Soldier)
            die = (BattleUnit)unit;
        else 
            die = (BattleUnit)unit;

        int isKill = die.WillDie(injury) ? 10 : 0;
        int result = injury + isKill;
        return result;
    }

    public class UnitCompare : IComparer<Unit>
    {
        public int Compare(Unit x, Unit y)
        {
            if (AIManage.instance.CalculationOfAttackWeight(x, AIManage.instance.nowAIUnit) >
                AIManage.instance.CalculationOfAttackWeight(y, AIManage.instance.nowAIUnit))
                return 1;
            else
                return -1;
        }
    }

    public class BuildCompare : IComparer<BuildUnit>
    {
        public int Compare(BuildUnit x, BuildUnit y)
        {
            if (x.cell.coordinates.DistanceToOther(AIManage.instance.nowAIUnit.cell.coordinates) >
                y.cell.coordinates.DistanceToOther(AIManage.instance.nowAIUnit.cell.coordinates))
                return -1;
            else
                return 1;
        }
    }

}
