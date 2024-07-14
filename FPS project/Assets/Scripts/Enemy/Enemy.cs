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
    private BTSelector root;
    private Fighter fighter;
    private event Action OnGetHit;

    public override void Initialize()
    {
        targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        fighter = GetComponent<Fighter>();
        hp = 100;
        defence = 0.1f;
        level = 1;
        autoCriticalRate = 0.0f;
        autoCriticalMagnification = 1f;
        isDead = false;

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
    }

    private void Start()
    {
        Initialize();
    }
    public override void Die()
    {
        base.Die();
        fighter.isWeaponFire = false;
    }

    public override void TakeDamage(GameObject instigator, float damage)
    {
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
        if(isDead) return BTNodeState.Failure;
        
        transform.LookAt(targetTransform);
        fighter.isWeaponFire = true;
        fighter.Fire();
        Debug.Log("Attack Action");
        return BTNodeState.Success;
    }

    private BTNodeState Chase()
    {
        if(isDead) return BTNodeState.Failure;
        if(fighter.isWeaponFire) fighter.isWeaponFire = false;
        transform.LookAt(targetTransform);
        Debug.Log("Chase Action");
        transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, moveSpeed * Time.deltaTime);
        return BTNodeState.Running;
    }

    private BTNodeState Scout()
    {
        if(isDead) return BTNodeState.Failure;
        if(fighter.isWeaponFire) fighter.isWeaponFire = false;

        Debug.Log("Scout Action");
        return BTNodeState.Running;
    }

    void ShowHitEffect()
    {
        // 피격 효과
        Debug.Log($"Effect! {gameObject.name}");
    }
}
