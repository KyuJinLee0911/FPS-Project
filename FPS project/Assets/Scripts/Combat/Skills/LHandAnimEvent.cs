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
        if (transform.childCount != 0)
            jav = transform.GetChild(0).gameObject;
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
