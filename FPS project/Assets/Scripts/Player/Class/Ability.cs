using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public AbilityData data;
    public int currentIdx = 0;
    public virtual void DoAbility()
    {
        GameManager.Instance._class.activatedAbilityDict.Add(data.Key, this);
    }

}
