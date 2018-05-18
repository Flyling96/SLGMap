using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUnitManage : Singleton<GameUnitManage> {


    /// <summary>
    /// 势力的战斗单位列表
    /// </summary>
    public Dictionary<int, List<BattleUnit>> battleUnitPowerDic = new Dictionary<int, List<BattleUnit>>();


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
            HexGrid.instance.GetCell(battleUnitInit.coordinates).unit = battleUnitModel.GetComponent<BattleUnit>();
            battleUnitModel.GetComponent<BattleUnit>().power = battleUnitInit.power;
            battleUnitModel.transform.position = battleUnitModel.GetComponent<BattleUnit>().Cell.transform.position;
            battleUnitModel.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            if(!battleUnitPowerDic.ContainsKey(battleUnitInit.power))
            {
                battleUnitPowerDic.Add(battleUnitInit.power, new List<BattleUnit>());
            }
            battleUnitPowerDic[battleUnitInit.power].Add(battleUnitModel.GetComponent<BattleUnit>());
        }
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


}
