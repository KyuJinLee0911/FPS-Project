using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float mag;
    public float Mag { get => mag; }
    [SerializeField] private float fireRate;
    public float FireRate { get => fireRate; }
    [SerializeField] private float fireRange;
    public float FireRange { get => fireRange; }
    [SerializeField] private WeaponType weaponType;
    public WeaponType WeaponType { get => weaponType; }
    [SerializeField] private Projectile projectile;
    private WeaponGrade weaponGrade;
    public GameObject weaponPrefab;
    public void FireArm(Transform transform, GameObject instigator)
    {
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
        Projectile projectileInstance = Instantiate(projectile, gunTransform.position, gunTransform.rotation);
        projectileInstance.SetDamage(damage);

        
        // 투사체의 목표를 설정 후 발사할 때 목표를 바라보고 화살이 일직선으로 나가도록
        // projectileInstance.SetTarget(target, instigator, Damage);
        // projectileInstance.transform.LookAt(gunTransform.position + gunTransform.forward);
        // projectileInstance.transform.LookAt(projectileInstance.GetAimLocation());
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
}
