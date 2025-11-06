// ColorPlatform.cs
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Renderer))]
public class ColourPlatform : MonoBehaviour
{
    public PlatformColor platformColor;
    public Material solidMaterial;
    public Material transparentMaterial;

    private Collider col;
    private Renderer rend;

    void Start()
    {
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
        UpdatePlatformState();
    }

    public void UpdatePlatformState()
    {
        Debug.Log("This is working");
        bool active = ColourPlatformManager.Instance.ActiveColor == platformColor;

        col.enabled = active;
        rend.sharedMaterial = active ? solidMaterial : transparentMaterial;
    }
}
