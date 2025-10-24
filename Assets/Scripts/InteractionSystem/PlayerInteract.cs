using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using Xenon;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float distance = 3f;

    private Camera cam;
    private PlayerUI playerUI;

    private Interactable currentInteractable;
    private int originalLayer;

    void Start()
    {
     
        cam = GetComponent<PlayerController>().playerCamera;
        playerUI = GetComponent<PlayerUI>();
    }

    void Update()
    {
        playerUI.UpdateText(string.Empty);

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.green);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                // Show the interaction prompt
                playerUI.UpdateText(interactable.promptMessage);

                // Handle layer switching for outline
                if (interactable != currentInteractable)
                {
                    ResetCurrentInteractable(); // Reset previous object
                    SetOutlineLayer(interactable);
                    currentInteractable = interactable;
                }

                // Interact when pressing E
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.BaseInteract();
                }
            }
            else
            {
                ResetCurrentInteractable();
            }
        }
        else
        {
            ResetCurrentInteractable();
        }
    }

    private void SetOutlineLayer(Interactable interactable)
    {
        originalLayer = interactable.gameObject.layer; // store original layer
        int outlineLayer = LayerMask.NameToLayer("Outline");
        if (outlineLayer != -1)
        {
            interactable.gameObject.layer = outlineLayer;
        }
        else
        {
            Debug.LogWarning("Layer 'Outline' does not exist. Please add it in the Layer Manager.");
        }
    }

    private void ResetCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable.gameObject.layer = originalLayer; // restore original layer
            currentInteractable = null;
        }
    }
}
