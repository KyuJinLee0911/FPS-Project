using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Organized : Feature
{
    // Start is called before the first frame update
    void Start()
    {
        Weapon weapon = transform.parent.parent.GetComponent<Weapon>();
        weapon.addRangeRateByFeature += 0.25f;
        weapon.ChangeRange();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
