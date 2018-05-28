using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameTimeFlow : MonoBehaviour {

    //public HexGrid hexGrid;
	// Use this for initialization
	void Start () {
        ConfigDateManage.instance.InitData();
        LoadMap("map001");
        FindRoad.instance.Init();
        GameUnitManage.instance.LoadBattleUnitInit(FileManage.instance.CSVTable["battleUnitInit"]);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void LoadMap(string fileName)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(Application.streamingAssetsPath + "/" + fileName, FileMode.Open)))
        {
            HexGrid.instance.Load(reader);
        }

        HexGrid.instance.Refresh();
    }

    public void ExitMyRound()
    {
        RoundManage.instance.ChangePower();
    }


}
