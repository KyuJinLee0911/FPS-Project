using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();
    Dictionary<string, Achievement> achievedAchievements = new Dictionary<string, Achievement>();
    [SerializeField] GameObject achievementUIObj;
    [SerializeField] Text nameTxt;
    [SerializeField] Text descriptionTxt;

    Dictionary<string, Achievement> LoadAchievementJson(string path)
    {
        Dictionary<string, Achievement> newAchievementDict = new Dictionary<string, Achievement>();
        var loadedJson = Resources.Load<TextAsset>(path);
        AchievementData myAchievement = JsonUtility.FromJson<AchievementData>(loadedJson.ToString());
        foreach (var achievement in myAchievement.achievements)
        {
            newAchievementDict.Add(achievement.key, achievement);
        }
        return newAchievementDict;
    }

    public void Init()
    {
        if (achievements.Count == 0)
            achievements = LoadAchievementJson("JSON/AchievementData");
    }

    public void Achived(string key)
    {
        if(achievedAchievements[key] == null)
            achievedAchievements.Add(key, achievements[key]);
        
        StartCoroutine(EnableAchievementUI(key));
    }

    public IEnumerator EnableAchievementUI(string key)
    {
        if(!achievementUIObj.activeSelf)
            achievementUIObj.SetActive(true);
        nameTxt.text = achievements[key].name;
        descriptionTxt.text = achievements[key].description;

        yield return new WaitForSeconds(4);

        achievementUIObj.SetActive(false);
    }

}

[Serializable]
public class Achievement
{
    public string key;
    public string name;
    public string description;
}

[Serializable]
public class AchievementData
{
    public List<Achievement> achievements;
}
