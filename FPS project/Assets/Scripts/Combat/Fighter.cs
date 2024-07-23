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
    [SerializeField] private float timeSinceLastFire = float.MaxValue;
    // 히트 스캔에 필요한 타겟
    [SerializeField] private IDamageable target;
    public Transform muzzleTransform;
    public Transform GunPosition;
    public GameObject gunModel;
    Vector3 defaultGunPos;
    Vector3 defaultGunRot;
    bool isRebound = false;


    private void Start()
    {
        currentWeapon.canFireWeapon = true;
        if (currentWeapon.WeaponType == WeaponType.WT_PROJECTILE)
        {
            GameManager.Instance._pool.AddNewObj(gameObject.name, currentWeapon.Projectile.gameObject);
            GameManager.Instance._pool.Initialize(gameObject.name, 20);
        }
        weaponSlots.Add(GunPosition.GetChild(0).gameObject);
        gunModel = weaponSlots[0];
        defaultGunPos = gunModel.transform.localPosition;
        defaultGunRot = gunModel.transform.localRotation.eulerAngles;
        currentWeaponIndex = 0;
    }
    void FixedUpdate()
    {
        if (isWeaponFire)
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

            isWeaponFire = false;
        }
    }

    public void Fire()
    {
        if (!currentWeapon.canFireWeapon) return;
        float _timeBetweenFires = 1 / currentWeapon.FireRate;

        timeSinceLastFire += Time.deltaTime;
        if (timeSinceLastFire >= _timeBetweenFires)
        {
            currentWeapon.FireArm(muzzleTransform, gameObject);
            if (currentWeapon.WeaponType == WeaponType.WT_HITSCAN)
                StartCoroutine(ShowBulletEffect(muzzleTransform));
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

    public IEnumerator ShowBulletEffect(Transform muzzleTransform)
    {
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        currentWeapon.bulletEffect.SetPosition(0, muzzleTransform.position);
        currentWeapon.bulletEffect.gameObject.SetActive(true);
        MakeRebound();
        for (int i = 0; i < 3; i++)
        {
            yield return waitForFixedUpdate;
        }
        currentWeapon.bulletEffect.gameObject.SetActive(false);
    }

    public void MakeRebound()
    {
        currentWeapon.Rebound += 3;
        gunModel.transform.localPosition = defaultGunPos;
        Vector3 targetRotation = gunModel.transform.localRotation.eulerAngles - new Vector3(currentWeapon.Rebound * 5,0,0);
        gunModel.transform.Translate(Vector3.forward * -0.1f);
        gunModel.transform.Rotate(targetRotation, Space.Self);
        
        if (!isRebound)
        {
            StartCoroutine(Rebound());
        }
    }

    IEnumerator Rebound()
    {
        isRebound = true;
        while(true)
        {
            gunModel.transform.localPosition = Vector3.Lerp(gunModel.transform.localPosition, defaultGunPos, Time.deltaTime * 3.0f);
            gunModel.transform.localRotation = Quaternion.Lerp(gunModel.transform.localRotation, Quaternion.Euler(defaultGunRot), Time.deltaTime* 3.0f);
            currentWeapon.Rebound = Mathf.Lerp(currentWeapon.Rebound, 0, Time.deltaTime * 3.0f);
            if(Vector3.Distance(gunModel.transform.localPosition, defaultGunPos) < 0.001f)
            {
                gunModel.transform.localPosition = defaultGunPos;
                currentWeapon.Rebound = 0;
                isRebound = false;
                break;
            }
            yield return null;
        }
        yield break;

    }
}