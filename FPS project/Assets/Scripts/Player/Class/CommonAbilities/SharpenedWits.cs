using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpenedWits : Ability
{
    public override void DoAbility()
    {
        base.DoAbility();
        
        GameManager.Instance.playerFighter.currentWeapon.AddCriticalMultiples(0.1f);
    }
}
