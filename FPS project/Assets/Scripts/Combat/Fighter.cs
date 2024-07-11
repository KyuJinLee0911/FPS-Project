using System.Collections;
using System.Collections.Generic;
using FPS.Control;
using UnityEngine;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    [SerializeField] private WeaponData currentWeapon;
    public WeaponData CurrentWeapon { get => currentWeapon; set => currentWeapon = value; }
    public List<GameObject> weaponSlots = new List<GameObject>();
    public int currentWeaponIndex;
    public bool isWeaponFire = false;
    [SerializeField] private float timeSinceLastFire = 0f;
    // 히트 스캔에 필요한 타겟
    [SerializeField] private IDamageable target;
    public Transform muzzleTransform;
    public Transform GunPosition;


    private void Start()
    {
        ObjectPool.Instance.AddNewObj(gameObject.name, currentWeapon.Projectile.gameObject);
        ObjectPool.Instance.Initialize(gameObject.name, currentWeapon.Mag);
        weaponSlots.Add(GunPosition.GetChild(0).gameObject);
        currentWeaponIndex = 0;
    }
    void Update()
    {
        if (isWeaponFire && currentWeapon.CanFireWeapon)
            Fire();
    }

    void OnFire(InputValue value)
    {
        if (!GetComponent<PlayerController>().isControlable) return;

        if (value.Get<float>() == 1)
        {
            isWeaponFire = true;
        }
        else
        {
            timeSinceLastFire = float.MaxValue;
            isWeaponFire = false;
        }
    }

    public void Fire()
    {
        float _timeBetweenFires = 1 / currentWeapon.FireRate;

        timeSinceLastFire += Time.deltaTime;
        if (timeSinceLastFire >= _timeBetweenFires)
        {
            currentWeapon.FireArm(muzzleTransform, gameObject);
            timeSinceLastFire = 0;
        }
        if (currentWeapon.currentMag <= 0)
        {
            currentWeapon.currentMag = 0;
            StartCoroutine(currentWeapon.ReloadMag());
        }
    }

    void OnReload()
    {
        StartCoroutine(currentWeapon.ReloadMag());
    }

    void OnMainSkill()
    {
        if (!GetComponent<PlayerController>().isControlable) return;

        UseMainSkill();
    }
    void UseMainSkill()
    {
        Player player = GetComponent<Player>();
        player.mainSkill.DoSkill();
    }

    void OnSubSkill()
    {
        if (!GetComponent<PlayerController>().isControlable) return;

        UseSubSkill();
    }

    void UseSubSkill()
    {
        Player player = GetComponent<Player>();
        player.subSkill.DoSkill();
    }

    public float CalculateDamage(float damage, DamageType damageType)
    {
        if (damageType == DamageType.DT_WEAKNESS)
        {
            float criticalDamage = damage * currentWeapon.CriticalMultiples;
            Debug.Log("Critical!");
            return criticalDamage;
        }
        else
        {
            float randomRate = UnityEngine.Random.Range(0.0f, 1.0f);
            float autoCriticalDamage = damage * gameObject.GetComponent<IStat>().autoCriticalMagnification;
            float autoCriticalRate = gameObject.GetComponent<IStat>().autoCriticalRate;

            if (randomRate < autoCriticalRate)
            {
                Debug.Log($"Auto Critical! Rate : {randomRate}");
                return autoCriticalDamage;
            }
            else
                return damage;
        }
    }

    void OnSwapToWeapon1()
    {
        SwapWeapon(0);
    }

    void OnSwapToWeapon2()
    {
        if (weaponSlots.Count == 1)
        {
            Debug.Log("No Weapon To Swap");
            return;
        }
        else
            SwapWeapon(1);

    }

    void SwapWeapon(int index)
    {
        if (currentWeaponIndex == index)
        {
            Debug.Log("Already");
            return;
        }
        weaponSlots[currentWeaponIndex].SetActive(false);
        weaponSlots[index].SetActive(true);
        currentWeapon = weaponSlots[index].GetComponent<Weapon>().weaponData;
        currentWeaponIndex = index;
    }

    public void PickUpWeapon(GameObject newWeapon)
    {
        // 무기 슬롯이 가득차있을때
        // 현재 무기를 바닥에 버리고 새로운 무기 장착
        // index는 변화 없음
        if (weaponSlots.Count == 2)
        {
            weaponSlots[currentWeaponIndex].transform.SetParent(null);
            weaponSlots[currentWeaponIndex].transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            weaponSlots[currentWeaponIndex] = newWeapon;
            CurrentWeapon = newWeapon.GetComponent<Weapon>().weaponData;
        }
        // 무기 슬롯이 가득차있지 않을 때
        // 새 무기를 새로운 슬롯에 장착하고
        // 인덱스를 현재 무기로 변환
        else
        {
            weaponSlots[currentWeaponIndex].SetActive(false);
            weaponSlots.Add(newWeapon);
            CurrentWeapon = newWeapon.GetComponent<Weapon>().weaponData;
            timeSinceLastFire = float.MaxValue;
            currentWeaponIndex = 1;
        }
        muzzleTransform = newWeapon.transform.GetChild(0);
    }
}