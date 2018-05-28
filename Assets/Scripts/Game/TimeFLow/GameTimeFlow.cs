using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameTimeFlow : MonoBehaviour {

    //public HexGrid hexGrid;
    // Use this for initialization
    GameControl gameControl = null;
	void Start () {
        ConfigDateManage.instance.InitData();
        LoadMap("map001");
        FindRoad.instance.Init();
        GameUnitManage.instance.LoadBattleUnitInit(FileManage.instance.CSVTable["battleUnitInit"]);
        gameControl = transform.GetComponent<GameControl>();
    }
	
	// Update is called once per frame
	void Update () {
        if(GameUnitManage.instance.powerList.Count<=1)
        {
            GameOver();
        }
		
	}

    void GameOver()
    {

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
        gameControl.ExitFindRoad();
        UIManage.instance.HideActionWnd();
        RoundManage.instance.ChangePower();
        //RoundManage.instance.ChangePower();
    }


}
