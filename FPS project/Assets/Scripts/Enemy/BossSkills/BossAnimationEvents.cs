using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationEvents : MonoBehaviour
{
    [SerializeField] GameObject trailR;
    [SerializeField] GameObject trailL;
    [SerializeField] Collider handColliderR;
    [SerializeField] Collider handColliderL;
    [SerializeField] GameObject explosionTrigger;
    [SerializeField] Boss boss;
    [SerializeField] GameObject bombGuy;
    GameObject newBombGuy = null;
    [SerializeField] Transform bombGuyTransfom;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] Vector3 explosionOffset;
    public void OnAttackStarted()
    {
        trailR.SetActive(true);
        handColliderR.enabled = true;
    }

    public void OnAttackEnded()
    {
        trailR.SetActive(false);
        handColliderR.enabled = false;
    }

    public void OnSkill5Started()
    {
        trailR.SetActive(true);
        trailL.SetActive(true);
        handColliderR.enabled = true;
        handColliderL.enabled = true;
    }

    public void OnHitGround()
    {
        explosionTrigger.SetActive(true);
    }

    public void OnSkill5Ended()
    {
        trailR.SetActive(false);
        trailL.SetActive(false);
        handColliderR.enabled = false;
        handColliderL.enabled = false;
        explosionTrigger.SetActive(false);
    }

    public void OnSelfHealed()
    {
        Debug.Log("Healing,,,");
        boss.SelfHeal();
    }

    public void OnSummonBombGuy()
    {
        newBombGuy = Instantiate(bombGuy, bombGuyTransfom);
    }

    public void LetGoBombGuy()
    {
        newBombGuy.transform.SetParent(null);
        newBombGuy.transform.rotation = Quaternion.identity;
        newBombGuy.GetComponent<BombGuy>().hitBox.root = newBombGuy.transform.root;
        newBombGuy = null;
    }
    
    public void OnBossDead()
    {
        GameObject newEffectObj = Instantiate(explosionEffect, boss.transform.position + explosionOffset, boss.transform.rotation);
        StartCoroutine(WaitForEndOfEffect(newEffectObj));
    }

    IEnumerator WaitForEndOfEffect(GameObject newEffectObj)
    {
        ParticleSystem expEffect = newEffectObj.GetComponent<ParticleSystem>();
        yield return new WaitUntil(() => expEffect.isStopped);

        if(expEffect.isStopped)
            Destroy(boss.gameObject);
    }
}
