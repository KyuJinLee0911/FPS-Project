using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickTrigger : Ability
{
    public override void DoAbility()
    {
        base.DoAbility();
        GameManager.Instance.playerFighter.additionalFireRateMagnifier += 0.15f;
        GameManager.Instance.playerFighter.currentWeapon.ChangeFireRate(GameManager.Instance.playerFighter.additionalFireRateMagnifier);
    }
}
