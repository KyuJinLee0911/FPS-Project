using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum WeaponType
{
    WT_PROJECTILE,
    WT_MELEE,
    WT_HEATSCAN,
}

public enum WeaponGrade
{
    WG_NORMAL,
    WG_RARE,
    WG_SUPERIOR,
    WG_EPIC,
    WG_LEGENDARY
}

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make new Weapon", order = 0)]
public class WeaponData : ScriptableObject
{
    [SerializeField] string weaponName;
    public string Name { get => weaponName; }
    [SerializeField] private float damage;
    public float Damage { get => damage; }
    [SerializeField] private float criticalMultiples;
    public float CriticalMultiples { get => criticalMultiples; }
    [SerializeField] private int mag;
    public int Mag { get => mag; }
    public int currentMag;
    private bool canFireWeapon = true;
    public bool CanFireWeapon { get => canFireWeapon; }
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
    private WeaponGrade weaponGrade;
    public GameObject weaponPrefab;

    private void Awake()
    {
        currentMag = Mag;
    }

    public void FireArm(Transform transform, GameObject instigator)
    {
        currentMag--;
        switch (weaponType)
        {
            case WeaponType.WT_PROJECTILE:
                LaunchProjectile(transform, instigator);
                break;

            case WeaponType.WT_HEATSCAN:
                FireHeatscan();
                break;

            case WeaponType.WT_MELEE:
                MeleeAttack();
                break;
        }
    }

    public void LaunchProjectile(Transform gunTransform, GameObject instigator)
    {
        projectile.range = fireRange;
        Projectile projectileInstance = ObjectPool.GetObj(instigator.name);
        projectileInstance.transform.SetPositionAndRotation(gunTransform.position, gunTransform.rotation);
        projectileInstance.SetDamage(damage, criticalMultiples, instigator);
        Debug.Log("PewPew");
    }

    public void FireHeatscan()
    {
        Debug.Log("Buzzzzzzz");
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
}
