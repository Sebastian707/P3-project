using UnityEngine;

public enum CustomVolumeBlendMode
{
    Global,
    Local
}

public class CustomPassVolume : MonoBehaviour
{
    public CustomVolumeBlendMode blendMode = CustomVolumeBlendMode.Local;

    [Range(0f, 1f)]
    public float weight = 1f;

    public float priority = 0f;

    [Header("Volume Bounds (Local Only)")]
    public Vector3 size = Vector3.one;

    [Header("Effect Parameters")]
    [Range(0f, 1f)]
    public float closeAmount = 0f;

    [Range(0f, 1f)]
    public float smoothness = 0.2f;

    public bool IsCameraInside(Vector3 camPos)
    {
        if (blendMode == CustomVolumeBlendMode.Global)
            return true;

        Bounds b = new Bounds(transform.position, size);
        return b.Contains(camPos);
    }
}
