using UnityEngine;

public class BookPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public Transform holdPoint;       // empty child of the player camera
    public float holdSmooth = 10f;

    private Camera cam;
    private Rigidbody heldBook;
    private bool holding = false;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!holding) TryPickup();
            else DropBook();
        }

        if (holding && heldBook)
        {
            // Smooth follow to hold point
            Vector3 targetPos = holdPoint.position;
            heldBook.MovePosition(Vector3.Lerp(heldBook.position, targetPos, Time.deltaTime * holdSmooth));
        }
    }

    /*
    void TryPickup()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            if (hit.collider.CompareTag("Book"))
            {
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb == null)
                {
                    Debug.LogWarning($"Book '{hit.collider.name}' has no Rigidbody attached!");
                    return;
                }

                heldBook = rb;
                heldBook.useGravity = false;
                heldBook.linearDamping = 10;
                heldBook.constraints = RigidbodyConstraints.FreezeRotation;
                holding = true;
            }
        }
    }
    */
    void TryPickup()
    {
        if (cam == null)
        {
            Debug.LogError("Camera not assigned or missing MainCamera tag!");
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            Debug.Log("Ray hit: " + hit.collider.name);

            if (hit.collider.CompareTag("Book"))
            {
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb == null)
                {
                    Debug.LogError("Book object has no Rigidbody on same GameObject as its collider!");
                    return;
                }

                heldBook = rb;
                heldBook.useGravity = false;
                heldBook.linearDamping = 10f;
                heldBook.constraints = RigidbodyConstraints.FreezeRotation;
                holding = true;
            }
        }
    }

    /*
    void DropBook()
    {
        if (heldBook)
        {
            heldBook.useGravity = true;
            heldBook.linearDamping = 1;
            heldBook.constraints = RigidbodyConstraints.None;
            heldBook = null;
        }
        holding = false;
    }
    */

    void DropBook()
    {
        if (heldBook)
        {
            // Find nearby slot
            Collider[] hits = Physics.OverlapSphere(heldBook.position, 0.3f);
            Transform nearestSlot = null;
            float minDist = float.MaxValue;

            foreach (var h in hits)
            {
                if (h.name.StartsWith("Slot_"))
                {
                    float dist = Vector3.Distance(heldBook.position, h.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearestSlot = h.transform;
                    }
                }
            }

            if (nearestSlot != null)
            {
                // Snap to slot position
                heldBook.position = nearestSlot.position;
                heldBook.rotation = nearestSlot.rotation;
                heldBook.useGravity = false;
                heldBook.linearDamping = 10f;
                heldBook.constraints = RigidbodyConstraints.FreezeAll;

                Debug.Log($"Book placed in {nearestSlot.name}");
            }
            else
            {
                // Normal drop
                heldBook.useGravity = true;
                heldBook.linearDamping = 1;
                heldBook.constraints = RigidbodyConstraints.None;
            }

            heldBook = null;
        }

        holding = false;
    }

}
