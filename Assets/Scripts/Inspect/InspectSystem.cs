using UnityEngine;

public class InspectSystem2 : MonoBehaviour
{
    public Transform objectToInspect;
    
    public float rotationSpeed = 100f;

    private Vector3 previousMousePosition;

    private bool isInspecting = false;

    public void ToggleInspect()
    {
        isInspecting = !isInspecting;
        Debug.Log("Inspect mode: " + isInspecting);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previousMousePosition = Input.mousePosition;
        }
        
        if(Input.GetMouseButton(0)) 
            {
                Vector3 deltaMousePosition = Input.mousePosition - previousMousePosition;
                float rotationX = deltaMousePosition.y * rotationSpeed * Time.deltaTime;
                float rotationY = -deltaMousePosition.x * rotationSpeed * Time.deltaTime;

            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
            objectToInspect.rotation = rotation * objectToInspect.rotation;

            previousMousePosition = Input.mousePosition;

            // Exit inspect on E
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleInspect();
            }
    
    }

}

