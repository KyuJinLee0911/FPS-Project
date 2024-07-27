using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill : MonoBehaviour
{
    [SerializeField] HitBox targetHitBox;
    [SerializeField] float damagePerTick;
    [SerializeField] int maximumTick;
    private IEnumerator coroutine;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        StartCoroutine(coroutine);
    }

    private void OnTriggerExit(Collider other)
    {
        StopCoroutine(coroutine);
    }

    private void OnDisable()
    {
        StopCoroutine(coroutine);
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        coroutine = DoDamage(targetHitBox);
        Vector3 targetDir = (targetHitBox.transform.position - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(targetDir, Vector3.right);
        float xRot = Quaternion.Slerp(transform.rotation, targetRot, 1.0f).eulerAngles.x;
        xRot = Mathf.Clamp(xRot, -20, 20);
        transform.Rotate(Vector3.right, xRot);

    }

    // 8틱(2초)를 다 채우거나 플레이어가 콜라이더에서 나가면 멈춤
    private IEnumerator DoDamage(HitBox target)
    {
        int tick = 0;
        if (target == null)
            yield break;
        while (tick < maximumTick)
        {
            target.GetHit(transform.root.gameObject, damagePerTick);
            tick++;
            yield return new WaitForSeconds(0.25f);
        }
    }
}
