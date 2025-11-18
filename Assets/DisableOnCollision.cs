using System.Collections.Generic;
using UnityEngine;

public class EnableOnCollision : MonoBehaviour
{
    [SerializeField] private string targetTag = "Target";
    private HashSet<Renderer> affectedRenderers = new HashSet<Renderer>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Renderer rend = other.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.enabled = true; // now visible inside the trigger
                affectedRenderers.Add(rend);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Renderer rend = other.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.enabled = false; // invisible outside the trigger
                affectedRenderers.Remove(rend);
            }
        }
    }

    private void OnDisable()
    {
        // Hide all renderers if this object is disabled
        foreach (var rend in affectedRenderers)
        {
            if (rend != null)
                rend.enabled = false;
        }
        affectedRenderers.Clear();
    }
}
