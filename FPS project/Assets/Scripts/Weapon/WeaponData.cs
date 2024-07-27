using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
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
    [SerializeField] private float damage;
    public float Damage { get => damage; set => damage = value; }
    [SerializeField] private float criticalMultiples;
    public float CriticalMultiples { get => criticalMultiples; }
    [SerializeField] private int mag;
    public int Mag { get => mag; }
    public int currentMag;
    public bool canFireWeapon { get; set; }
    [SerializeField] private float reloadTime;
    public float ReloadTime { get => reloadTime; }
    [SerializeField] private float fireRate;
    public float FireRate { get => fireRate; }
    [SerializeField] private float fireRange;
    public float FireRange { get => fireRange; }
    [SerializeField] private float effectiveRange;
    public float EffectiveRange { get => effectiveRange; }
    [SerializeField] private WeaponType weaponType;
    public WeaponType WeaponType { get => weaponType; }
    [SerializeField] private Projectile projectile;
    public Projectile Projectile { get => projectile; }
    public GameObject weaponPrefab;
    public LineRenderer bulletEffect;
    [SerializeField] private float rebound;
    public float Rebound { get => rebound; set => rebound = value; }
    public float reboundMagnifier;


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
        Projectile projectileInstance = GameManager.Instance._pool.GetProjectile(instigator.name, damage, criticalMultiples, instigator);
        projectileInstance.transform.SetPositionAndRotation(gunTransform.position, gunTransform.rotation);
    }

    public void FireHitScan(GameObject instigator)
    {
        Transform muzzleTransform = instigator.GetComponent<Fighter>().muzzleTransform;
        Vector2 reboundRayPos = Random.insideUnitCircle * rebound * 0.033f;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2 - 1 + reboundRayPos.x, Screen.height / 2 - 1 + reboundRayPos.y));
        
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, fireRange))
        {
            // 탄흔효과
            bulletEffect.SetPosition(1, hit.point);

            HitBox hitBox = hit.collider.transform.GetComponent<HitBox>();
            if (hitBox == null) return;

            hitBox.instigator = instigator;

            // 유효사거리보다 멀면 데미지 40% 감소
            float effectiveRangeMultiplier = Vector3.Distance(muzzleTransform.position, hit.point) <= effectiveRange ? 1 : 0.6f;
            float _damage = hitBox.CalculateDamage(damage, criticalMultiples) * effectiveRangeMultiplier;

            hitBox.damage = _damage;

            hitBox.GetHit(instigator, _damage);
        }
        else
        {
            Debug.DrawLine(muzzleTransform.position,  muzzleTransform.position + muzzleTransform.TransformDirection(reboundRayPos.x, reboundRayPos.y, 15), Color.red, 3.0f);
            bulletEffect.SetPosition(1, muzzleTransform.position + muzzleTransform.TransformDirection(reboundRayPos.x, reboundRayPos.y, 15));
        }
    }

    public void MeleeAttack()
    {
        Debug.Log("Swoosh");
    }

    public IEnumerator ReloadMag()
    {
        canFireWeapon = false;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentMag = mag;
        canFireWeapon = true;
    }

    public void ApplyImpack(Vector3 dir, float power)
    {
        // 넉백
    }
}
