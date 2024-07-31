using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionRaycast : MonoBehaviour
{

    [SerializeField] GameObject worldSpaceUI = null;
    IInteractable interactable = null;
    private void Update()
    {
        CheckInteraction();
    }

    void CheckInteraction()
    {
        Ray cameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;

        if (Physics.Raycast(cameraRay, out hit, 2.5f))
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
        // 새로운 UI 캔버스에 접근하기 전에
        // 이전 UI 캔버스가 할당되어 있다면 할당 해제
        if (worldSpaceUI != null)
            worldSpaceUI = null;
        interactable = hit.collider.GetComponent<IInteractable>();
        if (interactable.worldSpaceUI != null)
        {

            worldSpaceUI = interactable.worldSpaceUI;
            worldSpaceUI.SetActive(true);
            interactable.SetDescription();
        }

        interactable.canInteract = true;
    }

    private void DisableWorldSpaceUI()
    {
        if (interactable == null) return;
        interactable.canInteract = false;
        if (interactable.isInfinite) return;
        if (worldSpaceUI == null) return;

        worldSpaceUI.SetActive(false);
        worldSpaceUI = null;
        interactable = null;
    }

    void OnInteraction()
    {
        if (interactable.canInteract)
            interactable.DoInteraction();
    }
}
