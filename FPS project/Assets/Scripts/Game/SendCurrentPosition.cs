using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendCurrentPosition : MonoBehaviour
{
    [SerializeField] private Weapon weapon;

    private void OnEnable()
    {
        // weapon = transform.parent.GetComponent<Weapon>();
        weapon.weaponData.bulletEffect = GetComponent<LineRenderer>();
    }
}
