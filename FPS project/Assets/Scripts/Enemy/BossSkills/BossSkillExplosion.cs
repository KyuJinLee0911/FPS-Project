using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BossSkillExplosion : MonoBehaviour
{
    [SerializeField] HitBox target;
    [SerializeField] float explosionForce;
    [SerializeField] float explosionRadious;
    [SerializeField] float expUpwardModifier;
    [SerializeField] Collider[] colliders;
    private void OnEnable()
    {
        target = GameManager.Instance.player.hitbox;

        colliders = Physics.OverlapSphere(transform.position, explosionRadious);

        foreach (var col in colliders)
        {
            if (col.attachedRigidbody == null) continue;
            if (!col.CompareTag("Player")) continue;
            if (col.gameObject.layer == 8 || col.gameObject.layer == 10 || col.gameObject.layer == 7) continue;

            Debug.Log("BOOM");
            if(target.enabled == false) return;
            col.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadious, expUpwardModifier);
            target.GetHit(transform.root.gameObject, 15);
        }
    }
}
