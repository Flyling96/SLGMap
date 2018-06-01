using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

public class UIManage : Singleton<UIManage> {

    public GameObject tipWnd = null;
    public GameObject uiRoot = null;
    public TipUI inputWnd = null;
    public TipLineUI tipLine = null;
    public DownSelectWnd downSelectWnd = null;
    public GameObject unitInfoWnd = null;
    public ActionWnd actionWnd = null;
    public Button roundButton = null;


    private void Start()
    {
        roundButton = UIRoot.transform.Find("RoundButton").GetComponent<Button>();
    }

    public GameObject UIRoot
    {
        get
        {
            if (uiRoot == null)
                uiRoot = HexMapEditor.uiRoot;
            if (uiRoot == null)
                uiRoot = GameObject.Find("UICanvas");
            return uiRoot;
        }
    }

    public void ShowDownSelectWnd(List<BaseInfo> info, DownSelectWndType type)
    {
        if(downSelectWnd==null)
        {
            GameObject temp = GameObject.Instantiate(Resources.Load("Prefabs/UIPrefabs/DownSelectWnd") as GameObject);
            temp.transform.SetParent(UIRoot.transform);
            temp.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            temp.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            downSelectWnd = temp.GetComponent<DownSelectWnd>();
            downSelectWnd.ShowDownSelectWnd(info, type);
        }
        else
        {
            downSelectWnd.gameObject.SetActive(true);
            downSelectWnd.ShowDownSelectWnd(info, type);
        }
    }

    public void HideDownSelectWnd(string name)
    {
        if (downSelectWnd != null)
        {
            AddClearPool(name, downSelectWnd.transform.Find("ScrollView/Viewport/Content"));
            downSelectWnd.gameObject.SetActive(false);
        }
    }

    public void ShowInputWnd(List<string> inputName, TipUI.OnInputConfirm confirm, TipUI.OnInputCancel cancel,string title,List<InputType> inputType)
    {
        if(inputWnd==null)
        {
            GameObject temp =  GameObject.Instantiate(Resources.Load("Prefabs/UIPrefabs/InputWnd")as GameObject);
            temp.transform.SetParent(UIRoot.transform);
            temp.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            temp.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            inputWnd = temp.GetComponent<TipUI>();
            inputWnd.transform.Find("Title").GetComponent<Text>().text = title;
            inputWnd.ShowInputWnd(inputName, confirm, cancel, inputType);
        }
        else
        {
            inputWnd.transform.Find("Title").GetComponent<Text>().text = title;
            inputWnd.gameObject.SetActive(true);
            inputWnd.ShowInputWnd(inputName, confirm, cancel, inputType);
        }
    }

    public void HideInputWnd(string name)
    {
        if(inputWnd!=null)
        {
            AddPool(name, inputWnd.transform.Find("ScrollView/Viewport/Content"));
            inputWnd.gameObject.SetActive(false);
        }
    }

    public void HideInputWnd()
    {
        if (inputWnd != null)
        {
            inputWnd.gameObject.SetActive(false);
        }
    }


    public void ShowTipLine(string content,float time)
    {
        if (tipLine == null)
        {
            GameObject temp = GameObject.Instantiate(Resources.Load("Prefabs/UIPrefabs/TipLine") as GameObject);
            temp.transform.SetParent(UIRoot.transform);
            temp.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            temp.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            tipLine = temp.GetComponent<TipLineUI>();
            tipLine.ShowTipLine(content, time);
        }
        else
        {
            tipLine.gameObject.SetActive(true);
            tipLine.ShowTipLine(content, time);
        }
    }

    public void ShowUnitInfoWnd(BattleUnit unit)
    {
        if (unitInfoWnd == null)
        {
            unitInfoWnd = GameObject.Instantiate(Resources.Load("Prefabs/UIPrefabs/UnitInfoWnd") as GameObject);
            unitInfoWnd.transform.SetParent(UIRoot.transform);
            unitInfoWnd.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            unitInfoWnd.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        }
        else
        {
            unitInfoWnd.gameObject.SetActive(true);
        }

        unitInfoWnd.transform.Find("name").GetComponent<Text>().text = unit.battleUnitProperty.name;
        //unitInfoWnd.transform.Find("icon").GetComponent<Image>().sprite  = (Sprite)Resources.LoadAll(unit.battleUnitProperty.iconPath)[2];
        unitInfoWnd.transform.Find("icon").GetComponent<Image>().sprite = AtlasManage.instance.LoadAtlasSprite(unit.battleUnitProperty.atlasPath, unit.battleUnitProperty.iconName);
        unitInfoWnd.transform.Find("pro").transform.Find("UnitHUD").GetComponent<Slider>().value = unit.battleUnitProperty.nowHP / (unit.battleUnitProperty.unitHP * 1.0f);
        unitInfoWnd.transform.Find("pro").transform.Find("HP").GetComponent<Text>().text = "HP: " + unit.battleUnitProperty.nowHP + "/" + unit.battleUnitProperty.unitHP;
        unitInfoWnd.transform.Find("pro").transform.Find("Attack").GetComponent<Text>().text = "攻击: " + unit.battleUnitProperty.attack;
        unitInfoWnd.transform.Find("pro").transform.Find("Defence").GetComponent<Text>().text = "防御: " + unit.battleUnitProperty.defence;
        unitInfoWnd.transform.Find("pro").transform.Find("AttackDis").GetComponent<Text>().text = "射程: " + unit.battleUnitProperty.attackDistance;
        unitInfoWnd.transform.Find("pro").transform.Find("ActionPower").GetComponent<Text>().text = "移动距离: " + unit.battleUnitProperty.actionPower;


    }

    public void HideUnitInfoWnd()
    {
        unitInfoWnd.SetActive(false);
    }

    public UnitActionEnum actionType = UnitActionEnum.Move;
    public void ShowActionWnd(ActionWnd.OnClickButton cb, Dictionary<int, string> buttonNames)
    {
        if (actionWnd == null)
        {
            GameObject temp = GameObject.Instantiate(Resources.Load("Prefabs/UIPrefabs/ActionWnd") as GameObject);
            temp.transform.SetParent(UIRoot.transform);
            temp.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            temp.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            actionWnd = temp.GetComponent<ActionWnd>();
            actionWnd.ShowActionWnd(cb, buttonNames);
        }
        else
        {
            actionWnd.gameObject.SetActive(true);
            actionWnd.ShowActionWnd(cb, buttonNames);
        }
    }

    GameObject HUDPrefab;
    public GameObject NewHUD()
    {
        if(HUDPrefab==null)
        {
            HUDPrefab = GameObject.Instantiate(Resources.Load("Prefabs/UIPrefabs/UnitHUD") as GameObject);
        }
        return HUDPrefab;
    }


    public void HideActionWnd()
    {
        if (actionWnd == null)
            return;
        actionWnd.HideActionWnd();
        actionWnd.gameObject.SetActive(false);
    }

    public void AddPool(string name, Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.activeSelf == false)
            {
                continue;
            }

            GameObjectPool.instance.InsertChild(name, child.gameObject);
        }
    }

    public void AddPool(string name, GameObject item)
    {
        if(item.activeSelf==false)
        {
            return;
        }
        GameObjectPool.instance.InsertChild(name, item);
    }

    public void AddPool(string name ,List<GameObject> item)
    {
        for(int i=0;i<item.Count;i++)
        {
            if(item[i].activeSelf==false)
            {
                continue;
            }
            GameObjectPool.instance.InsertChild(name, item[i]);
        }
    }

    public void AddClearPool(string name, Transform parent)
    {
        foreach (Transform child in parent)
        {
            if(child.gameObject.activeSelf == false)
            {
                continue;
            }
            foreach (var component in child.GetComponents<MonoBehaviour>())
            {
                Destroy(component);
            }
            GameObjectPool.instance.InsertChild(name, child.gameObject);
        }
    }

    public void RefreshHUD()
    {

    }

}
