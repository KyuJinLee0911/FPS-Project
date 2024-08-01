using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public AbilityData data;
    public int currentIdx = 0;
    public virtual void DoAbility()
    {
        GameManager.Instance._class.activatedAbility.Add(this);
    }

    public virtual void RemoveAbility()
    {
        
    }

}
