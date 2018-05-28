using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseInfo
{

    public int id;

    public virtual void ChangeValues(string[] table){}

    public virtual BaseInfo GetNew() { return new BaseInfo(); }
}

public class TerrainTextureInfo : BaseInfo
{
    public TerrainTypes terrainType;
    public string iconName;
    public string iconPath;


    public override void ChangeValues(string[] table)
    {
        id = int.Parse(table[0]);
        iconName = table[1];
        iconPath = table[2];
        terrainType = (TerrainTypes)int.Parse(table[3]);
    }

    public override BaseInfo GetNew()
    {
        return new TerrainTextureInfo();
    }
}

public class TerrainColorInfo : BaseInfo
{
    public string iconName;
    public Color itemColor;
    public override void ChangeValues(string[] table)
    {
        id = int.Parse(table[0]);
        iconName = table[1];
        itemColor = ToolClass.instance.ConvertColor(table[2]);
    }

    public override BaseInfo GetNew()
    {
        return new TerrainColorInfo();
    }
}

public class SceneObjectInfo : BaseInfo
{
    public string iconName;
    public string iconPath;
    public string modelPathType;
    public string modelPath;

    public override void ChangeValues(string[] table)
    {
        id = int.Parse(table[0]);
        iconName = table[1];
        iconPath = table[2];
        modelPathType = table[3];
        modelPath = FileManage.instance.CSVHashTable["sceneHashTable"][table[3]].ToString();
    }

    public override BaseInfo GetNew()
    {
        return new SceneObjectInfo();
    }

}

public class BattleUnitInitInfo:BaseInfo
{
    public string mapName;
    public int unitId;
    public int power;
    public BattleUnitInfo battleUnitInfo;
    public HexCoordinates coordinates;

    public override void ChangeValues(string[] table)
    {
        id = int.Parse(table[0]);
        mapName = table[1];
        unitId = int.Parse(table[2]);
        coordinates = new HexCoordinates(int.Parse(table[3]), int.Parse(table[4]));
        if(FileManage.instance.CSVTable["battleUnitInfo"]!=null)
        {
            for(int i=0;i<FileManage.instance.CSVTable["battleUnitInfo"].Count;i++)
            {
                if(FileManage.instance.CSVTable["battleUnitInfo"][i].id == unitId)
                {
                    battleUnitInfo = (BattleUnitInfo)FileManage.instance.CSVTable["battleUnitInfo"][i];
                    break;
                }
            }
        }

        power = int.Parse(table[5]);
    }

    public override BaseInfo GetNew()
    {
        return new BattleUnitInitInfo();
    }

}

public class BattleUnitInfo:BaseInfo
{
    public string modelName;
    public BattleUnitProperty property;
    public override void ChangeValues(string[] table)
    {
        id = int.Parse(table[0]);
        modelName = table[1];
        property = new BattleUnitProperty();
        property.unitHP = int.Parse(table[2]);
        property.nowHP = property.unitHP;
        property.unitMP = int.Parse(table[3]);
        property.nowMp = property.unitMP;
        property.actionPower = int.Parse(table[4]);
        property.attack = int.Parse(table[5]);
        property.defanse = int.Parse(table[6]);
        property.attackDistance = int.Parse(table[7]);
    }

    public override BaseInfo GetNew()
    {
        return new BattleUnitInfo();
    }

}
public class ConfigDateManage : Singleton<ConfigDateManage> {

	public void InitData()
    {
        FileManage.instance.LoadHashCSV("sceneHashTable");
        FileManage.instance.LoadHashCSV("gameUnitModelTable");

        FileManage.instance.LoadCSV("terrainTexture", new TerrainTextureInfo());
        FileManage.instance.LoadCSV("terrainColor", new TerrainColorInfo());
        FileManage.instance.LoadCSV("sceneObject", new SceneObjectInfo());
        FileManage.instance.LoadCSV("battleUnitInfo", new BattleUnitInfo());
        FileManage.instance.LoadCSV("battleUnitInit", new BattleUnitInitInfo());

    }

}
