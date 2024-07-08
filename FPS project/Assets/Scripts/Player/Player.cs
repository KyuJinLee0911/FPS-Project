using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Media;
using UnityEngine;

public enum UnitCode
{
    UC_GUARDIAN,
    UC_COMMANDO,
    UC_WALKING,
    UC_FLYING,
    UC_BOSS
}

public class Player : Creature, IDamageable
{
    public UnitCode unitCode;
    public Status status;
    public PlayerClassData classData;
    public float hp { get; set; }
    public float defence { get; set; }
    public bool isDead { get; set; }

    public Skill mainSkill;
    public Skill subSkill;
    
    public event Action OnPlayerDamage;
    public event Action OnPlayerDie;

    public void Initialize()
    {
        status = new Status();
        status = status.SetUnitStatus(unitCode);
        hp = status.hp;
        defence = status.defence;
        isDead = false;
        mainSkill = classData.mainSkill;
        mainSkill.Initialize();
        subSkill = classData.subSkill;
        subSkill.Initialize();
    }

    private void Start()
    {
        Initialize();
    }

    public void TakeDamage(GameObject instigator, float damage)
    {
        Debug.Log("ouch!");
        // 캐릭터의 방어율이 100% 이하일때는 기본 데미지에서 방어율만큼의 데미지를 제외하고 차감, 100% 이상일때는 데미지가 1만 들어가도록 함
        float damageRate = defence < 1 ? 1 - defence : 1 / damage;
        hp -= damage * damageRate;
        // OnPlayerDamage();
        if (hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        OnPlayerDie();
    }

    public void ChangePlayerClass(int index)
    {
        classData = GameManager.Instance.playerClassDatas[index];
        unitCode = classData.unitCode;
        Initialize();
    }

}
