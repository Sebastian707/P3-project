using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class Interactable_Console : Interactable
{
    public List<GameObject> objectsToEnable = new List<GameObject>();
    public AudioClip interactSound;  
    public AudioSource audioSource;



    protected override void Interact()
    {
        audioSource.PlayOneShot(interactSound);
        foreach (var obj in objectsToEnable)
        {
            if (obj != null)
                obj.SetActive(!obj.activeSelf); 
        }
    }
}


