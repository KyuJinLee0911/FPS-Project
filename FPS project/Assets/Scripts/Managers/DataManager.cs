using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public Dictionary<int, Stat> userStats { get; private set; } = new Dictionary<int, Stat>();
    public Dictionary<int, Stat> enemyStats { get; private set; } = new Dictionary<int, Stat>();
    string path;
    private SaveData data = new SaveData();


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
        if (enemyStats.Count == 0)
            enemyStats = LoadJson<int, Stat>("JSON/EnemyStatData");
        path = Path.Combine(Application.dataPath + "/Resources/SaveData/", "savedData.json");
        Debug.Log(path);
    }

    public void SaveIngameData()
    {
        Player player = GameManager.Instance.player;
        data.hp = player.hp;
        data.defence = player.defence;
        data.level = player.level;
        data.currentExp = player.exp;
        data.currentClass = (int)GameManager.Instance._class.currentClass.classType;
        data.autoCriticalRate = player.autoCriticalRate;
        data.autoCriticalMagnification = player.autoCriticalMagnification;

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, json);
        Debug.Log("Save Complete");
    }

    public void LoadIngameData()
    {

        Player player = GameManager.Instance.player;
        if (!File.Exists(path))
        {
            Debug.Log("No file exist. Create new save file");
            player.level = userStats[1].level;
            player.hp = userStats[1].hp;
            player.maxHp = userStats[1].hp;
            player.defence = userStats[1].defence;
            player.exp = 0;
            player.expToNextLevel = userStats[1].expToNextLevel;
            GameManager.Instance._class.currentClass = GameManager.Instance._class.playerClassDatas[0];
            player.autoCriticalRate = 0.1f;
            player.autoCriticalMagnification = 1.75f;

            SaveIngameData();
        }
        else
        {
            string loadedJson = File.ReadAllText(path);
            data = JsonUtility.FromJson<SaveData>(loadedJson);

            player.level = data.level;
            player.hp = data.hp;
            player.maxHp = userStats[player.level].hp;
            player.defence = data.defence;
            player.exp = data.currentExp;
            player.expToNextLevel = userStats[data.level].expToNextLevel;
            GameManager.Instance._class.currentClass = GameManager.Instance._class.playerClassDatas[data.currentClass];
            player.autoCriticalRate = data.autoCriticalRate;
            player.autoCriticalMagnification = data.autoCriticalMagnification;

            Debug.Log("Successfully Loaded");
        }
    }

    public bool IsSaveDataExist()
    {
        return File.Exists(path);
    }

    public void DeleteIngameData()
    {
        if (!File.Exists(path))
            return;

        File.Delete(path);
    }
}

[Serializable]
public class SaveData
{
    public float hp;
    public float maxHp;
    public float defence;
    public int level;
    public int currentExp;
    public int currentClass;
    public float autoCriticalRate;
    public float autoCriticalMagnification;
}

public class JsonWrapper<T>
{
    public JsonWrapper(List<T> datas)
    {
        this.datas = datas;
    }
    public List<T> datas;
}
