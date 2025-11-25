using UnityEngine;
using System.Collections;

public class SilhouettePuzzleManager : MonoBehaviour
{
    [Header("Puzzle Sequence")]
    [SerializeField] private SilhouettePuzzle firstPuzzle;
    [SerializeField] private SilhouettePuzzle secondPuzzle;

    [Header("Spotlight & Audio")]
    [SerializeField] private GameObject secondPuzzleSpotlight;
    [SerializeField] private GameObject doorSpotlight;
    [SerializeField] private AudioSource spotlightAudio;

    [Header("Door")]
    [SerializeField] private GameObject door;
    [SerializeField] private float doorOpenYOffset = 5f;
    [SerializeField] private float doorAnimationDuration = 2f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        // Ensure spotlights start inactive
        if (secondPuzzleSpotlight != null)
            secondPuzzleSpotlight.SetActive(false);
        
        if (doorSpotlight != null)
            doorSpotlight.SetActive(false);

        // Ensure second puzzle starts inactive
        if (secondPuzzle != null)
            secondPuzzle.gameObject.SetActive(false);
    }

    // Call this from SilhouettePuzzle when first puzzle completes
    public void OnFirstPuzzleComplete()
    {
        StartCoroutine(FirstPuzzleCompleteSequence());
    }

    private IEnumerator FirstPuzzleCompleteSequence()
    {
        // Fade out first puzzle
        if (firstPuzzle != null)
        {
            yield return StartCoroutine(FadeOutAndDisable(firstPuzzle.gameObject));
        }

        // Activate second puzzle immediately
        if (secondPuzzle != null)
            secondPuzzle.gameObject.SetActive(true);

        // Play spotlight sound (non-blocking)
        if (spotlightAudio != null)
            spotlightAudio.Play();

        // Activate second puzzle spotlight (non-blocking)
        if (secondPuzzleSpotlight != null)
            secondPuzzleSpotlight.SetActive(true);
    }

    public void OnSecondPuzzleComplete()
    {
        StartCoroutine(SecondPuzzleCompleteSequence());
    }

    private IEnumerator SecondPuzzleCompleteSequence()
    {
        // Fade out second puzzle
        if (secondPuzzle != null)
        {
            yield return StartCoroutine(FadeOutAndDisable(secondPuzzle.gameObject));
        }

        // Play spotlight sound (non-blocking)
        if (spotlightAudio != null)
            spotlightAudio.Play();

        // Activate door spotlight (non-blocking)
        if (doorSpotlight != null)
            doorSpotlight.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        // Open door by moving it up
        if (door != null)
        {
            Vector3 currentPos = door.transform.position;
            Vector3 openPos = currentPos;
            openPos.y += doorOpenYOffset;
            yield return StartCoroutine(MoveDoor(currentPos, openPos, doorAnimationDuration));
        }

        Debug.Log("Puzzle sequence complete!");
    }

    private IEnumerator FadeOutAndDisable(GameObject target)
    {
        MeshRenderer renderer = target.GetComponent<MeshRenderer>();
        if (renderer == null) yield break;

        Material mat = renderer.material;
        Color originalColor = mat.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            
            Color col = originalColor;
            col.a = Mathf.Lerp(1f, 0f, t);
            mat.color = col;

            yield return null;
        }

        Color final = originalColor;
        final.a = 0f;
        mat.color = final;
        renderer.enabled = false;
        target.SetActive(false);
    }

    private IEnumerator MoveDoor(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);

            Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
            door.transform.position = newPos;

            yield return null;
        }

        door.transform.position = endPos;
    }
}