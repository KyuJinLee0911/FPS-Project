using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creature, IDamageable
{
    [SerializeField] private float hp = 100;
    public float Hp { get => hp; set => hp = value; }
    [SerializeField] private float defence = 0;
    public float Defence { get => defence; set => defence = value; }
    private bool isDead;
    public bool IsDead { get => isDead; set => isDead = value; }
    public event Action OnPlayerDamage;
    public event Action OnPlayerDie;

    public void TakeDamage(GameObject instigator, float damage)
    {
        Debug.Log("ouch!");
        // 캐릭터의 방어율이 100% 이하일때는 기본 데미지에서 방어율만큼의 데미지를 제외하고 차감, 100% 이상일때는 데미지가 1만 들어가도록 함
        float damageRate = defence < 1 ? 1 - defence : 1 / damage;
        hp -= damage * damageRate;
        // OnPlayerDamage();
        if (hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        OnPlayerDie();
    }

}
