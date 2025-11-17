using UnityEngine;

public class InspectSystem2 : MonoBehaviour
{
    public Transform inspectPosition;   // empty transform in front of camera
    public float rotationSpeed = 100f;

    [SerializeField] private Vector3 inspectScale = Vector3.one;

    private Transform currentObject;
    private Vector3 previousMousePos;
    private Transform originalParent;
    private bool inspecting = false;

    void Update()
    {
        if (!inspecting) return;

        // Rotate object while holding left mouse
        if (Input.GetMouseButtonDown(0))
            previousMousePos = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - previousMousePos;
            float rotX = delta.y * rotationSpeed * Time.deltaTime;
            float rotY = -delta.x * rotationSpeed * Time.deltaTime;

            currentObject.Rotate(Camera.main.transform.up, rotY, Space.World);
            currentObject.Rotate(Camera.main.transform.right, rotX, Space.World);

            previousMousePos = Input.mousePosition;
        }

        // Exit inspect
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        {
            EndInspect();
        }
    }

    

    public void StartInspect(Transform obj)
    {
        if (inspecting) return;

        inspecting = true;

        GameObject clone = Instantiate(obj.gameObject);
        clone.name = obj.name + "_InspectClone";

        if (clone.GetComponent<Collider>()) Destroy(clone.GetComponent<Collider>());
        if (clone.GetComponent<Rigidbody>()) Destroy(clone.GetComponent<Rigidbody>());

        currentObject = clone.transform;
        currentObject.SetParent(inspectPosition);
        currentObject.localPosition = Vector3.zero;
        currentObject.localRotation = Quaternion.identity;
        currentObject.localScale = inspectScale;
        
    }


    public void EndInspect()
    {
        if (!inspecting) return;

        Destroy(currentObject.gameObject); // destroy the clone
        currentObject = null;
        inspecting = false;
    }
    



}
