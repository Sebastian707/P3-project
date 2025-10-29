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
        float time = duration;
        while (time >= 0)
        {
            gameObject.transform.Rotate(0 + xAngle, yAngle, zAngle);
        }
    }
}
