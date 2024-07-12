using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public Dictionary<int, Stat> userStats { get; private set; } = new Dictionary<int, Stat>();
    public Dictionary<int, Stat> enemyStats { get; private set; } = new Dictionary<int, Stat>();
    string path;
    private SaveData data = new SaveData();

    Dictionary<int, Stat> LoadJson(string path)
    {
        Dictionary<int, Stat> newStatList = new Dictionary<int, Stat>();
        var loadedJson = Resources.Load<TextAsset>(path);

        StatData myStat = JsonUtility.FromJson<StatData>(loadedJson.ToString());

        foreach (var stat in myStat.stats)
        {
            newStatList.Add(stat.level, stat);
        }
        return newStatList;
    }

    private void Start()
    {

    }

    public void Init()
    {
        string userClassStatPath = "JSON/" + GameManager.Instance._class.currentClass.className + "StatData";
        userStats = LoadJson(userClassStatPath);
        if (enemyStats.Count == 0)
            enemyStats = LoadJson("JSON/EnemyStatData");
        path = Path.Combine(Application.dataPath + "/Resources/SaveData/", "savedData.json");
        Debug.Log(path);
    }

    public void SaveData()
    {
        Player player = GameManager.Instance.player;
        data.pos = player.transform.position;
        data.rot = player.transform.rotation.eulerAngles;
        data.hp = player.hp;
        data.defence = player.defence;
        data.level = player.level;
        data.currentExp = 0;

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, json);
        Debug.Log("Save Complete");
    }

    public void LoadData()
    {

        Player player = GameManager.Instance.player;
        if (!File.Exists(path))
        {
            Debug.Log("No file exist. Create new save file");
            player.level = userStats[1].level;
            player.hp = userStats[1].hp;
            player.defence = userStats[1].defence;
            player.currentExp = 0;
            player.expToNextLevel = userStats[1].expToNextLevel;

            player.transform.position = Vector3.zero;
            player.transform.rotation = Quaternion.identity;
            SaveData();
        }
        else
        {
            string loadedJson = File.ReadAllText(path);
            data = JsonUtility.FromJson<SaveData>(loadedJson);

            player.level = data.level;
            player.hp = data.hp;
            player.defence = data.defence;
            player.currentExp = data.currentExp;
            player.expToNextLevel = userStats[data.level].expToNextLevel;

            player.transform.position = new Vector3(data.pos.x, data.pos.y, data.pos.z);
            player.transform.rotation = Quaternion.Euler(data.rot.x, data.rot.y, data.rot.z);

            Debug.Log("Successfully Loaded");
        }
    }
}

[Serializable]
public class SaveData
{
    public Vector3 pos;
    public Vector3 rot;
    public float hp;
    public float defence;
    public int level;
    public int currentExp;
}
