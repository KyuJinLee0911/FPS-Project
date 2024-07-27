using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private Dictionary<string, GameObject> sampleObjDict = new Dictionary<string, GameObject>();
    private Dictionary<string, Queue<Projectile>> poolingObjQueueDict = new Dictionary<string, Queue<Projectile>>();

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
        if(obj == null) return;
        if (!sampleObjDict.ContainsKey(key))
            sampleObjDict.Add(key, obj);
        else
            sampleObjDict[key] = obj;

        if(!poolingObjQueueDict.ContainsKey(key))
            poolingObjQueueDict.Add(key, new Queue<Projectile>());
        
    }

    public Projectile GetProjectile(string key, float damage, float criticalMultiples, GameObject instigator)
    {
        if(poolingObjQueueDict[key].Count > 0)
        {
            var obj = poolingObjQueueDict[key].Dequeue();
            obj.SetProjectileDamage(damage, criticalMultiples, instigator);
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = CreateNewObj(key);
            newObj.SetProjectileDamage(damage, criticalMultiples, instigator);
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }

    public void ReturnObj(string key, Projectile obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(transform);
        if(poolingObjQueueDict.ContainsKey(key))
            poolingObjQueueDict[key].Enqueue(obj);
        else
            Destroy(obj.gameObject);

    }
}
