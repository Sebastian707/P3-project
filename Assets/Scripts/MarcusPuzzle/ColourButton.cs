using UnityEngine;

public class ColourButton : Interactable
{
    [Tooltip("Pick the platform color this button activates")]
    public PlatformColor buttonColour = PlatformColor.Red;

    [Header("Optional feedback")]
    public AudioClip pressSfx;
    public float pressAmount = 0.08f;
    public float pressTime = 0.12f;

    AudioSource audioSource;
    Vector3 initialLocalPos;
    bool isPressed = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        initialLocalPos = transform.localPosition;
    }

    // This is called by your player interaction system
    protected override void Interact()
    {
        Debug.Log("Hello World 2: Electric Boogaaloo");
        if (isPressed) return;
        StartCoroutine(ButtonPressAnim());

        // Tell the manager to change the active color
        ColourPlatformManager.Instance.SetActiveColor(buttonColour);
        Debug.Log($"Switched to: {buttonColour}");

        // Play feedback
        if (pressSfx && audioSource) audioSource.PlayOneShot(pressSfx);
    }

    System.Collections.IEnumerator ButtonPressAnim()
    {
        isPressed = true;
        Debug.Log("Hello World");
        float t = 0f;
        Vector3 target = initialLocalPos + Vector3.down * pressAmount;
        while (t < pressTime)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(initialLocalPos, target, t / pressTime);
            yield return null;
        }

        // hold briefly
        yield return new WaitForSeconds(0.08f);

        // return
        t = 0f;
        while (t < pressTime)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(target, initialLocalPos, t / pressTime);
            yield return null;
        }
        transform.localPosition = initialLocalPos;
        isPressed = false;
    }
}
