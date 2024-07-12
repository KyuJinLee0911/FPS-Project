using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Projectile : MonoBehaviour
{
    // IDamageable target;
    [SerializeField] float projectileSpeed = 6f;
    [SerializeField] float projectileDamage = 0f;
    // 약점 공격 시의 크리티컬 배율 (무기의 크리티컬 배율을 따라감)
    [SerializeField] float criticalMagnification = 1f;
    // 약점 아닌 곳을 공격했을 때의 크리티컬 배율 (instigator의 크리티컬 배율을 따라감)
    [SerializeField] float autoCriticalMagnification = 1f;
    float projectileHoamingRange = 5f;
    [SerializeField] bool isHoaming = false;
    [SerializeField] bool isExplode = false;
    [SerializeField] GameObject explosionEffect;
    public float range;
    [SerializeField] float maxLifeTime;
    float currentLifeTime;
    GameObject instigator = null;

    private void OnTriggerEnter(Collider other)
    {
        // Projectile은 HitBox레이어
        // HitBox 레이어는 HurtBox 레이어와만 상호작용이 가능하도록 설정해 두었기 때문에
        // other 콜라이더는 Enemy 오브젝트의 자식 오브젝트로 들어가있는 HurtBox레이어를 가진 Head와 Body 오브젝트와 상호작용
        // Enemy 오브젝트의 root 트랜스폼에 있는 IDamageable을 사용하기 위해 TriggerEnter시에 rootTransform을 따로 할당해줌
        Transform rootTransform;
        rootTransform = other.transform.root;
        IDamageable damageable = rootTransform.GetComponent<IDamageable>();
        CheckWeakness checkWeakness = other.GetComponent<CheckWeakness>();

        if (damageable == null ||damageable.isDead || checkWeakness == null)
        {
            currentLifeTime = 0;
            GameManager.Instance._pool.ReturnObj(instigator.name, this);
            return;
        }

        damageable.TakeDamage(instigator, instigator.GetComponent<Fighter>().CalculateDamage(projectileDamage, checkWeakness.damageType));

        if (isExplode)
            Debug.Log("BOOM!");
        // Instantiate(explosionEffect, transform.position, Quaternion.identity);

        currentLifeTime = 0;
        GameManager.Instance._pool.ReturnObj(instigator.name, this);
    }

    private void Start()
    {
        maxLifeTime = range / projectileSpeed;
        Debug.Log(maxLifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        DestroyProjectileByTime();
        MoveToTarget();
        if (isHoaming)
            HoamingTarget();
    }

    void MoveToTarget()
    {
        transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
    }

    // 대미지와 크리티컬 배율(무기의), 공격자를 무기로부터 전달받음
    // 크리티컬 확률은 받아오지 않아도 됨
    // 약점을 공격하면 무조건 크리티컬이기 때문
    public void SetDamage(float damage, float criticalMagnification, GameObject instigator)
    {
        projectileDamage = damage;
        this.criticalMagnification = criticalMagnification;
        this.instigator = instigator;
    }

    // 일정 거리 이상 날아갔을때 비활성화
    void DestroyProjectileByTime()
    {
        currentLifeTime += Time.deltaTime;

        if (currentLifeTime >= maxLifeTime)
        {
            currentLifeTime = 0;
            GameManager.Instance._pool.ReturnObj(instigator.name, this); 
        }
            
    }

    // SphereCast를 통해 목표 탐색 후 가장 가까운 목표를 향해 유도
    void HoamingTarget()
    {
        RaycastHit[] sphereCastData;
        sphereCastData = Physics.SphereCastAll(transform.position, projectileHoamingRange, Vector3.forward, 0);
        float closestDistance = float.MaxValue;
        GameObject closestEnemy = null;

        foreach (var data in sphereCastData)
        {
            if (data.distance < closestDistance)
            {
                if (data.collider.gameObject.GetComponent<Enemy>() == null)
                    continue;

                closestDistance = data.distance;
                closestEnemy = data.collider.gameObject;
            }
            else
                continue;
        }

        if (closestEnemy != null)
            Debug.Log(closestEnemy.name);

        float hoamingRotateSpeed = 80f;

        // 가장 가까운 적을 바라보는 방향으로 천천히 회전
        if (closestEnemy != null)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(closestEnemy.transform.position + new Vector3(0, 0.8f, 0) - transform.position), Time.deltaTime * hoamingRotateSpeed);
        }
        else
            return;
    }

    // 오브젝트 풀링
}
