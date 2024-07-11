using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool instance;
    public static ObjectPool Instance
    {
        get
        {
            if (instance != null) return instance;
            else return null;
        }
    }
    private Dictionary<string, GameObject> sampleObjDict = new Dictionary<string, GameObject>();
    private Dictionary<string, Queue<Projectile>> poolingObjQueueDict = new Dictionary<string, Queue<Projectile>>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void Initialize(string key, int initCount)
    {
        for (int i = 0; i < initCount; i++)
        {
            poolingObjQueueDict[key].Enqueue(CreateNewObj(key));
        }
    }

    private Projectile CreateNewObj(string key)
    {
        var newObj = Instantiate(sampleObjDict[key]).GetComponent<Projectile>();
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    public void AddNewObj(string key, GameObject obj)
    {
        if (!sampleObjDict.ContainsKey(key))
            sampleObjDict.Add(key, obj);
        else
            sampleObjDict[key] = obj;

        if(!poolingObjQueueDict.ContainsKey(key))
            poolingObjQueueDict.Add(key, new Queue<Projectile>());
        
    }

    public static Projectile GetObj(string key)
    {
        if(Instance.poolingObjQueueDict[key].Count > 0)
        {
            var obj = Instance.poolingObjQueueDict[key].Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = Instance.CreateNewObj(key);
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }

    public static void ReturnObj(string key, Projectile obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.poolingObjQueueDict[key].Enqueue(obj);
    }
}
