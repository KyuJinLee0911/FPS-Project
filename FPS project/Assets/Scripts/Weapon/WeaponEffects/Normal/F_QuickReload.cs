using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_QuickReload : Feature
{
    private void Start()
    {
        Weapon weapon = transform.parent.parent.GetComponent<Weapon>();
        weapon.addReloadTimeRateByFeature += -0.15f;
        weapon.ChangeReloadSpeed(GameManager.Instance.playerFighter.additionalReloadSpeedMagnifier);
    }
}
