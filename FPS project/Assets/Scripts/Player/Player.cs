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

public class Player : Creature
{
    public UnitCode unitCode;
    public Status status;
    public PlayerClassData classData;
    public Skill mainSkill;
    public Skill subSkill;

    public List<ItemData> itemList = new List<ItemData>();

    public event Action OnPlayerDamage;
    public event Action OnPlayerDie;

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
    }

    private void Start()
    {
        Initialize();
    }

    public override void TakeDamage(GameObject instigator, float damage)
    {
        base.TakeDamage(instigator, damage);
    }

    public override void Die()
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
