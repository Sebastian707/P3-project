using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class TurnObject : Interactable
{
    
    public float xAngle;
    public float yAngle;
    public float zAngle;
    public float duration = 2f; // duration of the rotation in seconds
    private bool isRotating = false;
    protected override void Interact()
    {
        //gameObject.transform.Rotate(transform.rotation.x + xAngle, transform.rotation.y + yAngle, transform.rotation.z + zAngle);
        if (!isRotating) 
        {
            StartCoroutine(ObjectRotation()); 
        }
            

    }
    public  IEnumerator ObjectRotation() 
    { 
        isRotating = true;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(transform.eulerAngles.x + xAngle, transform.eulerAngles.y + yAngle, transform.eulerAngles.z + zAngle);
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endRotation;
        isRotating = false;
    } 
        
}
