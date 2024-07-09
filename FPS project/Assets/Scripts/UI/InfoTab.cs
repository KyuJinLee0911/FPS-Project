using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTab : MonoBehaviour
{
    public GameObject weaponTab;
    public GameObject itemtab;
    private void OnEnable()
    {
        itemtab.SetActive(false);
        weaponTab.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDisable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;   
    }
}
