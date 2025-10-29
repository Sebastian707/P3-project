using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Interactable_Console : Interactable
{
    public List<GameObject> objectsToEnable = new List<GameObject>();



    protected override void Interact()
    {
        // Optional: toggle the light switch itself
  

        // Invert the active state of each object in the list
        foreach (var obj in objectsToEnable)
        {
            if (obj != null)
                obj.SetActive(!obj.activeSelf); // flip current state
        }
    }
}


