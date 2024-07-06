using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Fighter : MonoBehaviour
{
    [SerializeField] private WeaponData currentWeapon;
    public WeaponData CurrentWeapon { get => currentWeapon; }
    [SerializeField] private bool isWeaponFire = false;
    [SerializeField] private float timeSinceLastFire = 0f;
    [SerializeField] private IDamageable target;
    [SerializeField] Transform muzzleTransform;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isWeaponFire)
            Fire();
    }

    void OnFire(InputValue value)
    {
        if(value.Get<float>() == 1)
        {
            isWeaponFire = true;
        }
        else
        {
            timeSinceLastFire = 0;
            isWeaponFire = false;
        }
    }

    void Fire()
    {
        float _timeBetweenFires = 1 / currentWeapon.FireRate;

        timeSinceLastFire += Time.deltaTime;
        if(timeSinceLastFire >= _timeBetweenFires)
        {
            currentWeapon.FireArm(muzzleTransform, gameObject);
            timeSinceLastFire = 0;
        }
    }
}