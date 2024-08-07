using System.Collections;
using System.Net.Http.Headers;
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
    public float damage;
    public float criticalMultiples;

    [Header("Magazine")]
    public int mag;
    public float reloadTime;

    [Header("Weapon Info")]
    public bool isTwoHanded;
    public WeaponType weaponType;
    public float fireRange;
    public float fireRate;

    [Header("Transform Adjustment")]
    public Vector3 gunPosition;
    public Vector3 leftHandPosition;

}
