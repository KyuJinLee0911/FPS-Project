using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpenedWits : Ability
{
    public override void DoAbility()
    {
        base.DoAbility();
        GameManager.Instance.playerFighter.additionalCriticalMultiples += 0.1f;
        GameManager.Instance.playerFighter.currentWeapon.AddCriticalMultiples(GameManager.Instance.playerFighter.additionalCriticalMultiples);
    }

    public override void RemoveAbility()
    {
        GameManager.Instance.playerFighter.additionalCriticalMultiples -= 0.1f;
    }
}
