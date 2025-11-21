using UnityEngine;

public class SilhouetteSpotlightTrigger : MonoBehaviour
{
    [Header("Assign the spotlight GameObject here")]
    public GameObject spotlightObject;

    [Header("Assign an AudioSource with your sound here")]
    public AudioSource audioSource;

    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Turn on spotlight
            if (spotlightObject != null)
                spotlightObject.SetActive(true);

            // Play sound
            if (audioSource != null)
                audioSource.Play();

            // Disable only the collider so audio can finish playing
            if (triggerCollider != null)
                triggerCollider.enabled = false;
        }
    }
}