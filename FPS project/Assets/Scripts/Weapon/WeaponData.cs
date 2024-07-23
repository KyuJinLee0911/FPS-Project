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
    public float Damage { get => damage; }
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
    [SerializeField] private WeaponType weaponType;
    public WeaponType WeaponType { get => weaponType; }
    [SerializeField] private Projectile projectile;
    public Projectile Projectile { get => projectile; }
    public GameObject weaponPrefab;
    public LineRenderer bulletEffect;
    [SerializeField] private float rebound;
    public float Rebound { get => rebound; set => rebound = value; }


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
        Projectile projectileInstance = GameManager.Instance._pool.GetObj(instigator.name);
        projectileInstance.transform.SetPositionAndRotation(gunTransform.position, gunTransform.rotation);
        projectileInstance.SetProjectileDamage(damage, criticalMultiples, instigator);
    }

    public void FireHitScan(GameObject instigator)
    {
        Transform muzzleTransform = instigator.GetComponent<Fighter>().muzzleTransform;
        Vector2 reboundRayPos = Random.insideUnitCircle * rebound * 0.033f;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2 - 1 + reboundRayPos.x, Screen.height / 2 - 1 + reboundRayPos.y));
        
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, fireRange))
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red, 3.0f);
            IDamageable target = hit.collider.transform.root.GetComponent<IDamageable>();
            if (target == null) return;

            // 탄흔효과
            bulletEffect.SetPosition(1, hit.point);
            target.TakeDamage(instigator, damage);
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
