using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDoor : MonoBehaviour, IInteractable
{
    public GameObject worldSpaceUI { get; set; }

    public bool canInteract { get; set; }

    public void DoInteraction()
    {
        GameManager.Instance.BeginPlay();
    }

    public void SetDescription()
    {

    }
}
