using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyType
{
    ET_NORMAL = 0,
    ET_ELITE = 1,
    ET_BOSS = 2
}

public class Enemy : Creature
{
    public Transform targetTransform;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float chaseRange;
    [SerializeField] private bool isInsideBattleZone;
    [SerializeField] protected Animator animator;
    [SerializeField] protected GameObject hpBarParent;
    [SerializeField] protected Slider hpBar;
    [SerializeField] protected EnemyType enemyType;
    protected bool isMoving;
    protected BTSelector root;
    protected Fighter fighter;
    protected event Action OnGetHit;

    public override void Initialize()
    {
        targetTransform = GameManager.Instance.player.transform;

        int playerLevel = GameManager.Instance.player.level;
        fighter = GetComponent<Fighter>();
        hp = GameManager.Instance._data.enemyStats[playerLevel].hp;
        maxHp = hp;
        defence = GameManager.Instance._data.enemyStats[playerLevel].defence;
        level = playerLevel;
        exp = GameManager.Instance._data.enemyStats[playerLevel].expToNextLevel;
        autoCriticalRate = 0.0f;
        autoCriticalMagnification = 1f;
        isDead = false;
        isMoving = false;

        root = new BTSelector();
        BTSequence attackSequence = new BTSequence();
        BTSequence chaseSequence = new BTSequence();
        BTSequence scoutSequence = new BTSequence();

        BTAction attackAction = new BTAction(Attack);
        BTAction chaseAction = new BTAction(Chase);
        BTAction scoutAction = new BTAction(Scout);

        BTCondition playerInRange = new BTCondition(IsPlayerInRange);
        BTCondition playerInChaseRange = new BTCondition(IsPlayerInChaseRange);

        root.AddChild(attackSequence);
        root.AddChild(chaseSequence);
        root.AddChild(scoutSequence);
        attackSequence.AddChild(playerInRange);
        attackSequence.AddChild(attackAction);
        chaseSequence.AddChild(playerInChaseRange);
        chaseSequence.AddChild(chaseAction);
        scoutSequence.AddChild(scoutAction);

        root.Evaluate();

        OnGetHit += ShowHitEffect;
        GameManager.Instance.onChangeTarget.AddListener(ChangeTarget);
        if (hpBar != null)
        {
            hpBar.maxValue = maxHp;
            hpBar.value = hp;
        }
    }

    public void ChangeTarget(Transform newTarget)
    {
        targetTransform = newTarget;
    }

    private void Update()
    {
        root.Evaluate();
        animator.SetBool("isMoving", isMoving);

        if (hpBar != null)
            hpBar.value = hp;
    }

    private void Start()
    {
        Initialize();
    }
    public override void Die(GameObject instigator)
    {
        // if (isDead) return;
        base.Die(instigator);
        fighter.isWeaponFire = false;
        instigator.GetComponent<IStat>().exp += exp;
        isMoving = false;
        animator.SetTrigger("Die");
        if (isInsideBattleZone && GameManager.Instance.battleZoneCtrl != null)
        {
            GameManager.Instance.battleZoneCtrl.SubtractEnemyCount(this);
        }

        if (enemyType != EnemyType.ET_BOSS)
        {
            hpBarParent.SetActive(false);
            StartCoroutine(HideDeadBody());
        }
    }

    IEnumerator HideDeadBody()
    {

        yield return new WaitForSeconds(3.0f);

        gameObject.SetActive(false);
    }


    public override void TakeDamage(GameObject instigator, float damage)
    {
        if (hp == maxHp && hpBar != null)
        {
            hpBarParent.SetActive(true);
        }
        if (isDead) return;
        // OnGetHit();
        base.TakeDamage(instigator, damage);
    }

    protected virtual bool IsPlayerInRange()
    {
        float distance = Vector3.Distance(transform.position, targetTransform.position);
        return distance <= attackRange;
    }

    protected virtual bool IsPlayerInChaseRange()
    {
        float distance = Vector3.Distance(transform.position, targetTransform.position);
        return distance <= chaseRange;
    }
    protected virtual BTNodeState Attack()
    {
        if (isDead) return BTNodeState.Failure;
        Transform gunTransform = fighter.GunPosition.GetChild(0);
        isMoving = false;
        transform.LookAt(targetTransform);
        gunTransform.LookAt(targetTransform.position + new Vector3(0, 1, 0));
        fighter.isWeaponFire = true;
        fighter.Fire();
        Debug.Log("Attack Action");
        return BTNodeState.Success;
    }

    protected virtual BTNodeState Chase()
    {
        if (isDead) return BTNodeState.Failure;
        if (fighter.isWeaponFire) fighter.isWeaponFire = false;
        transform.LookAt(targetTransform);
        Debug.Log("Chase Action");
        transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, moveSpeed * Time.deltaTime);
        isMoving = true;
        return BTNodeState.Running;
    }

    private BTNodeState Scout()
    {
        if (isDead) return BTNodeState.Failure;
        if (fighter.isWeaponFire) fighter.isWeaponFire = false;
        isMoving = false;
        Debug.Log("Scout Action");
        return BTNodeState.Running;
    }

    void ShowHitEffect()
    {
        animator.SetTrigger("GetHit");
        // 피격 효과
        Debug.Log($"Effect! {gameObject.name}");
    }
}
