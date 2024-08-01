using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTrick : Ability
{
    public override void DoAbility()
    {
        base.DoAbility();
        GameManager.Instance.player.subSkill.skillDamage *= 1.15f;
    }

    public override void RemoveAbility()
    {
        GameManager.Instance.player.subSkill.skillDamage /= 1.15f;
    }
}
