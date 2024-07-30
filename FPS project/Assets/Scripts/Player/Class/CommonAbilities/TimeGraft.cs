using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeGraft : Ability
{
    public override void DoAbility()
    {
        base.DoAbility();
        GameManager.Instance.player.mainSkill.coolTime *= 0.90f;
        GameManager.Instance.player.subSkill.coolTime *= 0.90f;
    }
}
