using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SceneObjectMgr : MonoBehaviour {

    public List<SceneObjectClass> sceneObjectList;
	// Use this for initialization
	void OnEnable () {
        sceneObjectList = new List<SceneObjectClass>();
    }
	
    public void Clear()
    {
        sceneObjectList.Clear();
    }

    public void Refresh()
    {
        for(int i=0;i< sceneObjectList.Count;i++)
        {
            //if (refreshList.Contains(sceneObjectList[i].cell)||refreshList.Contains(sceneObjectList[i].cell.GetNeighbor(sceneObjectList[i].direction)))
            //{
            //    sceneObjectList[i].Refresh(false);
            //}
            if (sceneObjectList[i] != null)
            {
                sceneObjectList[i].Refresh(false);
            }
        }
    }

    List<SceneObjectClass> originList = new List<SceneObjectClass>();
    public void remindOriginList()
    {
        originList.Clear();
        for(int i=0;i< sceneObjectList.Count;i++)
        {
            if (sceneObjectList[i] != null)
            {
                SceneObjectClass temp = sceneObjectList[i].Clone();
                originList.Add(temp);
            }
        }
    }

    public bool isChange()
    {
        if(originList.Count!=sceneObjectList.Count)
        {
            return true;
        }

        for(int i=0;i<originList.Count;i++)
        {
            if(!originList[i].isSame(sceneObjectList[i]))
            {
                return true;
            }
        }

        return false;
    }

    public void AddSceneObject(SceneObjectClass item)
    {
        sceneObjectList.Add(item);
    }

    public void MinusSceneObject(SceneObjectClass item)
    {
        for (int i= sceneObjectList.Count-1; i>=0 ;i--)
        {
            if(ReferenceEquals(sceneObjectList[i], item))
            {
                sceneObjectList.RemoveAt(i);
                break;
            }
        }
        GameObjectPool.instance.InsertChild(item.gameObject.name, item.gameObject);
    }

    public void MinusSceneObject(HexCell cell)
    {
        for (int i = sceneObjectList.Count - 1; i >= 0; i--)
        {
            if(ReferenceEquals(sceneObjectList[i].cell,cell))
            {
                GameObjectPool.instance.InsertChild(sceneObjectList[i].gameObject.name, sceneObjectList[i].gameObject);
                sceneObjectList.RemoveAt(i);
            }
        }
    }

    public void MinusSceneObject(HexCell cell,List<SceneObjectClass> undoList)
    {
        for (int i = sceneObjectList.Count - 1; i >= 0; i--)
        {
            if (ReferenceEquals(sceneObjectList[i].cell, cell))
            {
                GameObjectPool.instance.InsertChild(sceneObjectList[i].gameObject.name, sceneObjectList[i].gameObject);
                undoList.Add(sceneObjectList[i]);
                sceneObjectList.RemoveAt(i);
            }
        }
    }
}
