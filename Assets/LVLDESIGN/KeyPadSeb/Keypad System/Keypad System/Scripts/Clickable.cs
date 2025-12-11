using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : Interactable
{

    protected override void Interact()
    {

        // Send the "But" message to this button when E is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameObject.SendMessage("But", SendMessageOptions.DontRequireReceiver);
        }
    }


}