using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagText : MonoBehaviour
{
    public Text magText;
    private WeaponData currentWeapon;


    void Start()
    {
        currentWeapon = GameManager.Instance.playerFighter.currentWeapon;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWeapon != GameManager.Instance.playerFighter.currentWeapon)
            currentWeapon = GameManager.Instance.playerFighter.currentWeapon;
        magText.text = $"{currentWeapon.currentMag} / {currentWeapon.totalMag}";
    }
}
