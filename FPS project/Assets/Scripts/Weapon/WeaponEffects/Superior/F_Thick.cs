using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Thick : Feature
{
    // Start is called before the first frame update
    void Start()
    {
        Weapon weapon = transform.parent.parent.GetComponent<Weapon>();
        weapon.addDamageRateByFeature += 0.1f;
        weapon.ChangeWeaponDamage(GameManager.Instance.playerFighter.additionalDamageMagnifier);
    }

    
}
