using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public WeaponData weaponData;
    public LineRenderer bulletEffect;

    private void Awake()
    {
        weaponData.currentMag = weaponData.mag;
        if (weaponData.WeaponType == WeaponType.WT_HITSCAN)
        {
            weaponData.bulletEffect = bulletEffect;
            weaponData.bulletEffect.startWidth = 0.05f;
            weaponData.bulletEffect.endWidth = 0.001f;
        }
    }


    public override void DoInteraction()
    {
        itemInfoWorldSpaceUI.SetActive(false);
        Fighter fighter = GameManager.Instance.player.transform.GetComponent<Fighter>();
        GameObject newWeapon = Instantiate(weaponData.weaponPrefab, fighter.GunPosition);
        fighter.PickUpWeapon(newWeapon);
    }
}
