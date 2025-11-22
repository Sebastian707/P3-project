using UnityEngine;

public class EyeCloseController : MonoBehaviour
{
    [Header("Material (same one used in Renderer Feature)")]
    public Material eyeMaterial;

    [Header("Shader Property Names (match ShaderGraph references)")]
    public string eyesClosedProperty = "_Eyesclosed";
    public string smoothnessProperty = "_Smoothness";

    [Header("Eyes Closed Settings")]
    public float closeSpeed = 2f;
    public float openSpeed = 2f;

    [Header("Smoothness Auto-Link Settings")]
    [Range(0f, 1f)]
    public float maxSmoothness = 0.8f;   // smoothness when fully closed
    public AnimationCurve smoothnessCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Input")]
    public KeyCode toggleKey = KeyCode.B;

    private float targetEyeValue = 0f;
    private float currentEyeValue = 0f;

    void Update()
    {
        // ### Input ###
        if (Input.GetKeyDown(toggleKey))
            targetEyeValue = 1f;

        if (Input.GetKeyUp(toggleKey))
            targetEyeValue = 0f;

        // ### Eyes closed animation ###
        float speed = targetEyeValue > currentEyeValue ? closeSpeed : openSpeed;
        currentEyeValue = Mathf.MoveTowards(currentEyeValue, targetEyeValue, Time.deltaTime * speed);

        // ### Smoothness follows eye closing ###
        float smoothness = smoothnessCurve.Evaluate(currentEyeValue) * maxSmoothness;

        // ### Apply values ###
        if (eyeMaterial != null)
        {
            eyeMaterial.SetFloat(eyesClosedProperty, currentEyeValue);
            eyeMaterial.SetFloat(smoothnessProperty, smoothness);
        }
    }
}
