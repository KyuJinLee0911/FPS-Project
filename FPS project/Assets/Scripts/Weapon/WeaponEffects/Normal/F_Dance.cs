using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class F_Dance : Feature
{
    // Start is called before the first frame update
    void Start()
    {
        Weapon weapon = transform.parent.parent.GetComponent<Weapon>();
        weapon.addFireRateByFeature += 0.1f;
        weapon.ChangeFireRate(GameManager.Instance.playerFighter.additionalFireRateMagnifier);
    }
}
