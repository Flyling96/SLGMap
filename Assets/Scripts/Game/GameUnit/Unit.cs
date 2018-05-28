using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour {

    public HexCell cell;
    public int power = 0;//势力编号

    public UnitType unityType = UnitType.Soldier;
    //在当前回合是否移动、攻击
    public bool isMove = false;
    public bool isAttack = false;

    public HUD hud;

    public HexCell Cell
    {
        get
        {
            return cell;
        }
        set
        {
            if (cell != null)
            {
                FindRoad.instance.UnBlockRoad(cell, power);
            }
            cell = value;
            FindRoad.instance.BlockRoad(cell);
        }
    }

    public virtual void RefreshHUD()
    {
        
    }

}
