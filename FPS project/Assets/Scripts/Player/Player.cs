using System;
using System.Collections;
using System.Collections.Generic;
using FPS.Control;
using UnityEngine;

public class Player : Creature
{
    public PlayerClassData classData;
    public Transform playerSkillsParent;
    public Skill mainSkill;
    public Skill subSkill;
    private int abilityPoint;
    public int currentTotalExp;
    public int expToNextLevel;
    public Collider playerHurtBox;
    public HitBox hitbox;

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

        // 처음 플레이어를 활성화 할 때는 메인 스킬을 새로 만들고
        // 클래스 변경 시에는 이미 만들어진 스킬을 변경된 스킬로 바꾼다
        if (mainSkill == null)
            mainSkill = Instantiate(classData.mainSkill, playerSkillsParent);
        else
            mainSkill = classData.mainSkill;
        mainSkill.Initialize();

        if (subSkill == null)
            subSkill = Instantiate(classData.subSkill, playerSkillsParent);
        else
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
        Initialize();
    }
    private void Update()
    {
        if (exp >= expToNextLevel)
        {
            exp -= expToNextLevel;
            PlayerLevelUp();
        }

        if (!mainSkill.IsReady()) mainSkill.CountSkillCooltime();
        if (!subSkill.IsReady()) subSkill.CountSkillCooltime();
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
