using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public GameObject instigator = null;
    public float damage;
    public Transform root;

    private void OnCollisionEnter(Collision other)
    {
        if(other.collider.CompareTag(root.gameObject.tag))
            return;
        GetHit(instigator, damage);
    }

    void Init()
    {
        root = transform.root;
    }

    private void Start()
    {
        Init();
    }

    public float CalculateDamage(float damage, float damageMagnifier)
    {
        if (gameObject.layer == 10)
        {
            float criticalDamage = damage * damageMagnifier;
            Debug.Log("Critical!");
            return criticalDamage;
        }
        else if (gameObject.layer == 8)
        {
            float randomRate = Random.Range(0.0f, 1.0f);
            float autoCriticalDamage = damage * instigator.GetComponent<IStat>().autoCriticalMagnification;
            float autoCriticalRate = instigator.GetComponent<IStat>().autoCriticalRate;

            if (randomRate < autoCriticalRate)
            {
                Debug.Log($"Auto Critical! Rate : {randomRate}");
                return autoCriticalDamage;
            }
            else
                return damage;
        }
        return 0;
    }

    public void GetHit(GameObject instigator, float damage)
    {
        IDamageable damageable = root.GetComponent<IDamageable>();
        if (damageable == null || damageable.isDead) return;

        damageable.TakeDamage(instigator, damage);
    }
}
