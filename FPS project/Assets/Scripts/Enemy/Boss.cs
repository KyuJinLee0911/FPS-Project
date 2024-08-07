using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Boss : Enemy
{
    List<BossSkillData> skillDatas = new List<BossSkillData>
    {
        new BossSkillData("Skill_1", 25.0f, 1),
        new BossSkillData("Skill_2", 20.0f, 2),
        new BossSkillData("Skill_3", 15.0f, 3),
        new BossSkillData("Skill_4", 40.0f, 4),
        new BossSkillData("Skill_5", 10.0f, 5),
    };
    int highestPriority = int.MaxValue;
    BossSkillData nextSkill = null;
    // [SerializeField] UnityEngine.UI.Slider hpBar;
    [SerializeField] TextMeshProUGUI hpText;
    bool isAttacking = false;

    public GameObject resultObj;
    public override void InitCreature()
    {
        if(GameManager.Instance.boss == null)
            GameManager.Instance.boss = this;
        else
            Destroy(gameObject);

        targetTransform = GameManager.Instance.player.transform;

        int playerLevel = GameManager.Instance.player.level;
        fighter = GetComponent<Fighter>();
        animator = GetComponent<Animator>();
        maxHp = 1500;
        hp = maxHp; // 테스트용 체력
        
        defence = 0.2f;
        autoCriticalRate = 0.0f;
        autoCriticalMagnification = 1f;
        isDead = false;

        // 씬이 로딩되자마자 스킬을 쓰는 것을 방지하기 위함
        skillDatas[1].currentCoolTime = 3;
        skillDatas[2].currentCoolTime = 3;

        root = new BTSelector();

        BTSequence closeAttackSequence = new BTSequence();
        BTSequence chaseSequence = new BTSequence();
        BTSequence farAttackSequence = new BTSequence();

        BTAction closeAttackAction = new BTAction(Attack);
        BTAction chaseAction = new BTAction(Chase);
        BTAction farAttackAction = new BTAction(FarAttack);

        BTCondition playerInCloseRange = new BTCondition(IsPlayerInRange);
        BTCondition playerInChaseRange = new BTCondition(IsPlayerInChaseRange);


        root.AddChild(closeAttackSequence);
        root.AddChild(chaseSequence);
        root.AddChild(farAttackSequence);

        closeAttackSequence.AddChild(playerInCloseRange);
        closeAttackSequence.AddChild(closeAttackAction);
        chaseSequence.AddChild(playerInChaseRange);
        chaseSequence.AddChild(chaseAction);
        farAttackSequence.AddChild(farAttackAction);

        root.Evaluate();

        hpBar.maxValue = maxHp;
        hpBar.value = hp;

        GameManager.Instance.onChangeTarget.AddListener(ChangeTarget);
    }

    public override void TakeDamage(GameObject instigator, float damage)
    {
        base.TakeDamage(instigator, damage);
        // animator.SetTrigger("GetHit");
    }

    public override void Die(GameObject instigator)
    {
        if(isDead) return;
        isDead = true;
        isMoving = false;
        AddScore(enemyType);
        animator.SetTrigger("Die");
        if(!GameManager.Instance._achivement.achievedDict.ContainsKey("reboot"))
        {
            GameManager.Instance._achivement.Achived("reboot");
            GameManager.Instance._class.UnlockClass("Ranger");
        }
    }

    private void Start()
    {
        InitCreature();

        StartCoroutine(SelfHealRoutine());
    }

    private void Update()
    {
        foreach (BossSkillData skill in skillDatas)
        {
            skill.UpdateDeltaTime(Time.deltaTime);
        }

        root.Evaluate();
        animator.SetBool("isMoving", isMoving);
        hpBar.value = hp;
        hpText.text = $"{Mathf.FloorToInt(hp)}/{maxHp}";
    }

    // 근거리 스킬 (1, 5) 혹은 일반 공격 사용
    protected override BTNodeState Attack()
    {
        if (isDead) return BTNodeState.Failure;
        isMoving = false;
        // transform.LookAt(targetTransform);
        if (!isAttacking)
        {
            Vector3 targetDir = (targetTransform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f * Time.deltaTime);
        }
        // fighter.Fire();
        if (skillDatas[0].IsReady())
        {
            isAttacking = true;
            animator.SetTrigger("0");
            skillDatas[0].currentCoolTime = skillDatas[0].coolTime;
        }
        else if (skillDatas[4].IsReady())
        {
            isAttacking = true;
            animator.SetTrigger("4");
            skillDatas[4].currentCoolTime = skillDatas[4].coolTime;
        }
        else
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
        }

        Debug.Log("Attack Action");
        return BTNodeState.Success;
    }

    protected override BTNodeState Chase()
    {
        if (isDead) return BTNodeState.Failure;
        // if (fighter.isWeaponFire) fighter.isWeaponFire = false;
        isAttacking = false;
        Vector3 targetDir = (targetTransform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.6f * Time.deltaTime);
        Debug.Log("Chase Action");
        transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, moveSpeed * Time.deltaTime);
        isMoving = true;
        return BTNodeState.Running;
    }

    // 2, 3번 스킬 사용
    protected BTNodeState FarAttack()
    {
        if (!isAttacking)
        {
            Vector3 targetDir = (targetTransform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.6f * Time.deltaTime);
        }
        if (skillDatas[1].IsReady())
        {
            isAttacking = true;
            animator.SetTrigger("1");
            skillDatas[1].currentCoolTime = skillDatas[1].coolTime;
        }
        else if (skillDatas[2].IsReady())
        {
            isAttacking = true;
            animator.SetTrigger("2");
            skillDatas[2].currentCoolTime = skillDatas[2].coolTime;
        }
        else
            return BTNodeState.Failure;
        return BTNodeState.Running;
    }

    IEnumerator SelfHealRoutine()
    {
        yield return new WaitUntil(() => hp <= (maxHp * 0.5f));

        if (!skillDatas[3].IsReady())
            yield return new WaitUntil(() => skillDatas[3].IsReady() == true);
        
        if (hp <= (maxHp * 0.5f))
        {
            animator.SetTrigger("3");
            skillDatas[3].currentCoolTime = skillDatas[3].coolTime;
            StartCoroutine(SelfHealRoutine());
            yield break;
        }
    }

    public void SelfHeal()
    {
        if(hp <= 0) return;

        hp += 150;

        if(hp > maxHp)
            hp = maxHp;
    }
}

public class BossSkillData
{
    string name;
    public float coolTime;
    public float currentCoolTime;
    public int priority;

    public BossSkillData(string name, float coolTime, int priority)
    {
        this.name = name;
        this.coolTime = coolTime;
        this.priority = priority;
        currentCoolTime = 0;
    }

    public void UpdateDeltaTime(float deltaTime)
    {
        if (currentCoolTime > 0)
        {
            currentCoolTime -= deltaTime;
        }
    }

    public bool IsReady()
    {
        return currentCoolTime <= 0;
    }
}
