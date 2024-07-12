using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public AbilityData data;
    public virtual void DoAbility()
    {
        GameManager.Instance._class.activatedAbilityDict.Add(data.Key, this);
        Debug.Log($"Name : {data.AbilityName}, Grade : {data.AbilityGrade} added and activated");
    }
}
