using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using FPS.Control;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    public WeaponData basicWeapon;
    public WeaponData currentWeapon;
    public WeaponData[] currentWeapons = new WeaponData[2];
    public GameObject[] weaponSlots = new GameObject[2];
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
    public TwoBoneIKConstraint rightArmConstraint;

    // 무기의 추가 수치 (씬 변경시 저장하여 넘겨주는 데이터)
    [Header("Additional Weapon Data Values")]
    public float additionalDamageMagnifier = 0;
    public float additionalReloadSpeedMagnifier = 0;
    public float additionalFireRateMagnifier = 0;
    public float additionalMagMagnifier = 0;
    public float additionalCriticalMultiples = 0;

    private void Start()
    {
        InitWeapon();
    }

    public void InitFighter()
    {
        InitWeapon();
    }

    // 초기 무기 세팅
    private void InitWeapon()
    {
        currentWeapon = basicWeapon;
        currentWeapons[0] = currentWeapon;
        currentWeaponIndex = 0;
        GameObject newBasicWeaponObj = Instantiate(currentWeapon.prefab, GunPosition);
        Weapon weapon = newBasicWeaponObj.GetComponent<Weapon>();
        muzzleTransform = weapon.muzzleTransform;

        // 총의 반동과 함께 팔도 움직일 수 있도록 IKConstraint의 target 트랜스폼의 부모 오브젝트 변경
        ChangeGripParent(weapon.rGripParent);

        if (currentWeapon.WeaponType == WeaponType.WT_PROJECTILE)
        {
            GameManager.Instance._pool.AddNewObj(gameObject.name, currentWeapon.projectile.gameObject);
            GameManager.Instance._pool.Initialize(gameObject.name, 20);
        }

        currentWeapon.canFireWeapon = true;
        currentWeapon.Init();

        weaponSlots[0] = newBasicWeaponObj;
        gunModel = weaponSlots[0];
        defaultGunPos = gunModel.transform.localPosition;
        defaultGunRot = gunModel.transform.localRotation.eulerAngles;

        currentWeapons[0] = currentWeapon;
    }

    private void ChangeGripParent(Transform targetR)
    {
        if (!gameObject.CompareTag("Player")) return;

        rightArmConstraint.data.target.SetParent(targetR);
        rightArmConstraint.data.target.localPosition = Vector3.zero;
    }

    void FixedUpdate()
    {
        // Debug.Log($"is sub skill set? {isSubSkillSet}");
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
            MakeRebound();

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
        if (weaponSlots[1] == null)
        {
            Debug.Log("No Weapon To Swap");
            return;
        }
        else
            SwapWeapon(1);

    }

    void SwapWeapon(int index)
    {
        Weapon weapon = weaponSlots[index].GetComponent<Weapon>();
        if (currentWeaponIndex == index)
        {
            Debug.Log("Already");
            return;
        }
        weaponSlots[currentWeaponIndex].SetActive(false);
        weaponSlots[index].SetActive(true);
        currentWeapon = currentWeapons[index];
        currentWeaponIndex = index;
        ChangeGripParent(weapon.rGripParent);
    }

    public void PickUpWeapon(Weapon weapon)
    {
        GameObject newWeapon = weapon.gameObject;
        newWeapon.transform.SetParent(GunPosition);

        // 무기 슬롯이 가득차있을때
        // 현재 무기를 바닥에 버리고 새로운 무기 장착
        // index는 변화 없음
        if (weaponSlots[1] != null)
        {
            weaponSlots[currentWeaponIndex].transform.SetParent(null);
            Vector3 newPos = transform.position + (Vector3.up * 0.6f) + Vector3.forward;
            weaponSlots[currentWeaponIndex].transform.SetPositionAndRotation(newPos, Quaternion.identity);
            weaponSlots[currentWeaponIndex] = newWeapon;
            currentWeapons[currentWeaponIndex] = weapon.weaponData;
        }
        // 무기 슬롯이 가득차있지 않을 때
        // 새 무기를 새로운 슬롯에 장착하고
        // 인덱스를 현재 무기로 변환
        else
        {
            weaponSlots[currentWeaponIndex].SetActive(false);
            weaponSlots[1] = newWeapon;
            currentWeapons[1] = weapon.weaponData;
            timeSinceLastFire = float.MaxValue;
            currentWeaponIndex = 1;
        }
        muzzleTransform = weapon.muzzleTransform;
        gunModel = weaponSlots[currentWeaponIndex];
        currentWeapon = currentWeapons[currentWeaponIndex];
        currentWeapon.canFireWeapon = true;

        if (transform.localScale.x != 1)
        {
            newWeapon.transform.localScale = Vector3.one;
        }
        newWeapon.transform.localPosition = currentWeapon.gunPosition;
        newWeapon.transform.localRotation = Quaternion.identity;

        if (currentWeapon.WeaponType == WeaponType.WT_PROJECTILE)
        {
            GameManager.Instance._pool.AddNewObj(gameObject.name, currentWeapon.projectile.gameObject);
            GameManager.Instance._pool.Initialize(gameObject.name, 20);
        }
        ChangeGripParent(weapon.rGripParent);
        SetInGameWeaponData();
    }

    // 게임이 끝났을 때 사용하는 함수
    public void RemoveEveryWeapon()
    {
        ChangeGripParent(GunPosition);
        // 모든 무기 데이터, 무기 게임 오브젝트 삭제
        for (int i = 0; i < currentWeapons.Length; i++)
        {
            if(currentWeapons[i] == null) continue;
            currentWeapons[i].Init();
            Destroy(weaponSlots[i]);
            weaponSlots[i] = null;
            currentWeapons[i] = null;
        }

        // 새로운 기본 무기 데이터 할당 및 오브젝트 생성
        InitWeapon();
    }

    // InGame 함수
    // 현재 어빌리티와 아이템에 의해 변경된 추가 수치에 맞게 새로 주운 무기의 수치 변경
    private void SetInGameWeaponData()
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
        // MakeRebound();
        for (int i = 0; i < 3; i++)
        {
            yield return waitForFixedUpdate;
        }
        currentWeapon.bulletEffect.gameObject.SetActive(false);
    }

    public void MakeRebound()
    {
        if(!gameObject.CompareTag("Player")) return;
        currentWeapon.rebound += currentWeapon.reboundMagnifier;
        gunModel.transform.localPosition = defaultGunPos;
        Vector3 targetRotation = GunPosition.transform.localRotation.eulerAngles - new Vector3(currentWeapon.rebound, 0, 0);
        GameManager.Instance.controller.reboundXRotation = currentWeapon.rebound * -1f;
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