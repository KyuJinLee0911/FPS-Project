using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianSubSkill : Skill
{

    public override void Initialize()
    {
        // uiSkillImage.sprite = skillSprite;
        skillName = "bash";
        coolTime = 10;
        currentCoolTime = 0;
        skillRange = 5;
        skillDamage = 55;
    }

    public override void DoSkill()
    {
        if(!IsReady()) return;
        skillCoolTimeUI.SetActive(true);
        // if(Vector3.Distance(transform.position, targetLocation) > skillRange) return;
        currentCoolTime = coolTime;
        Debug.Log(skillName);
    }


}
