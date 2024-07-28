using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Skill : MonoBehaviour
{
    public float coolTime;
    public float currentCoolTime;
    public float skillDamage;
    public float skillRange;
    public Vector3 targetLocation;
    public string skillName;
    public GameObject skillCoolTimeUI;
    public Sprite skillSprite;
    public Image uiSkillImage;
    public abstract void Initialize();
    public abstract void DoSkill();
    public void CountSkillCooltime()
    {
        if(currentCoolTime > 0)
        {
            currentCoolTime -= Time.deltaTime;
        }

        if(currentCoolTime <= 0 && skillCoolTimeUI.activeSelf)
        {
            skillCoolTimeUI.SetActive(false);
        }
    }

    public bool IsReady()
    {
        return currentCoolTime <= 0;
    }
}
