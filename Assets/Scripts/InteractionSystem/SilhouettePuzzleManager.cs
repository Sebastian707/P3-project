using UnityEngine;
using System.Collections;

public class SilhouettePuzzleManager : MonoBehaviour
{
    [Header("Puzzle Sequence")]
    [SerializeField] private SilhouettePuzzle firstPuzzle;
    [SerializeField] private SilhouettePuzzle secondPuzzle;

    [Header("Spotlight & Audio")]
    [SerializeField] private GameObject firstSpotlight;
    [SerializeField] private GameObject secondSpotlight;
    [SerializeField] private AudioSource spotlightAudio;

    [Header("Door")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string doorOpenBoolName = "IsOpen";

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        // Ensure second puzzle starts inactive
        if (secondPuzzle != null)
            secondPuzzle.gameObject.SetActive(false);

        if (firstSpotlight != null)
            firstSpotlight.SetActive(true);
    }

    // Call this from SilhouettePuzzle when puzzle completes
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

        yield return new WaitForSeconds(0.5f);

        // Deactivate first spotlight
        if (firstSpotlight != null)
            firstSpotlight.SetActive(false);

        // Play spotlight sound
        if (spotlightAudio != null)
            spotlightAudio.Play();

        // Activate second puzzle
        if (secondPuzzle != null)
            secondPuzzle.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        // Activate second spotlight
        if (secondSpotlight != null)
            secondSpotlight.SetActive(true);
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

        yield return new WaitForSeconds(0.5f);

        // Deactivate second spotlight
        if (secondSpotlight != null)
            secondSpotlight.SetActive(false);

        // Open door
        if (doorAnimator != null)
            doorAnimator.SetBool(doorOpenBoolName, true);

        yield return new WaitForSeconds(0.5f);

        // Play spotlight sound
        if (spotlightAudio != null)
            spotlightAudio.Play();
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
}