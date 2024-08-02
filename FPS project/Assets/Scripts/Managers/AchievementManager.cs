using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();
    public Dictionary<string, Achievement> achievedDict = new Dictionary<string, Achievement>();
    [SerializeField] GameObject achievementUIObj;
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] TextMeshProUGUI descriptionTxt;
    string path;
    
    public void Init()
    {
        path = Path.Combine(Application.dataPath + "/Resources/SaveData/", "achievementData.json");
        if (achievements.Count == 0)
            achievements = GameManager.Instance._data.LoadJson<string, Achievement>("JSON/AchievementData");
        if (achievedDict.Count == 0)
            LoadAchievedData();
    }

    #region Add Achievement Data and Activate UI
    public void Achived(string key)
    {
        if (achievedDict.ContainsKey(key)) return;

        achievedDict.Add(key, achievements[key]);
        SaveAchievedData();
        StartCoroutine(EnableAchievementUI(key));
    }

    public IEnumerator EnableAchievementUI(string key)
    {
        if (!achievementUIObj.activeSelf)
            achievementUIObj.SetActive(true);
        nameTxt.text = achievements[key].name;
        descriptionTxt.text = achievements[key].description;

        yield return new WaitForSeconds(4);

        achievementUIObj.SetActive(false);
    }
    #endregion

    #region Achieved Data Save
    public void SaveAchievedData()
    {
        List<Achievement> achievedList = new List<Achievement>();
        foreach (KeyValuePair<string, Achievement> achievement in achievedDict)
        {
            achievedList.Add(achievement.Value);
        }
        JsonWrapper<Achievement> newData = new JsonWrapper<Achievement>(achievedList);
        string _listToJson = JsonUtility.ToJson(newData, true);
        
        File.WriteAllText(path, _listToJson);
        Debug.Log("Successfully Saved Achievement Data");
    }

    public void LoadAchievedData()
    {
        
        if(!File.Exists(path))
        {
            Debug.Log("You achieved nothing yet");
            return;
        }
        var jsonText = Resources.Load<TextAsset>("SaveData/achievementData");
        JsonWrapper<Achievement> myAchievement = JsonUtility.FromJson<JsonWrapper<Achievement>>(jsonText.ToString());
        foreach (Achievement achiv in myAchievement.datas)
        {
            if (achievedDict.ContainsKey(achiv.key))
                continue;

            achievedDict.Add(achiv.key, achiv);
            Debug.Log($"{achiv.key} has added to achieved Dictionary");
        }
    }
    #endregion
}

[Serializable]
public class Achievement : Data<string>
{
    public Achievement(string key)
    {
        this.key = key;
    }
    public string name;
    public string description;
}
