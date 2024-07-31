using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeClassInteraction : MonoBehaviour, IInteractable
{
    
    public bool isInfinite { get => true; }
    [SerializeField] GameObject canvas;
    public GameObject worldSpaceUI { get => canvas; }

    public bool canInteract { get; set; }

    public GameObject ClassChangeUI;

    public void DoInteraction()
    {
        ClassChangeUI.SetActive(true);
    }

    public void SetDescription()
    {

    }
}
