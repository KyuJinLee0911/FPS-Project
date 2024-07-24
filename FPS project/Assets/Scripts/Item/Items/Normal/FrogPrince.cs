using System.Collections;
using System.Collections.Generic;
using FPS.Control;
using UnityEngine;

public class FrogPrince : Item
{
    public override void DoItem()
    {
        base.DoItem();
        float additionalDamage = GameManager.Instance.player.transform.GetComponent<Fighter>().CurrentWeapon.Damage * 0.15f;

    }
}
