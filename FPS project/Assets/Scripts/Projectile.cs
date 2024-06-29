using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // IDamageable target;
    [SerializeField] float projectileSpeed = 6f;
    [SerializeField] float projectileDamage = 0f;
    [SerializeField] bool isHoaming = false;
    [SerializeField] bool isExplode = false;
    [SerializeField] GameObject explosionEffect;
    public float range;
    float maxLifeTime;
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
        MoveToTarget();
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

    // 추가 예정
    // 유도기능
    // - 원래대로 앞을 향해 날아가다가 일정 범위 내에 적이 포착되면 해당 방향을 향해 날아가도록
    // 데미지 입을때 누가 공격하는지 알도록 instigator 추가 (경험치 획득을 위해)
    // 일정 거리 이상 날아갔을때 비활성화
    // 오브젝트 풀링
}
