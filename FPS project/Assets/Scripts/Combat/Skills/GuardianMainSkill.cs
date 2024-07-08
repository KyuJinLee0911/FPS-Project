using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianMainSkill : Skill
{
    public override void DoSkill()
    {
        // if(coolTime > 0) return;
        Debug.Log($"{skillName}, {coolTime}, {skillRange}, {skillDamage}");
    }

    public override void Initialize()
    {
        skillName = "Bastion";
        coolTime = 8;
        skillRange = 3;
        skillDamage = 0;
    }
}
