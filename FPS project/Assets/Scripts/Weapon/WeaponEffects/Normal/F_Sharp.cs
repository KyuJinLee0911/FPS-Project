using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Sharp : Feature
{
    void Start()
    {
        Weapon weapon = transform.parent.parent.GetComponent<Weapon>();
        weapon.addCriticalMultiplesByFeature += 0.15f;
        weapon.AddCriticalMultiples(GameManager.Instance.playerFighter.additionalCriticalMultiples);
    }

    
}
