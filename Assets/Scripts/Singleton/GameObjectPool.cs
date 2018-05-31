using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : Singleton<GameObjectPool> {

    public Dictionary<string, List<GameObject>> gameObjectPool = new Dictionary<string, List<GameObject>>();

    public void Init()
    {
        gameObjectPool.Clear();
    }

    public GameObject GetPoolChild(string name ,GameObject prefab)
    {
        name = name.Replace("(Clone)", "");
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

    public GameObject GetPoolChild(string name, string path)
    {
        name = name.Replace("(Clone)", "");
        GameObject result = null;
        if (gameObjectPool.ContainsKey(name) && gameObjectPool[name].Count > 0)
        {
            result = gameObjectPool[name][0];
            gameObjectPool[name].RemoveAt(0);
        }
        else if (gameObjectPool.ContainsKey(name) && !(gameObjectPool[name].Count > 0))
        {
            result = GameObject.Instantiate(Resources.Load(path) as GameObject);
        }
        else
        {
            gameObjectPool.Add(name, new List<GameObject>());
            result = GameObject.Instantiate(Resources.Load(path) as GameObject);
        }
        result.SetActive(true);
        return result;
    }

    public void InsertChild(string name,GameObject prefab)
    {
        name = name.Replace("(Clone)", "");
        if (!gameObjectPool.ContainsKey(name))
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
