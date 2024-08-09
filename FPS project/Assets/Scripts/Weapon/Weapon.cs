using System.Collections;
using System.Collections.Generic;
using UnityEditor.Hardware;
using UnityEngine;
using UnityEngine.VFX;

public class Weapon : Item
{
    public WeaponData weaponData;
    public ItemRarity weaponRarity;
    public LineRenderer bulletEffect;
    public Transform muzzleTransform;
    public Transform rGripParent;
    public Transform lGripParent;
    public List<Feature> normalFeature;
    public List<Feature> superiorFeature;
    public Feature legendaryFeature;
    public Transform featureParent;
    public GameObject effectObj;
    public VisualEffect dropEffect;

    [Header("Damage")]
    public float addDamageRateByFeature; // 생성시 추가된 특성에 의해 하는 값의 비율. 무기마다 다르게 적용
    float additionalDamageByAbility; // 어빌리티에 의해 증가한 값. 모든 무기에 적용되어 있음
    public float totalDamage;

    [Header("Critical")]
    public float addCriticalMultiplesByFeature; // 특성
    float additionalCriticalMultiplesByAbility; // 어빌리티
    public float totalCriticalMultiples;

    [Header("Mag")]
    public float addMagRateByFeature; // 특성
    float additionalMagByAbility; // 어빌리티
    public int currentMag;
    public int totalMag;

    [Header("Reload")]
    public float addReloadTimeRateByFeature; // 특성
    float additionalReloadTimeByAbility; // 어빌리티
    public float totalReloadTime;
    public bool canFireWeapon;

    [Header("Fire Rate")]
    public float addFireRateByFeature; // 특성
    float additionalFireRateByAbility; // 어빌
    public float totalFireRate;

    [Header("Fire Range")]
    public float addRangeRateByFeature;
    public float effectiveRange;
    public float totalFireRange;

    public Projectile projectile;

    [Header("Rebound")]
    public float rebound;
    public float reboundMagnifier;


    [SerializeField]
    List<float> rarityWeights = new List<float>
    {
        (float)ItemRarity.IR_NORMAL,
        (float)ItemRarity.IR_SUPERIOR,
        (float)ItemRarity.IR_LEGENDARY
    };

    private void Awake()
    {
        currentMag = totalMag;
        if (weaponData.weaponType == WeaponType.WT_HITSCAN)
        {
            bulletEffect.startWidth = 0.05f;
            bulletEffect.endWidth = 0.001f;
        }

        Init();
    }

    private void Start()
    {
        if (gameObject.CompareTag("Enemy")) return;

        DecideRarity();
        DecideFeatures();
        InstantiateFeature();
    }



    private void OnDisable()
    {
        // Init();
    }

    #region about random rarity and feature of weapon
    private void DecideRarity()
    {
        int rarityIndex = GameManager.Instance._item.PickRandomIndex(rarityWeights);
        switch (rarityIndex)
        {
            case 0:
                weaponRarity = ItemRarity.IR_NORMAL;
                if (transform.parent != null && transform.parent.gameObject.CompareTag("Player"))
                    break;

                effectObj = Instantiate(GameManager.Instance._item.itemDropEffect[0], transform.position, Quaternion.identity);
                dropEffect = effectObj.GetComponentInChildren<VisualEffect>();
                break;

            case 1:
                weaponRarity = ItemRarity.IR_SUPERIOR;
                if (transform.parent != null && transform.parent.gameObject.CompareTag("Player"))
                    break;
                effectObj = Instantiate(GameManager.Instance._item.itemDropEffect[3], transform.position, Quaternion.identity);
                dropEffect = effectObj.GetComponentInChildren<VisualEffect>();
                break;

            case 2:
                weaponRarity = ItemRarity.IR_LEGENDARY;
                if (transform.parent != null && transform.parent.gameObject.CompareTag("Player"))
                    break;
                effectObj = Instantiate(GameManager.Instance._item.itemDropEffect[4], transform.position, Quaternion.identity);
                dropEffect = effectObj.GetComponentInChildren<VisualEffect>();
                break;
        }
    }

    private void DecideFeatures()
    {
        switch (weaponRarity)
        {
            case ItemRarity.IR_NORMAL:
                break;

            case ItemRarity.IR_SUPERIOR:
                int cnt_n = Random.Range(1, 4); // 1개에서 3개
                normalFeature = PickFeatures(GameManager.Instance._item.weaponFeatures_n, cnt_n);
                int cnt_s = Random.Range(1, 3); // 1개 혹은 2개
                superiorFeature = PickFeatures(GameManager.Instance._item.weaponFeatures_s, cnt_s);
                break;

            case ItemRarity.IR_LEGENDARY:
                normalFeature = PickFeatures(GameManager.Instance._item.weaponFeatures_n, 3);
                superiorFeature = PickFeatures(GameManager.Instance._item.weaponFeatures_s, 3);
                break;
        }
    }

    private void InstantiateFeature()
    {
        if (normalFeature.Count == 0) return;
        for (int i = 0; i < normalFeature.Count; i++)
        {
            Feature newObj_n = Instantiate(normalFeature[i], featureParent);
            newObj_n.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        if (superiorFeature.Count == 0) return;
        for (int i = 0; i < superiorFeature.Count; i++)
        {
            Feature newObj_s = Instantiate(superiorFeature[i], featureParent);
            newObj_s.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        if (legendaryFeature == null) return;
        Feature newObj_l = Instantiate(legendaryFeature, featureParent);
        newObj_l.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private List<Feature> PickFeatures(List<Feature> features, int cnt)
    {
        if (features.Count == 0) return new List<Feature>();

        int _cnt = 0;
        List<Feature> pickedFeatures = new List<Feature>();


        while (_cnt < cnt)
        {
            int randIdx = Random.Range(0, features.Count);

            if (pickedFeatures.Contains(features[randIdx])) continue;

            pickedFeatures.Add(features[randIdx]);
            _cnt++;
        }

        return pickedFeatures;
    }
    #endregion

    #region Interaction and description
    public override void SetDescription()
    {
        if (canvasType == CanvasType.CT_SCREENSPACE) return;
        itemNameTxt.text = weaponData.itemName;
        string _desc = "";
        if (normalFeature.Count != 0)
        {
            for (int i = 0; i < normalFeature.Count; i++)
            {
                string normalText = "<color=green>" + normalFeature[i].description + "</color>";
                _desc += normalText;
            }
        }

        if (superiorFeature.Count != 0)
        {
            for (int i = 0; i < superiorFeature.Count; i++)
            {
                string supText = "<color=purple>" + superiorFeature[i].description + "</color>";
                _desc += supText;
            }
        }

        if (legendaryFeature != null)
            _desc += legendaryFeature.description;


        weaponData.itemDescription = _desc;
        itemDescriptionText.text = _desc.Replace("\\n", "\n");


        if (weaponData.itemImage != null)
            itemImage.sprite = weaponData.itemImage;
    }


    public override void DoInteraction()
    {
        if (itemInfoWorldSpaceUI.activeSelf)
            itemInfoWorldSpaceUI.SetActive(false);

        GameManager.Instance.playerFighter.PickUpWeapon(this);

        if (dropEffect != null)
        {
            dropEffect.Stop();
            effectObj.SetActive(false);
        }
    }
    #endregion

    // 아이템이 생성될 때 한 번만 호출
    public void Init()
    {
        totalReloadTime = weaponData.reloadTime;
        totalDamage = weaponData.damage;
        totalFireRate = weaponData.fireRate;
        totalCriticalMultiples = weaponData.criticalMultiples;
        totalMag = weaponData.mag;
        currentMag = weaponData.mag;
        totalFireRange = weaponData.fireRange;
        // 어빌리티 초기화
        additionalDamageByAbility = 0;
        additionalCriticalMultiplesByAbility = 0;
        additionalFireRateByAbility = 0;
        additionalReloadTimeByAbility = 0;
        additionalMagByAbility = 0;
        // 무기 특성 초기화
        addCriticalMultiplesByFeature = 0;
        addDamageRateByFeature = 0;
        addFireRateByFeature = 0;
        addMagRateByFeature = 0;
        addRangeRateByFeature = 0;
        addReloadTimeRateByFeature = 0;

    }

    public void ChangeValue(string valueName, float magnifier)
    {
        switch (valueName)
        {
            case "Damage":
                addDamageRateByFeature += magnifier;
                break;

            case "ReloadSpeed":
                addReloadTimeRateByFeature += magnifier;
                break;

            case "CriticalMultiple":
                addCriticalMultiplesByFeature += magnifier;
                break;

            case "Mag":
                addMagRateByFeature += magnifier;
                break;

            case "FireRate":
                addMagRateByFeature += magnifier;
                break;

            case "FireRange":
                addRangeRateByFeature += magnifier;
                break;
        }
    }

    #region change weapon values
    public void AddMag(float value)
    {
        additionalMagByAbility = value;
        int additionalMag = Mathf.FloorToInt(weaponData.mag * (additionalMagByAbility + addMagRateByFeature));
        totalMag = weaponData.mag + additionalMag;
        currentMag = totalMag;
    }

    public void AddCriticalMultiples(float value)
    {
        if (value == 0) return;
        additionalCriticalMultiplesByAbility = value;
        totalCriticalMultiples = weaponData.criticalMultiples + additionalCriticalMultiplesByAbility + addCriticalMultiplesByFeature;
    }

    public void ChangeRange()
    {
        float additionalRange = weaponData.fireRange * addRangeRateByFeature;
        totalFireRange = weaponData.fireRange + additionalRange;
    }

    public void ChangeWeaponDamage(float magnifier)
    {
        additionalDamageByAbility = magnifier;
        float additionalDamage = weaponData.damage * (additionalDamageByAbility + addDamageRateByFeature);
        totalDamage = weaponData.damage + additionalDamage;
    }

    public void ChangeReloadSpeed(float magnifier)
    {
        additionalReloadTimeByAbility = magnifier;
        float additionalFireRate = weaponData.reloadTime * (additionalReloadTimeByAbility + addReloadTimeRateByFeature);
        totalReloadTime = weaponData.reloadTime + additionalFireRate;
    }
    public void ChangeFireRate(float magnifier)
    {
        additionalFireRateByAbility = magnifier;
        float additionalFireRate = weaponData.fireRate * (additionalFireRateByAbility + addFireRateByFeature);
        totalFireRate = weaponData.fireRate + additionalFireRate;
    }
    #endregion

    #region firing weapon
    public float TimeBetweenFires()
    {
        return 1 / totalFireRate;
    }

    public void FireArm(Transform transform, GameObject instigator)
    {
        currentMag--;
        switch (weaponData.weaponType)
        {
            case WeaponType.WT_PROJECTILE:
                LaunchProjectile(transform, instigator);
                break;

            case WeaponType.WT_HITSCAN:
                FireHitScan(instigator);
                break;

            case WeaponType.WT_MELEE:
                MeleeAttack();
                break;
        }
    }

    public void LaunchProjectile(Transform gunTransform, GameObject instigator)
    {
        projectile.range = totalFireRange;
        Projectile projectileInstance = GameManager.Instance._pool.GetProjectile(instigator.name, totalDamage, totalCriticalMultiples, instigator);
        projectileInstance.transform.SetPositionAndRotation(gunTransform.position, gunTransform.rotation);
    }

    public void FireHitScan(GameObject instigator)
    {
        Transform muzzleTransform = instigator.GetComponent<Fighter>().muzzleTransform;
        Vector2 reboundRayPos = Random.insideUnitCircle * rebound * 0.033f;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2 - 1 + reboundRayPos.x, Screen.height / 2 - 1 + reboundRayPos.y));

        RaycastHit[] hits = Physics.RaycastAll(ray, totalFireRange);
        foreach (var info in hits)
        {
            if (info.collider.GetComponent<HitBox>() == null)
                continue;

            // 탄흔효과
            bulletEffect.SetPosition(1, info.point);

            HitBox hitBox = info.collider.transform.GetComponent<HitBox>();

            hitBox.instigator = instigator;

            // 유효사거리보다 멀면 데미지 40% 감소
            float effectiveRangeMultiplier = Vector3.Distance(muzzleTransform.position, info.point) <= effectiveRange ? 1 : 0.6f;
            float _damage = hitBox.CalculateDamage(totalDamage, totalCriticalMultiples) * effectiveRangeMultiplier;

            hitBox.damage = _damage;
            hitBox.GetHit(instigator, _damage);
            return;
        }

        Debug.DrawLine(muzzleTransform.position, muzzleTransform.position + muzzleTransform.TransformDirection(reboundRayPos.x, reboundRayPos.y, 15), Color.red, 3.0f);
        bulletEffect.SetPosition(1, muzzleTransform.position + muzzleTransform.TransformDirection(reboundRayPos.x, reboundRayPos.y, 15));
    }

    public void MeleeAttack()
    {
        Debug.Log("Swoosh");
    }

    public IEnumerator ReloadMag()
    {
        canFireWeapon = false;
        if (!gameObject.CompareTag("Enemy"))
            GameManager.Instance.playerFighter.reloadUIObj.SetActive(true);
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(totalReloadTime);
        currentMag = totalMag;
        canFireWeapon = true;
        if (!gameObject.CompareTag("Enemy"))
            GameManager.Instance.playerFighter.reloadUIObj.SetActive(false);
    }
    #endregion

    public void ApplyImpack(Vector3 dir, float power)
    {
        // 넉백
    }
}
