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

    public void Refresh(List<HexCell> refreshList)
    {
        for(int i=0;i< sceneObjectList.Count;i++)
        {
            //if (refreshList.Contains(sceneObjectList[i].cell)||refreshList.Contains(sceneObjectList[i].cell.GetNeighbor(sceneObjectList[i].direction)))
            //{
            //    sceneObjectList[i].Refresh(false);
            //}
            sceneObjectList[i].Refresh(false);
        }
    }

    public void AddSceneObject(SceneObjectClass item)
    {
        sceneObjectList.Add(item);
    }

    public void MinusSceneObject(SceneObjectClass item)
    {
        for(int i= sceneObjectList.Count-1; i>=0 ;i--)
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
	// Update is called once per frame
	void Update () {
		
	}
}
