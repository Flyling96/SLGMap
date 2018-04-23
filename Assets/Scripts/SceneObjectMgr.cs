using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SceneObjectMgr : MonoBehaviour {


    public List<SceneObjectClass> sceneObjectList;
	// Use this for initialization
	void Start () {
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
            sceneObjectList[i].Refresh (false);
        }
    }

    public void AddSceneObject(SceneObjectClass item)
    {
        sceneObjectList.Add(item);
    }

    public void MinusSceneObject(SceneObjectClass item)
    {
        for(int i=0;i< sceneObjectList.Count;i++)
        {
            if(ReferenceEquals(sceneObjectList[i], item))
            {
                sceneObjectList.RemoveAt(i);
            }
        }
        GameObjectPool.instance.InsertChild(HexMetrics.instance.editorSceneObject.name, item.gameObject);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
