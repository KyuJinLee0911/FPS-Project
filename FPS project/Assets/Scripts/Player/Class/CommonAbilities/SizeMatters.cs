using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeMatters : Ability
{
    public override void DoAbility()
    {
        base.DoAbility();
        GameManager.Instance.playerFighter.additionalMagMagnifier += 0.15f;
        GameManager.Instance.playerFighter.currentWeapon.AddMag(GameManager.Instance.playerFighter.additionalMagMagnifier);
    }

    public override void RemoveAbility()
    {
        GameManager.Instance.playerFighter.additionalMagMagnifier -= 0.15f;
    }
}
