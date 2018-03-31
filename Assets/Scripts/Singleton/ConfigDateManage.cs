using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseInfo
{

    public int id;
    public string iconName;
    public string iconPath;

    public virtual void ChangeValues(string[] table)
    {
        id = int.Parse(table[0]);
        iconName = table[1];
        iconPath = table[2];
    }

    public virtual BaseInfo GetNew() { return new BaseInfo(); }
}

public class TerrainTextureInfo : BaseInfo
{
    public TerrainTypes terrainType;

    public override void ChangeValues(string[] table)
    {
        base.ChangeValues(table);
        terrainType = (TerrainTypes)int.Parse(table[3]);
    }

    public override BaseInfo GetNew()
    {
        return new TerrainTextureInfo();
    }
}
public class ConfigDateManage : Singleton<ConfigDateManage> {

	public void InitData()
    {
        FileManage.instance.LoadCSV("terrainTexture", new TerrainTextureInfo());
    }

}
