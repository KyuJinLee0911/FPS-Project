using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenInteraction : MonoBehaviour, IInteractable
{
    public GameObject worldSpaceUI { get; }

    public bool canInteract { get; set; }

    [SerializeField] protected Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
