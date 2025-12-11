using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // If using TextMeshPro for the screen text

public class Press : Clickable
{

    [Header("Button Settings")]
    public string buttonValue; // Assign "0"-"9" in inspector for each button'
    private AudioSource audioSource;
    public AudioClip ClickSound;

    [Header("Keypad Reference")]
    public KeypadManager keypadManager; // Assign the main keypad manager here

    private void Awake() 
    { 
        audioSource = GetComponent<AudioSource>(); 
    }

    protected override void Interact()
    {
        audioSource.PlayOneShot(ClickSound);
        // When this button is clicked, tell the keypad manager
        keypadManager.ButtonPressed(buttonValue);
    }
}
