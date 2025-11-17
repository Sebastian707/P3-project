using UnityEngine;

public class InteractablePT : Interactable
{
    public InspectSystem2 inspectSystem;

    public override void BaseInteract()
    {
        inspectSystem.ToggleInspect();
    }
}
