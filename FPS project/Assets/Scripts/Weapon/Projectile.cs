using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class Projectile : MonoBehaviour
{
    // IDamageable target;
    [Header("Projectile Data")]
    [SerializeField] float projectileSpeed = 15f;
    [SerializeField] float projectileDamage = 0f;
    // 약점 공격 시의 크리티컬 배율 (무기의 크리티컬 배율을 따라감)
    [SerializeField] float criticalMagnification = 1f;
    // 약점 아닌 곳을 공격했을 때의 크리티컬 배율 (instigator의 크리티컬 배율을 따라감)
    [SerializeField] float autoCriticalMagnification = 1f;
    float projectileHoamingRange = 5f;
    [SerializeField] bool isHoaming = false;
    [SerializeField] bool isExplode = false;
    public float range;
    GameObject instigator = null;
    [SerializeField] float currentLifeTime = 0.0f;

    [Header("Particle System and Effect")]
    [SerializeField] protected float hitOffset = 0f;
    [SerializeField] protected GameObject hit;
    [SerializeField] protected ParticleSystem hitPS;
    [SerializeField] protected GameObject flash;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Collider col;
    [SerializeField] protected Light lightSourse;
    [SerializeField] protected ParticleSystem projectilePS;

    [Header("Explosion")]
    [SerializeField] GameObject explosionEffect;
    [SerializeField] private float explosionForce;
    [SerializeField] private float explosionRadious;
    [SerializeField] private float expUpwardModifier;

    bool isParticleStart = false;

    private void OnCollisionEnter(Collision other)
    {
        // 폭발하는 투사체의 경우, 폭발은 triggerEnter시에 항상 발생
        if (isExplode)
            Explode();
        if (other.collider.CompareTag(instigator.tag))
        {
            GameManager.Instance._pool.ReturnObj(instigator.name, this);
            return;
        }

        EnableVfx(other);

        Transform rootTransform = other.collider.transform.root;
        Fighter fighter = instigator.GetComponent<Fighter>();
        IDamageable damageable = rootTransform.GetComponent<IDamageable>();


        // 데미지를 입힐 수 없는 오브젝트이거나, 이미 죽은 적이거나, 약점을 판단하는 checkweakness가 없는 경우
        // 오브젝트를 풀에 바로 리턴
        if (damageable == null || damageable.isDead)
        {
            GameManager.Instance._pool.ReturnObj(instigator.name, this);
            return;
        }

        WeaponData weaponData = fighter.CurrentWeapon;

        if (!isExplode)
            // 넉백
            weaponData.ApplyImpack(Vector3.zero, 0);

        float _damage = 0;
        if (other.collider.gameObject.layer == 8)
            _damage = fighter.CalculateDamage(projectileDamage, DamageType.DT_NORMAL);
        else if (other.collider.gameObject.layer == 10)
            _damage = fighter.CalculateDamage(projectileDamage, DamageType.DT_WEAKNESS);

        Debug.Log(_damage);
        if (_damage != 0)
            damageable.TakeDamage(instigator, _damage);

        GameManager.Instance._pool.ReturnObj(instigator.name, this);
    }

    private void EnableVfx(Collision other)
    {
        // rb.constraints = RigidbodyConstraints.FreezeAll;
        if (lightSourse != null)
            lightSourse.enabled = false;
        col.enabled = false;
        projectilePS.Stop();
        projectilePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        ContactPoint contact = other.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal * hitOffset;

        if (hit != null)
        {
            hit.transform.rotation = rot;
            hit.transform.position = pos;
            hit.transform.LookAt(contact.point + contact.normal);
            hitPS.Play();
        }
    }

    private void Start()
    {
        currentLifeTime = 0;
        isParticleStart = true;
    }

    private void OnEnable()
    {
        currentLifeTime = 0;
        if (isParticleStart)
        {
            if (lightSourse != null)
                lightSourse.enabled = true;

            col.enabled = true;
            // rb.constraints = RigidbodyConstraints.None;
        }
    }

    private void OnDisable()
    {
        rb.angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= 5.0f)
            GameManager.Instance._pool.ReturnObj(instigator.name, this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Debug.Log(rb.angularVelocity);
        MoveToTarget();
        if (isHoaming)
            HoamingTarget();
    }

    void MoveToTarget()
    {
        transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        // rb.velocity = transform.forward * projectileSpeed;    
    }

    // 대미지와 크리티컬 배율(무기의), 공격자를 무기로부터 전달받음
    // 크리티컬 확률은 받아오지 않아도 됨
    // 약점을 공격하면 무조건 크리티컬이기 때문
    public void SetProjectileDamage(float damage, float criticalMagnification, GameObject instigator)
    {
        projectileDamage = damage;
        this.criticalMagnification = criticalMagnification;
        this.instigator = instigator;
    }

    // 일정 거리 이상 날아갔을때 비활성화
    protected IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (gameObject.activeSelf)
            GameManager.Instance._pool.ReturnObj(instigator.name, this);
        yield break;
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

    void Explode()
    {
        Collider[] colliders;

        colliders = Physics.OverlapSphere(transform.position, 20);

        foreach (var col in colliders)
        {
            if (col.attachedRigidbody == null) continue;
            if (col.CompareTag("Player")) continue;

            col.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadious, expUpwardModifier);
        }
    }
}
