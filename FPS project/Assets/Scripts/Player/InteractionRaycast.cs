using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionRaycast : MonoBehaviour
{

    GameObject worldSpaceUI = null;
    IInteractable interactable = null;
    private void Update()
    {
        CheckInteraction();
    }

    void CheckInteraction()
    {
        Ray cameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;

        if (Physics.Raycast(cameraRay, out hit, 5))
        {
            if (hit.collider.GetComponent<IInteractable>() != null)
            {
                EnableWorldSpaceUI(hit);
            }
            else
            {
                DisableWorldSpaceUI();
                return;

            }
        }
        else
        {
            DisableWorldSpaceUI();
            return;
        }
    }

    private void EnableWorldSpaceUI(RaycastHit hit)
    {
        interactable = hit.collider.GetComponent<IInteractable>();
        worldSpaceUI = interactable.worldSpaceUI;
        worldSpaceUI.SetActive(true);
        interactable.canInteract = true;
    }

    private void DisableWorldSpaceUI()
    {
        if (interactable == null) return;

        interactable.canInteract = false;
        worldSpaceUI.SetActive(false);
        worldSpaceUI = null;
        interactable = null;
    }

    void OnInteraction()
    {
        interactable.DoInteraction();
    }
}
