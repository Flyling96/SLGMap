using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class GameTimeFlow : MonoBehaviour {

    //public HexGrid hexGrid;
    // Use this for initialization
    GameControl gameControl = null;
    public GameObject GameOverWnd;

    static bool isFirst = true; 
    public static bool isGameOver = false;
	void Start ()
    {
        GameObjectPool.instance.Init();
        RoundManage.instance.Init();
        GameOverWnd.SetActive(false);
        isGameOver = false;
        if (isFirst)
        {
            ConfigDateManage.instance.InitData();
            LoadMap("map001");
            FindRoad.instance.Init();
            isFirst = false;
        }
        else
        {
            LoadMap("map001");
        }
        GameUnitManage.instance.isInit = true;
        GameUnitManage.instance.LoadBattleUnitInit(FileManage.instance.CSVTable["battleUnitInit"]);
        GameUnitManage.instance.LoadBuildUnitInit(FileManage.instance.CSVTable["buildUnitInit"]);
        GameUnitManage.instance.isInit = false;
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
        isGameOver = true;
        GameOverWnd.SetActive(true);
        string textStr = "";
        if(GameUnitManage.instance.powerList.Count==1)
        {
            if(GameUnitManage.instance.powerList[0]==GameUnitManage.instance.myPower)
            {
                textStr = "You Win!";
            }
            else
            {
                textStr = "Game Over!";
            }
        }
        else
        {
            textStr = "You Win!";
        }
        GameOverWnd.transform.Find("Text").GetComponent<Text>().text = textStr;
        GameOverWnd.transform.Find("RoundCount").GetComponent<Text>().text = "RoundCount :  "+RoundManage.instance.RoundCount;

    }
    public void Restart()
    {
        //SceneManager.LoadScene("SLGPlay");
        SceneManager.LoadScene("SLGPlay");
    }

    public void Exit()
    {
        Application.Quit();
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
