using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public Dictionary<int, Stat> userStats { get; private set; } = new Dictionary<int, Stat>();
    public Dictionary<int, Stat> enemyStats { get; private set; } = new Dictionary<int, Stat>();
    [SerializeField] DataToSave data = new DataToSave();
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

    public async void SaveUserData()
    {
        DataToSave data = new DataToSave();
        data.totalBatteryCount = GameManager.Instance._item.batteries;
        data.totalKillCount = GameManager.Instance.totalKillCount;
        data.classData = GetClassKey();

        string json = JsonUtility.ToJson(data, true);

        await File.WriteAllTextAsync(path, json);
        Debug.Log("Saved");
    }

    public void LoadUserData()
    {
        if (!File.Exists(path))
        {
            GameManager.Instance._item.batteries = 0;
            GameManager.Instance.totalKillCount = 0;
            if (GameManager.Instance._class.unlockedClasses.Count == 0)
                GameManager.Instance._class.unlockedClasses.Add(GameManager.Instance._class.playerClassDatas[0]);
            SaveUserData();
        }
        else
        {
            string loadedJson = File.ReadAllText(path);
            data = JsonUtility.FromJson<DataToSave>(loadedJson);
            GameManager.Instance._item.batteries = data.totalBatteryCount;
            GameManager.Instance.totalKillCount = data.totalKillCount;
            foreach (var type in data.classData.datas)
            {
                PlayerClassData savedClassData = GameManager.Instance._class.playerClassDatas[type];
                if (!GameManager.Instance._class.unlockedClasses.Contains(savedClassData))
                    GameManager.Instance._class.unlockedClasses.Add(savedClassData);
            }
        }

        Debug.Log("Loaded");
    }

    public JsonWrapper<int> GetClassKey()
    {
        List<int> types = new List<int>();


        foreach (var data in GameManager.Instance._class.unlockedClasses)
        {
            types.Add((int)data.classType);
        }

        JsonWrapper<int> classWrappers = new JsonWrapper<int>(types);

        return classWrappers;
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
    public JsonWrapper<int> classData;
    public JsonWrapper<string> perkData;
    public int totalKillCount;
    public int totalBatteryCount;
}

