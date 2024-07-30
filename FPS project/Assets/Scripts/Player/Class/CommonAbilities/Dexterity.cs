using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dexterity : Ability
{
    public override void DoAbility()
    {
        base.DoAbility();
        GameManager.Instance.controller.moveSpeed *= 1.06f;
        
        GameManager.Instance.playerFighter.additionalReloadSpeedMagnifier -= 0.15f;
        GameManager.Instance.playerFighter.currentWeapon.ChangeReloadSpeed(GameManager.Instance.playerFighter.additionalReloadSpeedMagnifier);
    }
    
}
