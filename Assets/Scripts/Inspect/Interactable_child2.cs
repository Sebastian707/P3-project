using UnityEngine;

public class Interactable_child2 : Interactable
{
    [SerializeField] private InspectSystem2 inspectSystem;

    void Start()
    {
        // Optional: auto-assign if not set in inspector
        if (inspectSystem == null)
        {
            inspectSystem = Object.FindAnyObjectByType<InspectSystem2>();
        }
    }

    protected override void Interact()
    {
        if (inspectSystem != null)
        {
            inspectSystem.StartInspect(transform);
        }
        else
        {
            Debug.LogWarning("InspectSystem2 not assigned on " + gameObject.name);
        }
    }
}