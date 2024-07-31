using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using FPS.Control;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    public WeaponData basicWeapon;
    public WeaponData currentWeapon;
    public WeaponData[] currentWeapons = new WeaponData[2];
    public List<GameObject> weaponSlots = new List<GameObject>();
    public int currentWeaponIndex = 0;
    public bool isWeaponFire = false;
    [SerializeField] private float timeSinceLastFire = float.MaxValue;
    public Transform muzzleTransform;
    public Transform GunPosition;
    public GameObject gunModel;
    Vector3 defaultGunPos;
    Vector3 defaultGunRot;
    bool isRebound = false;
    public bool isSubSkillSet = false;

    // 무기의 추가 수치 (씬 변경시 저장하여 넘겨주는 데이터)
    [Header("Additional Weapon Data Values")]
    public float additionalDamageMagnifier = 0;
    public float additionalReloadSpeedMagnifier = 0;
    public float additionalFireRateMagnifier = 0;
    public float additionalMagMagnifier = 0;
    public float additionalCriticalMultiples = 0;

    private void Start()
    {
        currentWeapon.canFireWeapon = true;
        currentWeapon.Init();

        if (currentWeapon.WeaponType == WeaponType.WT_PROJECTILE)
        {
            GameManager.Instance._pool.AddNewObj(gameObject.name, currentWeapon.projectile.gameObject);
            GameManager.Instance._pool.Initialize(gameObject.name, 20);
        }
        weaponSlots.Add(GunPosition.GetChild(currentWeaponIndex).gameObject);
        gunModel = weaponSlots[currentWeaponIndex];
        defaultGunPos = gunModel.transform.localPosition;
        defaultGunRot = gunModel.transform.localRotation.eulerAngles;
        
        currentWeapons[currentWeaponIndex] = currentWeapon;
    }

    void FixedUpdate()
    {
        Debug.Log($"is sub skill set? {isSubSkillSet}");
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
        float _timeBetweenFires = currentWeapon.TimeBetweenFires();

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

    void OnMainSkill(InputValue input)
    {
        if (!GetComponent<PlayerController>().isControlable) return;

        UseMainSkill();
    }
    void UseMainSkill()
    {
        Player player = GetComponent<Player>();
        player.mainSkill.DoSkill();
    }

    void OnSubSkill(InputValue input)
    {
        if (!GetComponent<PlayerController>().isControlable) return;

        if (input.Get<float>() == 1)
        {
            isSubSkillSet = true;
        }
        else if (input.Get<float>() == 0)
        {
            isSubSkillSet = false;
            UseSubSkill();
        }

    }

    void UseSubSkill()
    {
        Player player = GetComponent<Player>();
        player.subSkill.DoSkill();
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
            currentWeapon = newWeapon.GetComponent<Weapon>().weaponData;
        }
        // 무기 슬롯이 가득차있지 않을 때
        // 새 무기를 새로운 슬롯에 장착하고
        // 인덱스를 현재 무기로 변환
        else
        {
            weaponSlots[currentWeaponIndex].SetActive(false);
            weaponSlots.Add(newWeapon);
            currentWeapon = newWeapon.GetComponent<Weapon>().weaponData;
            timeSinceLastFire = float.MaxValue;
            currentWeaponIndex = 1;
        }
        muzzleTransform = newWeapon.transform.GetChild(0);
        InitializeNewWeapon();
    }

    private void InitializeNewWeapon()
    {
        currentWeapon.Init();
        currentWeapon.ChangeWeaponDamage(additionalDamageMagnifier);
        currentWeapon.ChangeReloadSpeed(additionalReloadSpeedMagnifier);
        currentWeapon.ChangeFireRate(additionalFireRateMagnifier);
        currentWeapon.AddMag(additionalMagMagnifier);
        currentWeapon.AddCriticalMultiples(additionalCriticalMultiples);
    }

    public IEnumerator ShowBulletEffect(Transform muzzleTransform)
    {
        if (currentWeapon.WeaponType != WeaponType.WT_HITSCAN)
            yield break;
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
        currentWeapon.rebound += currentWeapon.reboundMagnifier;
        gunModel.transform.localPosition = defaultGunPos;
        Vector3 targetRotation = gunModel.transform.localRotation.eulerAngles - new Vector3(currentWeapon.rebound * 5, 0, 0);
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
        while (true)
        {
            gunModel.transform.localPosition = Vector3.Lerp(gunModel.transform.localPosition, defaultGunPos, Time.deltaTime * 3.0f);
            gunModel.transform.localRotation = Quaternion.Lerp(gunModel.transform.localRotation, Quaternion.Euler(defaultGunRot), Time.deltaTime * 3.0f);
            currentWeapon.rebound = Mathf.Lerp(currentWeapon.rebound, 0, Time.deltaTime * 3.0f);
            if (Vector3.Distance(gunModel.transform.localPosition, defaultGunPos) < 0.001f)
            {
                gunModel.transform.localPosition = defaultGunPos;
                currentWeapon.rebound = 0;
                isRebound = false;
                break;
            }
            yield return null;
        }
        yield break;

    }
}