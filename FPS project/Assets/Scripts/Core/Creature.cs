using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : MonoBehaviour, IDamageable, IStat
{
    public float autoCriticalRate { get; set; }
    public float autoCriticalMagnification { get; set; }
    public int level { get; set; }
    public float hp { get; set; }
    public float defence { get; set; }
    public bool isDead { get; set; }
    public int exp { get; set; }

    public abstract void Initialize();

    public virtual void Die(GameObject instigator)
    {
        if (isDead) return;
        Debug.Log("Dead!!!!!!");
        isDead = true;
    }

    public virtual void TakeDamage(GameObject instigator, float damage)
    {
        // 캐릭터의 방어율이 100% 이하일때는 기본 데미지에서 방어율만큼의 데미지를 제외하고 차감, 100% 이상일때는 데미지가 1만 들어가도록 함
        float damageRate = defence < 1 ? 1 - defence : 1 / damage;
        hp -= damage * damageRate;
        // OnPlayerDamage();
        if (hp <= 0)
        {
            Die(instigator);
        }
        Debug.Log($"ouch! HP : {hp}");
    }
}

public interface IDamageable
{
    float hp { get; set; }
    float defence { get; set; }
    bool isDead { get; set; }
    void TakeDamage(GameObject instigator, float damage);
    void Die(GameObject instigator);
}

public interface IStat
{
    float autoCriticalRate { get; set; }
    float autoCriticalMagnification { get; set; }
    int level { get; set; }
    int exp { get; set; }
}
