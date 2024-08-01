using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public WeaponData weaponData;
    public LineRenderer bulletEffect;
    public Transform muzzleTransform;
    public Transform rGripParent;
    public Transform lGripParent;

    private void Awake()
    {
        weaponData.currentMag = weaponData.totalMag;
        if (weaponData.WeaponType == WeaponType.WT_HITSCAN)
        {
            weaponData.bulletEffect = bulletEffect;
            weaponData.bulletEffect.startWidth = 0.05f;
            weaponData.bulletEffect.endWidth = 0.001f;
        }
    }

    public override void SetDescription()
    {
        if (canvasType == CanvasType.CT_SCREENSPACE) return;
        itemNameTxt.text = weaponData.itemName;
        itemDescriptionText.text = weaponData.itemDescription;
        if (weaponData.itemImage != null)
            itemImage.sprite = weaponData.itemImage;
    }


    public override void DoInteraction()
    {
        if (itemInfoWorldSpaceUI.activeSelf)
            itemInfoWorldSpaceUI.SetActive(false);

        GameManager.Instance.playerFighter.PickUpWeapon(this);
    }
}
