using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public AbilityData[] datas = new AbilityData[3];
    public int currentIdx;
    public virtual void DoAbility()
    {
        SelectAbility();
    }

    private void SelectAbility()
    {
        for (int i = 0; i < datas.Length; i++)
        {
            if (GameManager.Instance._class.activatedAbilityDict.ContainsKey(datas[i].Key)) continue;

            GameManager.Instance._class.activatedAbilityDict.Add(datas[i].Key, this);
            Debug.Log($"Name : {datas[i].AbilityName}, Grade : {datas[i].AbilityGrade} added and activated");
            currentIdx = i;
            break;
        }
    }
}
