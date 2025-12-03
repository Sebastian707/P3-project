using UnityEngine;

public class InspectSystem : MonoBehaviour
{
    [Header("References")]
    public Transform inspectPosition;

    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 0.5f;
    public float minZoom = 0.5f;
    public float maxZoom = 2.0f;

    [Header("Movement Settings")]
    public Vector3 inspectScale = Vector3.one;

    private Transform currentObject;
    private float rotationX = 0f;
    private float rotationY = 0f;
    private float targetZoom = 1f;
    private float currentZoom = 1f;
    private bool inspecting = false;

    void Update()
    {
        if (!inspecting || !currentObject) return;

        HandleMouseRotation();
        HandleZoom();
        ApplyTransforms();

        if (Input.GetMouseButtonDown(1))
            ResetRotation();

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
            EndInspect();
    }

    private void HandleMouseRotation()
    {
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Accumulate rotation based on mouse movement
            rotationY += mouseX * rotationSpeed * Time.unscaledDeltaTime;
            rotationX -= mouseY * rotationSpeed * Time.unscaledDeltaTime;
        }
    }

    private void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0f)
        {
            targetZoom -= scroll * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }
    }

    private void ApplyTransforms()
    {
        // Apply rotation using accumulated Euler angles
        currentObject.rotation = Quaternion.Euler(rotationX, rotationY, 0f);

        // Smooth zoom
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, 10f * Time.unscaledDeltaTime);

        // Lock position
        currentObject.position = inspectPosition.position;

        // Apply scale
        currentObject.localScale = inspectScale * currentZoom;
    }

    private void ResetRotation()
    {
        rotationX = 0f;
        rotationY = 0f;
    }

    public void StartInspect(Transform obj)
    {
        if (inspecting) return;

        inspecting = true;

        // Create clone
        GameObject clone = Instantiate(obj.gameObject);
        clone.name = obj.name + "_InspectClone";

        // Remove physics components
        foreach (var col in clone.GetComponentsInChildren<Collider>())
            Destroy(col);
        foreach (var rb in clone.GetComponentsInChildren<Rigidbody>())
            Destroy(rb);

        // Set layer to ignore raycasts
        clone.layer = LayerMask.NameToLayer("Ignore Raycast");

        currentObject = clone.transform;
        currentObject.position = inspectPosition.position;
        currentObject.rotation = Quaternion.identity;
        currentObject.localScale = inspectScale;

        // Reset rotation values
        rotationX = 0f;
        rotationY = 0f;

        targetZoom = 1f;
        currentZoom = 1f;

        Time.timeScale = 0f;
    }

    public void EndInspect()
    {
        Time.timeScale = 1f;

        if (currentObject)
            Destroy(currentObject.gameObject);

        currentObject = null;
        inspecting = false;
    }
}