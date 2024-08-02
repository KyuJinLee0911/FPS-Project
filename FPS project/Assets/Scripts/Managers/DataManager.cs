using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public Dictionary<int, Stat> userStats { get; private set; } = new Dictionary<int, Stat>();
    public Dictionary<int, Stat> enemyStats { get; private set; } = new Dictionary<int, Stat>();
    string path;


    public Dictionary<TKey, TValue> LoadJson<TKey, TValue>(string path) where TValue : Data<TKey>
    {
        Dictionary<TKey, TValue> newStatList = new Dictionary<TKey, TValue>();
        var loadedJson = Resources.Load<TextAsset>(path);

        JsonWrapper<TValue> wrapper = JsonUtility.FromJson<JsonWrapper<TValue>>(loadedJson.ToString());

        foreach (var data in wrapper.datas)
        {
            newStatList.Add(data.key, data);
        }
        Debug.Log(path);
        return newStatList;
    }

    private void Start()
    {

    }

    public void Init()
    {
        string userClassStatPath = "JSON/" + GameManager.Instance._class.currentClass.className + "StatData";
        if (userStats.Count == 0)
            userStats = LoadJson<int, Stat>(userClassStatPath);
        else
        {
            userStats.Clear();
            userStats = LoadJson<int, Stat>(userClassStatPath);
        }
        if (enemyStats.Count == 0)
            enemyStats = LoadJson<int, Stat>("JSON/EnemyStatData");
        path = Path.Combine(Application.dataPath + "/Resources/SaveData/", "savedData.json");
        Debug.Log(path);
    }

    public void DeleteIngameData()
    {
        if (!File.Exists(path))
            return;

        File.Delete(path);
    }
}

[Serializable]
public class JsonWrapper<T>
{
    public JsonWrapper(List<T> datas)
    {
        this.datas = datas;
    }
    public List<T> datas;
}

[Serializable]
public class DataToSave
{
    public JsonWrapper<string> classData;
    public JsonWrapper<string> perkData;
    public int totalKillCount;
    public int totalBatteryCount;
}

