using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IInteractable
{
    public WeaponData weaponData;
    [SerializeField] GameObject itemInfoWorldSpaceUI;
    public GameObject worldSpaceUI { get => itemInfoWorldSpaceUI; }
    public bool canInteract { get; set; }

    public void DoInteraction()
    {
        itemInfoWorldSpaceUI.SetActive(false);
        Fighter fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
        GameObject newWeapon = Instantiate(weaponData.weaponPrefab, fighter.GunPosition);
        fighter.PickUpWeapon(newWeapon);
    }
}
