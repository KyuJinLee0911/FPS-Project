using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianSubSkill : Skill
{

    private void Start()
    {
        Initialize();
    }
    public override void Initialize()
    {
        skillName = "bash";
        coolTime = 10;
        skillRange = 5;
        skillDamage = 55;
    }

    public override void DoSkill()
    {
        // if(coolTime > 0) return;
        // if(Vector3.Distance(transform.position, targetLocation) > skillRange) return;

        Debug.Log(skillName);
    }


}
