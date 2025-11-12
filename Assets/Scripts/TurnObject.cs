using UnityEngine;
using UnityEngine.UIElements;

public class TurnObject : Interactable
{
    public float turnSpeed; // degrees per second
    public float xAngle;
    public float yAngle;
    public float zAngle;
    public float duration = 2f; // duration of the rotation in seconds
    protected override void Interact()
    {
       gameObject.transform.Rotate(transform.rotation.x + xAngle, transform.rotation.y + yAngle, transform.rotation.z + zAngle);

        
    }
}
