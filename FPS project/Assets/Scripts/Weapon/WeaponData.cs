using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum WeaponType
{
    WT_PROJECTILE,
    WT_MELEE,
    WT_HITSCAN,
}

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make new Weapon", order = 0)]
public class WeaponData : ItemData
{
    [Header("Damage")]
    [SerializeField] private float damage;
    [SerializeField] private float additionalDamage;
    public float totalDamage;
    [SerializeField] private float criticalMultiples;
    [SerializeField] private float additionalCriticalMultiples;
    public float totalCriticalMultiples;

    [Header("Magazine")]
    [SerializeField] private int mag;
    [SerializeField] private int additionalMag;
    public int currentMag;
    public int totalMag;
    [SerializeField] private float reloadTime;
    [SerializeField] private float totalReloadTime;
    [SerializeField] private float additionalReloadTime;

    [Header("Weapon Info")]
    public bool canFireWeapon;
    [SerializeField] private float fireRate;
    [SerializeField] private float additionalFireRate;
    [SerializeField] private float totalFireRate;
    [SerializeField] private float fireRange;
    [SerializeField] private float effectiveRange;
    public float EffectiveRange { get => effectiveRange; }
    [SerializeField] private WeaponType weaponType;
    public WeaponType WeaponType { get => weaponType; }
    public Projectile projectile;
    public GameObject weaponPrefab;
    public LineRenderer bulletEffect;
    public float rebound;
    public float reboundMagnifier;

    [Header("Transform Adjustment")]
    public Vector3 gunPosition;
    public Vector3 leftHandPosition;
    public bool isTwoHanded;


    // 아이템이 생성될 때 한 번만 호출
    public void Init()
    {
        totalReloadTime = reloadTime;
        totalDamage = damage;
        totalFireRate = fireRate;
        totalCriticalMultiples = criticalMultiples;
        totalMag = mag;
        additionalDamage = 0;
        additionalCriticalMultiples = 0;
        additionalFireRate = 0;
        additionalReloadTime = 0;
        additionalMag = 0;
    }

    public void AddMag(float value)
    {
        ChangeValue(ref totalMag, ref additionalMag, mag, value);
    }

    public void AddCriticalMultiples(float value)
    {
        if (value == 0) return;
        additionalCriticalMultiples += value;
        totalCriticalMultiples = criticalMultiples + additionalCriticalMultiples;
    }

    public void ChangeWeaponDamage(float magnifier)
    {
        ChangeValue(ref totalDamage, ref additionalDamage, damage, magnifier);
    }

    public void ChangeReloadSpeed(float magnifier)
    {
        ChangeValue(ref totalReloadTime, ref additionalReloadTime, reloadTime, magnifier);
    }
    public void ChangeFireRate(float magnifier)
    {
        ChangeValue(ref totalFireRate, ref additionalFireRate, fireRate, magnifier);
    }

    public void ChangeValue(ref float valueToChange, ref float additionalValue, float originalValue, float magnifier)
    {
        if (magnifier == 0) return;

        additionalValue = originalValue * magnifier;
        valueToChange = additionalValue + originalValue;
    }

    public void ChangeValue(ref int valueToChange, ref int additionalValue, int originalValue, float magnifier)
    {
        if (magnifier == 0) return;

        additionalValue = Mathf.FloorToInt(originalValue * magnifier);
        valueToChange = originalValue + additionalValue;
    }

    public float TimeBetweenFires()
    {
        return 1 / totalFireRate;
    }

    public void FireArm(Transform transform, GameObject instigator)
    {
        currentMag--;
        switch (weaponType)
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
        projectile.range = fireRange;
        Projectile projectileInstance = GameManager.Instance._pool.GetProjectile(instigator.name, totalDamage, totalCriticalMultiples, instigator);
        projectileInstance.transform.SetPositionAndRotation(gunTransform.position, gunTransform.rotation);
    }

    public void FireHitScan(GameObject instigator)
    {
        Transform muzzleTransform = instigator.GetComponent<Fighter>().muzzleTransform;
        Vector2 reboundRayPos = Random.insideUnitCircle * rebound * 0.033f;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2 - 1 + reboundRayPos.x, Screen.height / 2 - 1 + reboundRayPos.y));

        RaycastHit[] hits = Physics.RaycastAll(ray, fireRange);
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
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(totalReloadTime);
        currentMag = totalMag;
        canFireWeapon = true;
    }

    public void ApplyImpack(Vector3 dir, float power)
    {
        // 넉백
    }
}
