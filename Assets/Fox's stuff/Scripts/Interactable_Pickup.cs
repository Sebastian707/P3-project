using UnityEngine;

public class Interactable_Pickup : Interactable
{
    [Header("Pickup Settings")]
    public Transform holdPoint;
    public float holdSmooth = 12f;
    public float dropDistance = 0.8f;   // slightly farther so it doesn't clip into player

    private Rigidbody rb;
    private static Rigidbody heldObject;
    private static bool holding = false;
    private Collider playerCollider;    // reference to player for ignore collision

    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.LogWarning($"Added missing Rigidbody to {gameObject.name}");
        }
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Try to find the player collider to ignore collision
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
            playerCollider = player.GetComponent<Collider>();
    }

    protected void Update()
    {
        if (holding && heldObject == rb && holdPoint != null)
        {
            Vector3 targetPos = holdPoint.position;
            rb.MovePosition(Vector3.Lerp(rb.position, targetPos, Time.deltaTime * holdSmooth));
        }
    }

    protected override void Interact()
    {
        if (!holding)
        {
            Pickup();
        }
        else if (heldObject == rb)
        {
            Drop();
        }
    }

    private void Pickup()
    {
        if (holding || holdPoint == null) return;

        heldObject = rb;
        holding = true;

        rb.useGravity = false;
        rb.linearDamping = 12f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Ignore collision between object and player
        if (playerCollider && rb.TryGetComponent(out Collider objectCol))
            Physics.IgnoreCollision(playerCollider, objectCol, true);

        Debug.Log($"Picked up {gameObject.name}");
    }

    private void Drop()
    {
        if (heldObject == null) return;

        rb.useGravity = true;
        rb.linearDamping = 1f;
        rb.constraints = RigidbodyConstraints.None;

        // Drop a bit in front of the player to prevent overlap
        rb.position = holdPoint.position + holdPoint.forward * dropDistance;

        // Re-enable collision with player
        if (playerCollider && rb.TryGetComponent(out Collider objectCol))
            Physics.IgnoreCollision(playerCollider, objectCol, false);

        heldObject = null;
        holding = false;

        Debug.Log($"Dropped {gameObject.name}");
    }
}
