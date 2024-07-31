using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenInteraction : MonoBehaviour, IInteractable
{
    public bool isInfinite { get => false; }
    [SerializeField] protected GameObject uiObj;
    public GameObject worldSpaceUI { get; protected set; }

    public bool canInteract { get; set; }

    [SerializeField] protected Animator animator;

    private void Start()
    {
        if (animator == null)
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
