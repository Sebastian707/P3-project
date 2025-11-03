using UnityEngine;

public class SilhouettePuzzle : Interactable
{
    [Header("Float Settings")]
    [SerializeField] private float floatHeight = 2f;
    [SerializeField] private float floatSpeed = 2f;
    
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f;
    
    private bool isFloating = false;
    private bool canRotate = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float floatProgress = 0f;
    
    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.up * floatHeight;
    }
    
    protected override void Interact()
    {
        if (!isFloating)
        {
            isFloating = true;
            floatProgress = 0f;
        }
    }
    
    private void Update()
    {
        if (isFloating)
        {
            HandleFloating();
        }
        
        if (canRotate)
        {
            HandleRotation();
        }
    }
    
    private void HandleFloating()
    {
        floatProgress += Time.deltaTime * floatSpeed;
        transform.position = Vector3.Lerp(startPosition, targetPosition, floatProgress);
        
        if (floatProgress >= 1f)
        {
            canRotate = true;
        }
    }
    
    private void HandleRotation()
    {
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            
            transform.Rotate(Vector3.up, -mouseX * rotationSpeed, Space.World);
            transform.Rotate(Vector3.right, mouseY * rotationSpeed, Space.World);
        }
    }
}