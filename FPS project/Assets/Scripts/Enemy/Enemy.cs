using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Enemy : Creature
{
    [SerializeField] Transform targetTransform;
    [SerializeField] float moveSpeed;
    [SerializeField] float attackRange;
    [SerializeField] float chaseRange;
    [SerializeField] private bool isInsideBattleZone;
    [SerializeField] Animator animator;
    bool isMoving;
    private BTSelector root;
    private Fighter fighter;
    private event Action OnGetHit;



    public override void Initialize()
    {
        targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
        int playerLevel = GameManager.Instance.player.level;
        fighter = GetComponent<Fighter>();
        hp = GameManager.Instance._data.enemyStats[playerLevel].hp;
        totalHp = hp;
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
    }

    private void Update()
    {
        root.Evaluate();
        animator.SetBool("isMoving", isMoving);
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
        Debug.Log(instigator.GetComponent<IStat>().exp);
        if(isInsideBattleZone && GameManager.Instance.battleZoneCtrl != null)
        {
            GameManager.Instance.battleZoneCtrl.SubtractEnemyCount(this);
        }
    }


    public override void TakeDamage(GameObject instigator, float damage)
    {
        if (isDead) return;
        OnGetHit();
        base.TakeDamage(instigator, damage);
    }

    private bool IsPlayerInRange()
    {
        float distance = Vector3.Distance(transform.position, targetTransform.position);
        return distance <= attackRange;
    }

    private bool IsPlayerInChaseRange()
    {
        float distance = Vector3.Distance(transform.position, targetTransform.position);
        return distance <= chaseRange;
    }
    private BTNodeState Attack()
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

    private BTNodeState Chase()
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
