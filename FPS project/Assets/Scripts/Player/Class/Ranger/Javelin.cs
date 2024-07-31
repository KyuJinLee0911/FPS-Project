using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Javelin : MonoBehaviour, IInteractable
{
    
    public bool isInfinite { get => false; }
    Vector3 initialPos;
    Quaternion initialRot;
    public float javDamage = 75;

    public GameObject worldSpaceUI { get; set; }
    public bool canInteract { get; set; }

    private void Awake()
    {
        worldSpaceUI = null;
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
        GameManager.Instance.onSceneChange += OnChangeScene;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                other.GetComponent<IDamageable>().TakeDamage(GameManager.Instance.player.transform.gameObject, javDamage);
                Vector3 damageDirVec = (other.transform.position - transform.position).normalized;
                other.attachedRigidbody.AddForce(damageDirVec * 10, ForceMode.Impulse);
                break;

            case "Environment":
                transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
                break;
        }
    }

    private void OnDisable()
    {
        transform.localPosition = initialPos;
        transform.localRotation = initialRot;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void DoInteraction()
    {
        transform.SetParent(GameManager.Instance.controller.leftHandTransform);
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        transform.GetComponent<Rigidbody>().useGravity = false;
        gameObject.SetActive(false);
    }

    public void OnChangeScene()
    {
        if(GameManager.Instance.controller.leftHandTransform.childCount != 0) return;

        // 씬이 바뀔때 쿨타임을 초기화해서 자동으로 자벨린 회수할 수 있도록 함
        GameManager.Instance.player.subSkill.currentCoolTime = 0;
    }


    public void SetDescription()
    {
        if (worldSpaceUI == null) return;
    }
}
