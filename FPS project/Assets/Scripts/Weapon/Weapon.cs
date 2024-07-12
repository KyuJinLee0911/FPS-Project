using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public WeaponData weaponData;

    public override void DoInteraction()
    {
        itemInfoWorldSpaceUI.SetActive(false);
        Fighter fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
        GameObject newWeapon = Instantiate(weaponData.weaponPrefab, fighter.GunPosition);
        fighter.PickUpWeapon(newWeapon);
    }
}
