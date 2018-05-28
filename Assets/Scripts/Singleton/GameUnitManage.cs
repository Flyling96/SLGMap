using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUnitManage : Singleton<GameUnitManage> {


    /// <summary>
    /// 势力的战斗单位列表
    /// </summary>
    public Dictionary<int, List<BattleUnit>> battleUnitPowerDic = new Dictionary<int, List<BattleUnit>>();
    public List<int> powerList = new List<int>();

    public int myPower = 0;

    public void LoadBattleUnitInit(List<BaseInfo> battleUnitInitInfoList)
    {
        for (int i = 0; i < battleUnitInitInfoList.Count; i++)
        {
            BattleUnitInitInfo battleUnitInit = (BattleUnitInitInfo)battleUnitInitInfoList[i];
            string modelName = battleUnitInit.battleUnitInfo.modelName;
            string modelPath = (string)FileManage.instance.CSVHashTable["gameUnitModelTable"][modelName];
            GameObject battleUnitModel = GameObjectPool.instance.GetPoolChild(modelName, modelPath);
            battleUnitModel.AddComponent<BattleUnit>();
            battleUnitModel.GetComponent<BattleUnit>().battleUnitProperty = battleUnitInit.battleUnitInfo.property;
            battleUnitModel.GetComponent<BattleUnit>().Cell = HexGrid.instance.GetCell(battleUnitInit.coordinates);
            battleUnitModel.GetComponent<BattleUnit>().hud = GameObjectPool.instance.GetPoolChild("UnitHUD", UIManage.instance.NewHUD()).GetComponent<HUD>();
            battleUnitModel.GetComponent<BattleUnit>().hud.transform.SetParent(UIManage.instance.UIRoot.transform);
            if(battleUnitInit.power == myPower)
            {
                battleUnitModel.GetComponent<BattleUnit>().hud.transform.Find("Fill").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00DD269A");
            }
            HexGrid.instance.GetCell(battleUnitInit.coordinates).unit = battleUnitModel.GetComponent<BattleUnit>();
            battleUnitModel.GetComponent<BattleUnit>().power = battleUnitInit.power;
            battleUnitModel.transform.position = battleUnitModel.GetComponent<BattleUnit>().Cell.transform.position;
            battleUnitModel.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            if(!battleUnitPowerDic.ContainsKey(battleUnitInit.power))
            {
                battleUnitPowerDic.Add(battleUnitInit.power, new List<BattleUnit>());
            }
            battleUnitPowerDic[battleUnitInit.power].Add(battleUnitModel.GetComponent<BattleUnit>());
            if(!powerList.Contains(battleUnitInit.power))
            {
                powerList.Add(battleUnitInit.power);
            }
        }
        powerList.Sort();
    }


    public List<BattleUnit> FindCanAttack(BattleUnit attacker)
    {
        List<BattleUnit> result = new List<BattleUnit>();
        foreach(KeyValuePair<int,List<BattleUnit>> child in battleUnitPowerDic)
        {
            List<BattleUnit> childValues = child.Value;
            if (child.Key == attacker.power )
            {
                continue;
            }
            else
            {
                for (int i = 0; i < childValues.Count; i++)
                {
                    if (CanAttack(attacker,childValues[i]))
                    {
                        result.Add(childValues[i]);
                    }
                }
            }
        }
        return result;
    }


    bool CanAttack(BattleUnit attacker,BattleUnit hiter)
    {
        if (attacker.isMove)
        {
            if (attacker.Cell.coordinates.DistanceToOther(hiter.Cell.coordinates)
                < attacker.battleUnitProperty.attackDistance )//+ (attacker.Cell.Elevation - hiter.Cell.Elevation))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (attacker.Cell.coordinates.DistanceToOther(hiter.Cell.coordinates)
                <=attacker.battleUnitProperty.attackDistance +attacker.battleUnitProperty.actionPower)//+ (attacker.Cell.Elevation - hiter.Cell.Elevation))
            {
                FindRoad.instance.UnBlockRoad(hiter.Cell, hiter.power);
                int realDis = FindRoad.instance.AStar(attacker.Cell, hiter.Cell,HexGrid.instance.AllCellList).Count;
                FindRoad.instance.BlockRoad(hiter.Cell);

                if (attacker.Cell.coordinates.DistanceToOther(hiter.Cell.coordinates)
                <= attacker.battleUnitProperty.attackDistance + realDis)//+ (attacker.Cell.Elevation - hiter.Cell.Elevation)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }

    public void UnitDie(BattleUnit dieUnit)
    {
        battleUnitPowerDic[dieUnit.power].Remove(dieUnit);
        if(battleUnitPowerDic[dieUnit.power].Count == 0)
        {
            powerList.Remove(dieUnit.power);
        }
        Destroy(dieUnit.gameObject);
    }


    public void UnitAction(int power)
    {
        List<BattleUnit> units = battleUnitPowerDic[power];
        for(int i=0;i< units.Count;i++)
        {
            UnitAction(units[i]);
        }
    }

    public void UnitAction(BattleUnit unit)
    {
        unit.MoveInRound();
    }

    public void UnBlockRoad(int power)
    {
        List<BattleUnit> units = battleUnitPowerDic[power];
        for(int i=0;i<units.Count;i++)
        {
            FindRoad.instance.UnBlockRoad(units[i].Cell,power);
        }
    }

    public void UnBlockRoadAll()
    {
        foreach(KeyValuePair<int,List<BattleUnit>> child in battleUnitPowerDic)
        {
            for(int i =0;i<child.Value.Count;i++)
            {
                FindRoad.instance.UnBlockRoad(child.Value[i].Cell, child.Key);
            }
        }
    }

    public void BlockRoad(int power)
    {
        List<BattleUnit> units = battleUnitPowerDic[power];
        for (int i = 0; i < units.Count; i++)
        {
            FindRoad.instance.BlockRoad(units[i].Cell);
        }
    }

    public void BlockRoadExcept(int power)
    {
        foreach (KeyValuePair<int, List<BattleUnit>> child in battleUnitPowerDic)
        {
            if (child.Key != power)
            {
                for (int i = 0; i < child.Value.Count; i++)
                {
                    FindRoad.instance.UnBlockRoad(child.Value[i].Cell, child.Key);
                }
            }
        }
    }


    private void Update()
    {
        foreach (KeyValuePair<int, List<BattleUnit>> child in battleUnitPowerDic)
        {
            for (int i = 0; i < child.Value.Count; i++)
            {
                child.Value[i].RefreshHUD();
            }
        }
    }
}
