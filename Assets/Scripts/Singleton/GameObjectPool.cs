using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : Singleton<GameObjectPool> {

    public Dictionary<string, List<GameObject>> gameObjectPool = new Dictionary<string, List<GameObject>>();

    public GameObject GetPoolChild(string name ,GameObject prefab)
    {
        GameObject result = null;
        if(gameObjectPool.ContainsKey(name)&&gameObjectPool[name].Count>0)
        {
            result = gameObjectPool[name][0];
            gameObjectPool[name].RemoveAt(0);
        }
        else if(gameObjectPool.ContainsKey(name) && !(gameObjectPool[name].Count > 0))
        {
            result = GameObject.Instantiate(prefab);
        }
        else
        {
            gameObjectPool.Add(name, new List<GameObject>());
            result = GameObject.Instantiate(prefab);
        }
        result.SetActive(true);
        return result;
    }

    public void InsertChild(string name,GameObject prefab)
    {
        if(!gameObjectPool.ContainsKey(name))
        {
            gameObjectPool.Add(name, new List<GameObject>());
        }

        if (!gameObjectPool[name].Contains(prefab))
        {
            gameObjectPool[name].Add(prefab);
            prefab.SetActive(false);
        }
    }
}
