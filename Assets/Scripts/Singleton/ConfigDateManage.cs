using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseInfo
{

    public virtual void ChangeValues(string[] table) { }

    public virtual BaseInfo GetNew() { return new BaseInfo(); }
}

public class TerrainTextureInfo : BaseInfo
{
    public int textureID;
    public string name;
    public string iconPath;
    public int terrainType;

    public override void ChangeValues(string[] table)
    {
        base.ChangeValues(table);
        textureID = int.Parse(table[0]);
        name = table[1];
        iconPath = table[2];
        terrainType = int.Parse(table[3]);
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
