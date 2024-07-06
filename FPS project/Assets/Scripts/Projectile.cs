using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // IDamageable target;
    [SerializeField] float projectileSpeed = 6f;
    [SerializeField] float projectileDamage = 0f;
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
        if (other.GetComponent<IDamageable>().IsDead) return;

        other.GetComponent<IDamageable>().TakeDamage(instigator, projectileDamage);
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

    public void SetDamage(float damage)
    {
        projectileDamage = damage;
        // this.instigator = instigator;
    }

    // 일정 거리 이상 날아갔을때 비활성화
    void DestroyProjectileByTime()
    {
        currentLifeTime += Time.deltaTime;

        if (currentLifeTime >= maxLifeTime)
            Destroy(gameObject);
    }

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

    // 추가 예정
    // 유도기능
    // - 원래대로 앞을 향해 날아가다가 일정 범위 내에 적이 포착되면 해당 방향을 향해 날아가도록
    // 데미지 입을때 누가 공격하는지 알도록 instigator 추가 (경험치 획득을 위해)

    // 오브젝트 풀링
}
