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
        if (other.GetComponent<IDamageable>() == null)
        {
            Destroy(gameObject);
            return;
        }
        if (other.GetComponent<IDamageable>().isDead) return;

        // 약점 공격 시
        // 무기의 크리티컬 확률을 따르는 크리티컬 대미지
        // Physics.OverlapBox 이용해서 구현

        // 약점이 아닌 곳 공격 시
        // 공격자의 자동 크리티컬 확률과 배율을 따르는 크리티컬 대미지
        other.GetComponent<IDamageable>().TakeDamage(instigator, instigator.GetComponent<Fighter>().CalculateAutoCriticalDamage(projectileDamage));

        Debug.Log(instigator.name);
        if (isExplode)
            Debug.Log("BOOM!");
        // Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
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
            Destroy(gameObject);
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
