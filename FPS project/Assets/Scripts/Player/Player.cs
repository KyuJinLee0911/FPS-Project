using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using FPS.Control;
using UnityEngine;

public class Player : Creature
{
    public PlayerClassData classData;
    public Transform mainSkillParent;
    public Transform subSkillParent;
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

    // 최초 플레이어 생성 시, 클래스 변경 시, 게임을 클리어하고 다시 거점으로 돌아올 때 호출
    // 플레이어의 레벨을 1로, 스탯을 1레벨 기준으로 초기화
    public override void InitCreature()
    {
        level = 1;
        Stat stat = GameManager.Instance._data.userStats[level];
        hp = stat.hp;
        maxHp = stat.hp;
        exp = 0;
        expToNextLevel = stat.expToNextLevel;
        autoCriticalMagnification = 1.75f;
        autoCriticalRate = 0.1f;
        defence = stat.defence;

        Debug.Log($"User created, level : {level}, hp : {hp}, defence : {defence}");
        isDead = false;

        Debug.Log($"{mainSkillParent.childCount}");
        if (mainSkillParent.childCount == 0)
            mainSkill = Instantiate(classData.mainSkill, mainSkillParent);
        else
            mainSkill = mainSkillParent.GetComponentInChildren<Skill>();
        
        if (subSkillParent.childCount == 0)
            subSkill = Instantiate(classData.subSkill, subSkillParent);
        else
            subSkill = subSkillParent.GetComponentInChildren<Skill>();

        mainSkill.Initialize();
        subSkill.Initialize();


        OnPlayerLevelUp += SetStats;
        // OnPlayerLevelUp += GainAbilityPoint;
        OnPlayerLevelUp += GameManager.Instance._class.OpenSelectAbilityUI;

        GameManager.Instance.playerFighter = transform.GetComponent<Fighter>();
    }

    public void GetHp(float value)
    {
        hp += value;
        if(hp > maxHp)
            hp = maxHp;
    }

    public void SetPlayerPosition()
    {
        Debug.Log("Player Position Set");
        transform.position = GameManager.Instance.startPos.position;
        transform.localRotation = GameManager.Instance.startPos.localRotation;
    }
    void Awake()
    {
        if (GameManager.Instance.player == null)
        {
            GameManager.Instance.player = this;
            GameManager.Instance.controller = GetComponent<PlayerController>();
        }
        else
        {
            if (GameManager.Instance.player != this)
                Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    void OnEnable()
    {
        InitCreature();
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
        GameObject oldMainSkill = mainSkill.gameObject;
        GameObject oldSubSkill = subSkill.gameObject;
        oldMainSkill.transform.SetParent(null);
        oldSubSkill.transform.SetParent(null);
        Destroy(oldMainSkill);
        Destroy(oldSubSkill);
        classData = GameManager.Instance._class.playerClassDatas[index];
        GameManager.Instance._class.currentClass = classData;
        GameManager.Instance._data.Init();
        InitCreature();
        GameManager.Instance.hud.Init();
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

        OnPlayerLevelUp();
    }

    // public void GainAbilityPoint()
    // {
    //     abilityPoint++;
    // }

    public void SetStats()
    {

        Dictionary<int, Stat> statDict = GameManager.Instance._data.userStats;
        // 레벨업 시 증가하는 최대 hp만큼 현재 hp에 더해줌
        float additionalHp = statDict[level].hp - maxHp;
        hp += additionalHp;
        maxHp = statDict[level].hp;
        defence = statDict[level].defence;
        expToNextLevel = statDict[level].expToNextLevel;
        Debug.Log($"Level Up! Current Level : {level}, current exp : {exp}");
    }
}
