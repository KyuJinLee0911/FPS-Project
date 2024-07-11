using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitCode
{
    UC_GUARDIAN,
    UC_COMMANDO,
    UC_WALKING,
    UC_FLYING,
    UC_BOSS
}

public class Player : Creature
{
    public UnitCode unitCode;
    public Status status;
    public PlayerClassData classData;
    public Skill mainSkill;
    public Skill subSkill;
    private int abilityPoint;

    public List<ItemData> itemList = new List<ItemData>();

    public event Action OnPlayerDamage;
    public event Action OnPlayerDie;
    public event Action OnPlayerLevelUp;

    // 처음 시작할 때, 캐릭터 클래스 변경 시에만 호출
    // 캐릭터 클래스는 게임 플레이 도중 변경할 수 없고, 거점에서만 변경이 가능하기 때문에
    // 모든 스탯을 초기값으로 리셋해도 무방함
    public override void Initialize()
    {
        status = new Status();
        status = status.SetUnitStatus(unitCode);
        hp = status.hp;
        defence = status.defence;
        level = 1;
        autoCriticalRate = 0.1f;
        autoCriticalMagnification = 1.75f;
        isDead = false;
        mainSkill = classData.mainSkill;
        mainSkill.Initialize();
        subSkill = classData.subSkill;
        subSkill.Initialize();

        OnPlayerLevelUp += GainAbilityPoint;
        OnPlayerLevelUp += GameManager.Instance.OpenSelectAbilityUI;
    }

    private void Start()
    {
        Initialize();
        GameManager.Instance.currentClass = classData;
        {
            
        }
    }

    public override void TakeDamage(GameObject instigator, float damage)
    {
        base.TakeDamage(instigator, damage);
    }

    public override void Die()
    {
        base.Die();
        OnPlayerDie();
    }

    public void ChangePlayerClass(int index)
    {
        classData = GameManager.Instance.playerClassDatas[index];
        GameManager.Instance.currentClass = classData;
        unitCode = classData.unitCode;
        Initialize();
    }

    public void PlayerLevelUp()
    {
        level++;
        Debug.Log($"Level Up! Current Level : {level}");
        OnPlayerLevelUp();
    }

    public void GainAbilityPoint()
    {
        abilityPoint++;
    }
}
