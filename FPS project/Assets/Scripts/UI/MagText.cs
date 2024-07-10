using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagText : MonoBehaviour
{
    public Text magText;
    private WeaponData currentWeapon;
    Fighter fighter;

    void Start()
    {
        fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();

    }

    // Update is called once per frame
    void Update()
    {
        currentWeapon = fighter.CurrentWeapon;
        magText.text = $"{currentWeapon.currentMag} / {currentWeapon.Mag}";
    }
}
