using System;
using System.Collections;
using System.Collections.Generic;
using FPS.Control;
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
    public PlayerClassData classData;
    public Skill mainSkill;
    public Skill subSkill;
    private int abilityPoint;
    public int currentTotalExp;
    public int expToNextLevel;

    public event Action OnPlayerDamage;
    public event Action OnPlayerDie;
    public event Action OnPlayerLevelUp;

    // 스테이지 별로 플레이어 오브젝트 존재
    // 각 스테이지가 시작될때마다 호출
    public override void Initialize()
    {
        if (GameManager.Instance.player == null)
        {
            GameManager.Instance.player = this;
            GameManager.Instance.controller = GetComponent<PlayerController>();
        }

        GameManager.Instance._data.LoadIngameData();

        Debug.Log($"User created, level : {level}, hp : {hp}, defence : {defence}");
        isDead = false;
        mainSkill = classData.mainSkill;
        mainSkill.Initialize();
        subSkill = classData.subSkill;
        subSkill.Initialize();

        OnPlayerLevelUp += GainAbilityPoint;
        OnPlayerLevelUp += GameManager.Instance._class.OpenSelectAbilityUI;
        OnPlayerLevelUp += SetStats;

        // SetPlayerPosition();
    }

    void SetPlayerPosition()
    {
        Debug.Log("Player Position Set");
        transform.position = GameManager.Instance.startPos.position;
        transform.localRotation = GameManager.Instance.startPos.localRotation;
    }

    void OnEnable()
    {
        Initialize();
    }

    private void Start()
    {
        SetPlayerPosition();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance.player != null)
            GameManager.Instance.player = null;
    }

    public override void TakeDamage(GameObject instigator, float damage)
    {
        base.TakeDamage(instigator, damage);
    }

    public override void Die(GameObject instigator)
    {
        base.Die(instigator);
        OnPlayerDie();
    }

    public void ChangePlayerClass(int index)
    {
        classData = GameManager.Instance._class.playerClassDatas[index];
        GameManager.Instance._class.currentClass = classData;
        GameManager.Instance._data.Init();
        unitCode = classData.unitCode;
        Initialize();
    }
    private void Update()
    {
        if (exp >= expToNextLevel)
        {
            exp -= expToNextLevel;
            PlayerLevelUp();
        }
    }

    public void PlayerLevelUp()
    {
        level++;
        // Debug.Log($"Level Up! Current Level : {level}");
        OnPlayerLevelUp();
    }

    public void GainAbilityPoint()
    {
        abilityPoint++;
    }

    public void SetStats()
    {
        Dictionary<int, Stat> statDict = GameManager.Instance._data.userStats;
        hp = statDict[level].hp;
        defence = statDict[level].defence;
        exp -= expToNextLevel;
        expToNextLevel = statDict[level].expToNextLevel;
    }
}
