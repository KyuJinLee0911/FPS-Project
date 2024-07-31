using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFist : MonoBehaviour
{
    HitBox target = null;
    [SerializeField] GameObject trail;
    [SerializeField] float fistDamage;
    private void OnCollisionEnter(Collision other)
    {
        target = GameManager.Instance.player.hitbox;

        if (!other.collider.CompareTag("Player")) return;
        target = other.collider.transform.GetComponent<HitBox>();
        if (target == null) return;

        if(target.enabled == false) return;
        target.instigator = transform.root.gameObject;
        target.damage = fistDamage;
    }
}
