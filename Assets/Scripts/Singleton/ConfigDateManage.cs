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
public class ConfigDateManage : Singleton<ConfigDateManage> {

	public void InitData()
    {
        FileManage.instance.LoadHashCSV("sceneHashTable");
        FileManage.instance.LoadHashCSV("gameUnitModelTable");

        FileManage.instance.LoadCSV("terrainTexture", new TerrainTextureInfo());
        FileManage.instance.LoadCSV("terrainColor", new TerrainColorInfo());
        FileManage.instance.LoadCSV("sceneObject", new SceneObjectInfo());

    }

}
