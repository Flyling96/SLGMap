using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUnitManage : Singleton<GameUnitManage> {


    List<BattleUnit> battleUnitList = new List<BattleUnit>();

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
            battleUnitModel.GetComponent<BattleUnit>().cell = HexGrid.instance.GetCell(battleUnitInit.coordinates);
            HexGrid.instance.GetCell(battleUnitInit.coordinates).unit = battleUnitModel.GetComponent<BattleUnit>();
            battleUnitModel.transform.position = battleUnitModel.GetComponent<BattleUnit>().cell.transform.position;
            battleUnitModel.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            battleUnitList.Add(battleUnitModel.GetComponent<BattleUnit>());
        }
    }


}
