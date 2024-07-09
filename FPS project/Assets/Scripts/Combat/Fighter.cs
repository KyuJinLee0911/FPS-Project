using System.Collections;
using System.Collections.Generic;
using FPS.Control;
using UnityEngine;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    [SerializeField] private WeaponData currentWeapon;
    public WeaponData CurrentWeapon { get => currentWeapon; }
    [SerializeField] private bool isWeaponFire = false;
    [SerializeField] private float timeSinceLastFire = 0f;
    // 히트 스캔에 필요한 타겟
    [SerializeField] private IDamageable target;
    [SerializeField] Transform muzzleTransform;
    
    void Start()
    {

    }
    void Update()
    {
        if (isWeaponFire)
            Fire();
    }

    void OnFire(InputValue value)
    {
        if(!GetComponent<PlayerController>().isControlable) return;
            
        if (value.Get<float>() == 1)
        {
            isWeaponFire = true;
        }
        else
        {
            timeSinceLastFire = 0;
            isWeaponFire = false;
        }
    }

    void Fire()
    {
        float _timeBetweenFires = 1 / currentWeapon.FireRate;

        timeSinceLastFire += Time.deltaTime;
        if (timeSinceLastFire >= _timeBetweenFires)
        {
            currentWeapon.FireArm(muzzleTransform, gameObject);
            timeSinceLastFire = 0;
        }
    }

    void OnMainSkill()
    {
        if(!GetComponent<PlayerController>().isControlable) return;
            
        UseMainSkill();
    }
    void UseMainSkill()
    {
        Player player = GetComponent<Player>();
        player.mainSkill.DoSkill();
    }

    void OnSubSkill()
    {
        if(!GetComponent<PlayerController>().isControlable) return;
            
        UseSubSkill();
    }

    void UseSubSkill()
    {
        Player player = GetComponent<Player>();
        player.subSkill.DoSkill();
    }

    // 자동 크리티컬 확률은 초기값 10%, 1.75배율의 고정값을 가짐
    // 아이템 습득을 통해 상승 가능
    // 자동 크리티컬 대미지를 계산하는 함수
    // 투사체, 스킬에 사용 가능
    public float CalculateAutoCriticalDamage(float damage)
    {
        float randomRate = UnityEngine.Random.Range(0.0f, 1.0f);
        float criticalDamage = damage * gameObject.GetComponent<IStat>().autoCriticalMagnification;
        float autoCriticalRate = gameObject.GetComponent<IStat>().autoCriticalRate;
        
        if(randomRate <= autoCriticalRate) 
        {
            Debug.Log($"Auto Critical! Rate : {randomRate}");
            return criticalDamage;
        }
        else return damage;
    }

    public float CalculateCriticalDamage(float damage)
    {
        float criticalDamage = damage * currentWeapon.CriticalMultiples;
        Debug.Log("Critical!");
        return criticalDamage;
    }
}