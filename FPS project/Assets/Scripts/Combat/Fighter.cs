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
    // public GameObject gunModel;
    [SerializeField] Vector3 defaultGunPos;
    Vector3 defaultGunRot;
    bool isRebound = false;
    public bool isSubSkillSet = false;
    public TwoBoneIKConstraint rightArmConstraint;
    public TwoBoneIKConstraint leftArmConstraint;
    public Transform lOGTargetParent;

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

        currentWeaponIndex = 0;
        currentWeapons[currentWeaponIndex] = basicWeapon;
        currentWeapon = currentWeapons[currentWeaponIndex];

        GameObject newBasicWeaponObj = Instantiate(currentWeapon.prefab, GunPosition);
        Weapon weapon = newBasicWeaponObj.GetComponent<Weapon>();
        weaponSlots[currentWeaponIndex] = newBasicWeaponObj;
        muzzleTransform = weapon.muzzleTransform;
        if (gameObject.CompareTag("Player"))
            lOGTargetParent = leftArmConstraint.data.target.parent;

        if (currentWeapon.WeaponType == WeaponType.WT_PROJECTILE)
        {
            GameManager.Instance._pool.AddNewObj(gameObject.name, currentWeapon.projectile.gameObject);
            GameManager.Instance._pool.Initialize(gameObject.name, 20);
        }

        currentWeapon.canFireWeapon = true;
        currentWeapon.Init();

        defaultGunPos = weaponSlots[currentWeaponIndex].transform.localPosition;
        defaultGunRot = weaponSlots[currentWeaponIndex].transform.localRotation.eulerAngles;

        // 총의 반동과 함께 팔도 움직일 수 있도록 IKConstraint의 target 트랜스폼의 부모 오브젝트 변경
        ChangeGripParent();
    }

    private void ChangeGripParent()
    {
        if (!gameObject.CompareTag("Player")) return;

        Weapon weapon = weaponSlots[currentWeaponIndex].GetComponent<Weapon>();
        rightArmConstraint.data.target.SetParent(weapon.rGripParent);
        rightArmConstraint.data.target.localPosition = Vector3.zero;
        // 두손무기면 왼손도 이동
        ChangeLGripParent(currentWeapon.isTwoHanded);
    }

    // 왼손 이동시키는 함수
    public void ChangeLGripParent(bool isTwoHanded)
    {
        if (!gameObject.CompareTag("Player")) return;
        Weapon weapon = weaponSlots[currentWeaponIndex].GetComponent<Weapon>();
        if (isTwoHanded)
        {
            leftArmConstraint.data.target.SetParent(weapon.lGripParent);
            leftArmConstraint.data.target.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        else
        {
            leftArmConstraint.data.target.SetParent(lOGTargetParent);
            leftArmConstraint.data.target.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
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
        Player player = GameManager.Instance.player;

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

        // 기존 무기 오브젝트 비활성화 및 새 무기 오브젝트 활성화
        weaponSlots[currentWeaponIndex].SetActive(false);
        currentWeaponIndex = index;
        weaponSlots[index].SetActive(true);

        currentWeapon = currentWeapons[currentWeaponIndex];
        currentWeapon.canFireWeapon = true;

        SetDefaultPos();

        ChangeGripParent();
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

        currentWeapon = currentWeapons[currentWeaponIndex];
        currentWeapon.canFireWeapon = true;

        // 습득시 스케일 조정 및 위치 조절
        if (transform.localScale.x != 1)
        {
            newWeapon.transform.localScale = Vector3.one;
        }
        newWeapon.transform.localPosition = currentWeapon.gunPosition;
        newWeapon.transform.localRotation = Quaternion.identity;


        SetDefaultPos();

        if (currentWeapon.WeaponType == WeaponType.WT_PROJECTILE)
        {
            GameManager.Instance._pool.AddNewObj(gameObject.name, currentWeapon.projectile.gameObject);
            GameManager.Instance._pool.Initialize(gameObject.name, 20);
        }
        ChangeGripParent();
        SetInGameWeaponData();
    }

    private void SetDefaultPos()
    {
        // 무기 교체와 동시에 반동에 사용되는 기본 총 위치도 변경
        defaultGunPos = weaponSlots[currentWeaponIndex].transform.localPosition;
        defaultGunRot = weaponSlots[currentWeaponIndex].transform.localRotation.eulerAngles;
    }

    // 게임이 끝났을 때 사용하는 함수
    public void RemoveEveryWeapon()
    {
        currentWeapon = basicWeapon;
        ChangeGripParent();
        // 모든 무기 데이터, 무기 게임 오브젝트 삭제
        for (int i = 0; i < currentWeapons.Length; i++)
        {
            if (currentWeapons[i] == null) continue;
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
        if (!gameObject.CompareTag("Player")) return;
        currentWeapon.rebound += currentWeapon.reboundMagnifier;
        weaponSlots[currentWeaponIndex].transform.localPosition = defaultGunPos;
        Vector3 targetRotation = GunPosition.transform.localRotation.eulerAngles - new Vector3(currentWeapon.rebound, 0, 0);
        GameManager.Instance.controller.reboundXRotation = currentWeapon.rebound * -1f;
        weaponSlots[currentWeaponIndex].transform.Translate(Vector3.forward * -0.1f);
        weaponSlots[currentWeaponIndex].transform.Rotate(targetRotation, Space.Self);

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
            weaponSlots[currentWeaponIndex].transform.localPosition = Vector3.Lerp(weaponSlots[currentWeaponIndex].transform.localPosition, defaultGunPos, Time.deltaTime * 3.0f);
            weaponSlots[currentWeaponIndex].transform.localRotation = Quaternion.Lerp(weaponSlots[currentWeaponIndex].transform.localRotation, Quaternion.Euler(defaultGunRot), Time.deltaTime * 3.0f);
            currentWeapon.rebound = Mathf.Lerp(currentWeapon.rebound, 0, Time.deltaTime * 3.0f);
            if (Vector3.Distance(weaponSlots[currentWeaponIndex].transform.localPosition, defaultGunPos) < 0.001f)
            {
                weaponSlots[currentWeaponIndex].transform.localPosition = defaultGunPos;
                currentWeapon.rebound = 0;
                isRebound = false;
                break;
            }
            yield return null;
        }
        yield break;

    }
}