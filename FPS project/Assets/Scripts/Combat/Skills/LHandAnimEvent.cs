using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LHandAnimEvent : MonoBehaviour
{
    [SerializeField] GameObject jav;
    void Start()
    {

    }

    public void Init()
    {
        if (transform.childCount == 0) return;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Javelin>() == null) continue;

            jav = transform.GetChild(i).gameObject;
            break;

        }
    }

    void EnableJav()
    {
        if (jav.activeSelf) return;
        jav.SetActive(true);
    }

    void OnThrow()
    {
        jav.transform.parent = null;
        Rigidbody javRb = jav.GetComponent<Rigidbody>();
        javRb.useGravity = true;
        Vector3 dirVec = jav.transform.up;
        javRb.AddForce(dirVec * 15, ForceMode.Impulse);
        // 왼손 위치 한손인지 두손인지 판별해서 옮김
        GameManager.Instance.playerFighter.ChangeLGripParent(GameManager.Instance.playerFighter.currentWeapon.isTwoHanded);
        StartCoroutine(AutoRegain());
    }

    IEnumerator AutoRegain()
    {
        yield return new WaitUntil(() => GameManager.Instance.player.subSkill.IsReady());

        jav.transform.SetParent(GameManager.Instance.controller.leftHandTransform);
        jav.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        jav.transform.GetComponent<Rigidbody>().useGravity = false;
        jav.SetActive(false);
    }

}
