using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public enum fileType
{
    Binary,
    Xml,
    CSV,
}


public class FileManage : Singleton<FileManage>
{
    private string txtTable;

    delegate void CallBackFunction();

    void Start()
    {

    }


    public Dictionary<string, List<BaseInfo>> CSVTable = new Dictionary<string, List<BaseInfo>>();
    public void LoadCSV(string fileName, BaseInfo classType)
    {
        txtTable = ((TextAsset)Resources.Load("Data/"+fileName)).text;
        string[] msgArray = txtTable.Split('\n');
        List<BaseInfo> baseInfoList = new List<BaseInfo>();
        for(int i=1;i<msgArray.Length-1;i++)
        {
            BaseInfo newInfo = classType.GetNew();
            newInfo.ChangeValues(msgArray[i].Split(','));
            baseInfoList.Add(newInfo);
        }
        CSVTable.Add(fileName, baseInfoList);
    }

}