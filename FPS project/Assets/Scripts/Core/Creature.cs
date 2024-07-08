using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

public interface IDamageable
{
    float hp { get; set; }
    float defence { get; set; }
    bool isDead { get; set; }
    void TakeDamage(GameObject instigator, float damage);
    void Die();
}
