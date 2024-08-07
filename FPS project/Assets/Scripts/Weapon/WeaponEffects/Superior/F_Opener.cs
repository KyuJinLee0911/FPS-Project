using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class F_Opener : Feature
{
    Weapon data;
    private void Start()
    {
        data = GameManager.Instance.playerFighter.currentWeapon;
    }
    // Update is called once per frame
    void Update()
    {
        if (data.currentMag > data.totalMag - 3)
        {
            if(data.addDamageRateByFeature <= 0.69f)
            {
                data.addDamageRateByFeature += 0.7f;
                data.ChangeWeaponDamage(GameManager.Instance.playerFighter.additionalDamageMagnifier);
            }  
        }
        else
        {
            if(data.addDamageRateByFeature >= 0.69f)
            {
                data.addDamageRateByFeature -= 0.7f;
                data.ChangeWeaponDamage(GameManager.Instance.playerFighter.additionalDamageMagnifier);
            }
        }
    }
}
