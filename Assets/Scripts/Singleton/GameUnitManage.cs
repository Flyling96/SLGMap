using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUnitManage : Singleton<GameUnitManage> {


    /// <summary>
    /// 势力的战斗单位列表
    /// </summary>
    public Dictionary<int, List<BattleUnit>> battleUnitPowerDic = new Dictionary<int, List<BattleUnit>>();
    public Dictionary<int, List<BuildUnit>> buildUnitPowerDic = new Dictionary<int, List<BuildUnit>>();
    public List<BuildUnit> MainBuildList = new List<BuildUnit>();
    public List<int> powerList = new List<int>();
    public bool isInit = false;

    public int myPower = 0;

    public void LoadBattleUnitInit(List<BaseInfo> battleUnitInitInfoList)
    {
        battleUnitPowerDic.Clear();
        for (int i = 0; i < battleUnitInitInfoList.Count; i++)
        {
            BattleUnitInitInfo battleUnitInit = (BattleUnitInitInfo)battleUnitInitInfoList[i];
            string modelName = battleUnitInit.battleUnitInfo.modelName;
            string modelPath = (string)FileManage.instance.CSVHashTable["gameUnitModelTable"][modelName];
            GameObject battleUnitModel = GameObjectPool.instance.GetPoolChild(modelName, modelPath);
            battleUnitModel.transform.SetParent(GameObject.Find("Units").transform);
            battleUnitModel.AddComponent<BattleUnit>();
            battleUnitModel.GetComponent<BattleUnit>().battleUnitProperty = battleUnitInit.battleUnitInfo.property;
            battleUnitModel.GetComponent<BattleUnit>().Cell = HexGrid.instance.GetCell(battleUnitInit.coordinates);
            battleUnitModel.GetComponent<BattleUnit>().hud = GameObjectPool.instance.GetPoolChild("UnitHUD", UIManage.instance.NewHUD()).GetComponent<HUD>();
            battleUnitModel.GetComponent<BattleUnit>().hud.transform.SetParent(UIManage.instance.UIRoot.transform.Find("UnitHUDParent"));
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


    public void LoadBuildUnitInit(List<BaseInfo> buildUnitInitInfoList)
    {
        buildUnitPowerDic.Clear();
        for (int i = 0; i < buildUnitInitInfoList.Count; i++)
        {
            BuildUnitInitInfo buildUnitInit = (BuildUnitInitInfo)buildUnitInitInfoList[i];
            string modelName = buildUnitInit.buildUnitInfo.modelName;
            string modelPath = (string)FileManage.instance.CSVHashTable["gameUnitModelTable"][modelName];
            GameObject buildUnitModel = GameObjectPool.instance.GetPoolChild(modelName, modelPath);
            buildUnitModel.transform.SetParent(GameObject.Find("Units").transform);
            buildUnitModel.AddComponent<BuildUnit>();
            buildUnitModel.GetComponent<BuildUnit>().property = buildUnitInit.buildUnitInfo.property;
            buildUnitModel.GetComponent<BuildUnit>().Cell = HexGrid.instance.GetCell(buildUnitInit.coordinates);
            buildUnitModel.GetComponent<BuildUnit>().isMainBuild = buildUnitInit.isMainBuild;
            if(buildUnitInit.isMainBuild)
            {
                MainBuildList.Add(buildUnitModel.GetComponent<BuildUnit>());
            }
            buildUnitModel.GetComponent<BuildUnit>().hud = GameObjectPool.instance.GetPoolChild("UnitHUD", UIManage.instance.NewHUD()).GetComponent<HUD>();
            buildUnitModel.GetComponent<BuildUnit>().hud.transform.SetParent(UIManage.instance.UIRoot.transform.Find("UnitHUDParent"));
            if (buildUnitInit.power == myPower)
            {
                buildUnitModel.GetComponent<BuildUnit>().hud.transform.Find("Fill").GetComponent<Image>().color = ToolClass.instance.ConvertColor("#00DD269A");
            }
            List<HexCell> cells = HexGrid.instance.GetRangeCells(buildUnitModel.GetComponent<BuildUnit>().Cell, buildUnitModel.GetComponent<BuildUnit>().property.range);
            for(int j=0;j<cells.Count;j++)
            {
                cells[j].buildUnit = buildUnitModel.GetComponent<BuildUnit>();
            }
            BlockRoad(buildUnitModel.GetComponent<BuildUnit>());
            buildUnitModel.GetComponent<BuildUnit>().power = buildUnitInit.power;
            buildUnitModel.transform.position = buildUnitModel.GetComponent<BuildUnit>().Cell.transform.position;
            //buildUnitModel.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            if (!buildUnitPowerDic.ContainsKey(buildUnitInit.power))
            {
                buildUnitPowerDic.Add(buildUnitInit.power, new List<BuildUnit>());
            }
            buildUnitPowerDic[buildUnitInit.power].Add(buildUnitModel.GetComponent<BuildUnit>());
            if (!powerList.Contains(buildUnitInit.power))
            {
                powerList.Add(buildUnitInit.power);
            }
        }
        powerList.Sort();
    }

    public List<Unit> FindCanAttack(BattleUnit attacker)
    {
        List<Unit> result = new List<Unit>();
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

        foreach (KeyValuePair<int, List<BuildUnit>> child in buildUnitPowerDic)
        {
            List<BuildUnit> childValues = child.Value;
            if (child.Key == attacker.power)
            {
                continue;
            }
            else
            {
                for (int i = 0; i < childValues.Count; i++)
                {
                    if (CanAttack(attacker, childValues[i]))
                    {
                        result.Add(childValues[i]);
                    }
                }
            }
        }
        return result;
    }

    bool CanAttack(BattleUnit attacker,Unit hiter)
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
                UnBlockRoad(hiter);
                int realDis = FindRoad.instance.AStar(attacker.Cell, hiter.Cell,HexGrid.instance.AllCellList).Count;
                BlockRoad(hiter);

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

    public List<Unit> FindCanAttack(BuildUnit attacker)
    {
        List<Unit> result = new List<Unit>();
        foreach (KeyValuePair<int, List<BattleUnit>> child in battleUnitPowerDic)
        {
            List<BattleUnit> childValues = child.Value;
            if (child.Key == attacker.power)
            {
                continue;
            }
            else
            {
                for (int i = 0; i < childValues.Count; i++)
                {
                    if (CanAttack(attacker, childValues[i]))
                    {
                        result.Add(childValues[i]);
                    }
                }
            }
        }

        foreach (KeyValuePair<int, List<BuildUnit>> child in buildUnitPowerDic)
        {
            List<BuildUnit> childValues = child.Value;
            if (child.Key == attacker.power)
            {
                continue;
            }
            else
            {
                for (int i = 0; i < childValues.Count; i++)
                {
                    if (CanAttack(attacker, childValues[i]))
                    {
                        result.Add(childValues[i]);
                    }
                }
            }
        }
        return result;
    }

    bool CanAttack(BuildUnit attacker, Unit hiter)
    {
        if (attacker.Cell.coordinates.DistanceToOther(hiter.Cell.coordinates)
    < attacker.property.attackDistance)//+ (attacker.Cell.Elevation - hiter.Cell.Elevation))
        {
            return true;
        }
        else
        {
            return false;
        }

    }



    public void UnitDie(Unit dieUnit)
    {
        switch (dieUnit.unitType)
        {
            case UnitType.Soldier:
                {
                    battleUnitPowerDic[dieUnit.power].Remove((BattleUnit)dieUnit);
                    if (battleUnitPowerDic[dieUnit.power].Count == 0)
                    {
                        powerList.Remove(dieUnit.power);
                    }
                    Destroy(dieUnit.gameObject);
                }
                break;
            case UnitType.Buide:
                {
                    buildUnitPowerDic[dieUnit.power].Remove((BuildUnit)dieUnit);
                    if (buildUnitPowerDic[dieUnit.power].Count == 0||dieUnit.isMainBuild)
                    {
                        powerList.Remove(dieUnit.power);
                    }
                    Destroy(dieUnit.gameObject);
                }
                break;
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

    public void UnBlockRoad(Unit unit)
    {
        if (unit.unitType == UnitType.Buide)
        {
            GameUnitManage.instance.UnBlockRoad((BuildUnit)unit);
        }
        else
        {
            FindRoad.instance.UnBlockRoad(unit.cell, unit.power);
        }
    }

    public void BlockRoad(Unit unit)
    {
        if (unit.unitType == UnitType.Buide)
        {
            GameUnitManage.instance.BlockRoad((BuildUnit)unit);
        }
        else
        {
            FindRoad.instance.BlockRoad(unit.cell);
        }
    }

    public void UnBlockRoad(BuildUnit unit)
    {
        List<HexCell> cells = HexGrid.instance.GetRangeCells(unit.Cell, unit.property.range);
        for(int i=0;i<cells.Count;i++)
        {
            FindRoad.instance.UnBlockRoad(cells[i], unit.power);
        }
    }

    public void BlockRoad(BuildUnit unit)
    {
        List<HexCell> cells = HexGrid.instance.GetRangeCells(unit.Cell, unit.property.range);
        for (int i = 0; i < cells.Count; i++)
        {
            FindRoad.instance.BlockRoad(cells[i]);
        }
    }

    public void UnBlockRoad(int power)
    {
        List<BattleUnit> units = battleUnitPowerDic[power];
        for(int i=0;i<units.Count;i++)
        {
            FindRoad.instance.UnBlockRoad(units[i].Cell,power);
        }

        if (buildUnitPowerDic.ContainsKey(power))
        {
            List<BuildUnit> buildUnit = buildUnitPowerDic[power];
            for (int i = 0; i < buildUnit.Count; i++)
            {
                List<HexCell> cells = HexGrid.instance.GetRangeCells(buildUnit[i].Cell, buildUnit[i].property.range);
                for (int j = 0; j < cells.Count; j++)
                {
                    FindRoad.instance.UnBlockRoad(cells[i], power);
                }
            }
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

        if (buildUnitPowerDic.ContainsKey(power))
        {
            List<BuildUnit> buildUnit = buildUnitPowerDic[power];
            for (int i = 0; i < buildUnit.Count; i++)
            {
                List<HexCell> cells = HexGrid.instance.GetRangeCells(buildUnit[i].Cell, buildUnit[i].property.range);
                for (int j = 0; j < cells.Count; j++)
                {
                    FindRoad.instance.BlockRoad(cells[i]);
                }
            }
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
        if(isInit)
        {
            return;
        }
        foreach (KeyValuePair<int, List<BattleUnit>> child in battleUnitPowerDic)
        {
            for (int i = 0; i < child.Value.Count; i++)
            {
                child.Value[i].RefreshHUD();
            }
        }

        foreach (KeyValuePair<int, List<BuildUnit>> child in buildUnitPowerDic)
        {
            for (int i = 0; i < child.Value.Count; i++)
            {
                child.Value[i].RefreshHUD();
            }
        }
    }
}
