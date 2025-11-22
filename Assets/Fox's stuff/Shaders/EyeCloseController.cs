using UnityEngine;

public class EyeCloseController : MonoBehaviour
{
    [Header("Material (same one used in your Renderer Feature)")]
    public Material eyeMaterial;

    [Header("Shader Property Names")]
    public string eyeCloseProperty = "_Eyesclosed"; // Confirm this is correct in ShaderGraph

    [Header("Animation Settings")]
    public float closeSpeed = 2f;     // speed of closing
    public float openSpeed = 2f;      // speed of opening

    [Header("Input")]
    public KeyCode toggleKey = KeyCode.B; // press B to blink

    private float targetValue = 0f;   // 0 = open, 1 = closed
    private float currentValue = 0f;

    void Update()
    {
        // Toggle blink with key
        if (Input.GetKeyDown(toggleKey))
        {
            targetValue = 1f; // close
        }
        if (Input.GetKeyUp(toggleKey))
        {
            targetValue = 0f; // open
        }

        // Smooth animation
        float speed = targetValue > currentValue ? closeSpeed : openSpeed;

        currentValue = Mathf.MoveTowards(
            currentValue,
            targetValue,
            Time.deltaTime * speed
        );

        // Apply to shader
        if (eyeMaterial != null)
        {
            eyeMaterial.SetFloat(eyeCloseProperty, currentValue);
        }
    }
}
