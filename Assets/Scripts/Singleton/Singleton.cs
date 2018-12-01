using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSingleton : MonoBehaviour
{

}

public class Singleton<T>: BaseSingleton where T: BaseSingleton
{

    private static T _instance;
    private static object _lock = new object();

    public static T instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    string singletonName = "(singleton)" + typeof(T).ToString();
                    if (!GameObject.Find(singletonName))
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton)" + typeof(T).ToString();
                        if(!HexMetrics.instance.isEditor)
                        DontDestroyOnLoad(singleton);
                    }
                }
            }
            return _instance;
        }
    }

}

public class SingletonDestory<T> : BaseSingleton where T : BaseSingleton
{

    private static T _instance;
    private static object _lock = new object();

    public static T instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = "(singleton)" + typeof(T).ToString();
                }
            }
            return _instance;

        }
    }

}
