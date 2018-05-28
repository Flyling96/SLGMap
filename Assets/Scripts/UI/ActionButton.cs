using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch(gameObject.name)
        {
            case "Button0":
                {
                    UIManage.instance.actionType = UnitActionEnum.Move;
                    break;
                }
            case "Button1":
                {
                    UIManage.instance.actionType = UnitActionEnum.Attack;
                    break;
                }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManage.instance.actionType = UnitActionEnum.Move;
    }
}
