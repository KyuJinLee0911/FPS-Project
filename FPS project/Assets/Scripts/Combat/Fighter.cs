using System.Collections;
using FPS.Control;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    public Weapon basicWeapon;
    public Weapon currentWeapon;
    public Weapon[] currentWeapons = new Weapon[2];
    // public GameObject[] weaponSlots = new GameObject[2];
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
    public GameObject reloadUIObj;

    // 무기의 추가 수치 (어빌리티 습득시 증가. 모든 무기에 영향을 미침)
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
        GameObject newBasicWeaponObj = Instantiate(basicWeapon.weaponData.prefab, GunPosition);
        Weapon weapon = newBasicWeaponObj.GetComponent<Weapon>();
        currentWeapons[currentWeaponIndex] = weapon;
        currentWeapon = weapon;

        muzzleTransform = weapon.muzzleTransform;
        if (gameObject.CompareTag("Player"))
            lOGTargetParent = leftArmConstraint.data.target.parent;

        if (currentWeapon.weaponData.weaponType == WeaponType.WT_PROJECTILE)
        {
            GameManager.Instance._pool.AddNewObj(gameObject.name, currentWeapon.projectile.gameObject);
            GameManager.Instance._pool.Initialize(gameObject.name, 20);
        }

        currentWeapon.canFireWeapon = true;
        currentWeapon.Init();

        defaultGunPos = currentWeapon.transform.localPosition;
        defaultGunRot = currentWeapon.transform.localRotation.eulerAngles;

        // 총의 반동과 함께 팔도 움직일 수 있도록 IKConstraint의 target 트랜스폼의 부모 오브젝트 변경
        ChangeGripParent(weapon);

        // currentWeapon.effectObj.SetActive(false);
    }

    private void ChangeGripParent(Weapon weapon)
    {
        if (!gameObject.CompareTag("Player")) return;
        
        rightArmConstraint.data.target.SetParent(weapon.rGripParent);
        rightArmConstraint.data.target.localPosition = Vector3.zero;
        // 두손무기면 왼손도 이동
        ChangeLGripParent(currentWeapon.weaponData.isTwoHanded);
    }

    // 왼손 이동시키는 함수
    public void ChangeLGripParent(bool isTwoHanded)
    {
        if (!gameObject.CompareTag("Player")) return;
        Weapon weapon = currentWeapon;
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
            if (currentWeapon.weaponData.weaponType == WeaponType.WT_HITSCAN)
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
        if (currentWeapons[1] == null)
        {
            Debug.Log("No Weapon To Swap");
            return;
        }
        else
            SwapWeapon(1);

    }

    void SwapWeapon(int index)
    {
        Weapon weapon = currentWeapons[index];
        if (currentWeaponIndex == index)
        {
            Debug.Log("Already");
            return;
        }

        // 기존 무기 오브젝트 비활성화 및 새 무기 오브젝트 활성화
        currentWeapon.gameObject.SetActive(false);
        currentWeaponIndex = index;
        currentWeapon = currentWeapons[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);

        currentWeapon.canFireWeapon = true;

        SetDefaultPos();

        ChangeGripParent(currentWeapon);
    }

    public void PickUpWeapon(Weapon weapon)
    {
        GameObject newWeapon = weapon.gameObject;
        newWeapon.transform.SetParent(GunPosition);

        // 무기 슬롯이 가득차있을때
        // 현재 무기를 바닥에 버리고 새로운 무기 장착
        // index는 변화 없음
        if (currentWeapons[1] != null)
        {
            currentWeapon.transform.SetParent(null);
            Vector3 newPos = transform.position + (Vector3.up * 0.6f) + Vector3.forward;
            currentWeapon.transform.SetPositionAndRotation(newPos, Quaternion.identity);
        }
        // 무기 슬롯이 가득차있지 않을 때
        // 새 무기를 새로운 슬롯에 장착하고
        // 인덱스를 현재 무기로 변환
        else
        {
            currentWeapon.gameObject.SetActive(false);
            currentWeaponIndex = 1;
        }

        currentWeapon = weapon;
        currentWeapons[currentWeaponIndex] = weapon;
        timeSinceLastFire = float.MaxValue;
        muzzleTransform = weapon.muzzleTransform;

        currentWeapon.canFireWeapon = true;

        // 습득시 스케일 조정 및 위치 조절
        if (transform.localScale.x != 1)
        {
            newWeapon.transform.localScale = Vector3.one;
        }
        newWeapon.transform.localPosition = currentWeapon.weaponData.gunPosition;
        newWeapon.transform.localRotation = Quaternion.identity;


        SetDefaultPos();

        if (currentWeapon.weaponData.weaponType == WeaponType.WT_PROJECTILE)
        {
            GameManager.Instance._pool.AddNewObj(gameObject.name, currentWeapon.projectile.gameObject);
            GameManager.Instance._pool.Initialize(gameObject.name, 20);
        }
        ChangeGripParent(weapon);
        SetInGameWeaponData();
    }

    private void SetDefaultPos()
    {
        // 무기 교체와 동시에 반동에 사용되는 기본 총 위치도 변경
        defaultGunPos = currentWeapon.transform.localPosition;
        defaultGunRot = currentWeapon.transform.localRotation.eulerAngles;
    }

    // 게임이 끝났을 때 사용하는 함수
    public void RemoveEveryWeapon()
    {
        // 손 위치 기본 무기 위치로 변경
        GameObject tempWeapon = Instantiate(basicWeapon.weaponData.prefab, GunPosition);
        Weapon temp = tempWeapon.GetComponent<Weapon>();
        ChangeGripParent(temp);
        Destroy(tempWeapon);
        currentWeapon = basicWeapon;
        // 모든 무기 데이터, 무기 게임 오브젝트 삭제
        for (int i = 0; i < currentWeapons.Length; i++)
        {
            if (currentWeapons[i] == null) continue;
            Destroy(currentWeapons[i].gameObject);
            currentWeapons[i] = null;
        }

        // 새로운 기본 무기 데이터 할당 및 오브젝트 생성
        InitWeapon();
    }

    // InGame 함수
    // 현재 어빌리티와 아이템에 의해 변경된 추가 수치에 맞게 새로 주운 무기의 수치 변경
    private void SetInGameWeaponData()
    {
        currentWeapon.ChangeWeaponDamage(additionalDamageMagnifier);
        currentWeapon.ChangeReloadSpeed(additionalReloadSpeedMagnifier);
        currentWeapon.ChangeFireRate(additionalFireRateMagnifier);
        currentWeapon.AddMag(additionalMagMagnifier);
        currentWeapon.AddCriticalMultiples(additionalCriticalMultiples);
    }

    public IEnumerator ShowBulletEffect(Transform muzzleTransform)
    {
        if (currentWeapon.weaponData.weaponType != WeaponType.WT_HITSCAN)
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
        currentWeapon.transform.localPosition = defaultGunPos;
        Vector3 targetRotation = GunPosition.transform.localRotation.eulerAngles - new Vector3(currentWeapon.rebound, 0, 0);
        GameManager.Instance.controller.reboundXRotation = currentWeapon.rebound * -1f;
        currentWeapon.transform.Translate(Vector3.forward * -0.1f);
        currentWeapon.transform.Rotate(targetRotation, Space.Self);

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
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, defaultGunPos, Time.deltaTime * 3.0f);
            currentWeapon.transform.localRotation = Quaternion.Lerp(currentWeapon.transform.localRotation, Quaternion.Euler(defaultGunRot), Time.deltaTime * 3.0f);
            currentWeapon.rebound = Mathf.Lerp(currentWeapon.rebound, 0, Time.deltaTime * 3.0f);
            if (Vector3.Distance(currentWeapon.transform.localPosition, defaultGunPos) < 0.001f)
            {
                currentWeapon.transform.localPosition = defaultGunPos;
                currentWeapon.rebound = 0;
                isRebound = false;
                break;
            }
            yield return null;
        }
        yield break;

    }
}