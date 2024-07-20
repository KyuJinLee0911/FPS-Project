using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] protected GameObject uiObj;
    public GameObject worldSpaceUI { get; protected set;}

    public bool canInteract { get; set; }

    [SerializeField] protected Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        worldSpaceUI = uiObj;
    }

    public virtual void DoInteraction()
    {
        Debug.Log("Open Sesame");
        animator.Play("Open");
    }

    public void SetDescription()
    {

    }


}
