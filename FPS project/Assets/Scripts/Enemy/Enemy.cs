using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature
{
    public override void Initialize()
    {
        hp = 100;
        defence = 0.1f;
        level = 1;
        autoCriticalRate = 0.0f;
        autoCriticalMagnification = 1f;
        isDead = false;
    }

    private void Start()
    {
        Initialize();
    }
    public override void Die()
    {

    }

    public override void TakeDamage(GameObject instigator, float damage)
    {
        base.TakeDamage(instigator, damage);
    }
}
