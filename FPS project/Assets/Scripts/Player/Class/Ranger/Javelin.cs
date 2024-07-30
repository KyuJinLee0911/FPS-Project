using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Javelin : MonoBehaviour, IInteractable
{
    Vector3 initialPos;
    Quaternion initialRot;
    public float javDamage = 75;

    public GameObject worldSpaceUI {get; set;}
    public bool canInteract { get; set; }

    private void Awake()
    {
        worldSpaceUI = null;
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
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

    public void DoInteraction()
    {
        transform.SetParent(GameManager.Instance.controller.leftHandTransform);
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        transform.GetComponent<Rigidbody>().useGravity = false;
        transform.localPosition = initialPos;
        transform.localRotation = initialRot;
        gameObject.SetActive(false);
        
    }

    public void SetDescription()
    {
        if(worldSpaceUI == null) return;
    }
}
