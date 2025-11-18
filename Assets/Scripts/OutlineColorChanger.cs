using UnityEngine;

public class OutlineColorChanger : MonoBehaviour
{
    public Material outlineMaterial; // Drag the Outline Material here in Inspector

    public void SetOutlineColor(Color newColor)
    {
        outlineMaterial.SetColor("_OutlineColor", newColor);
    }
}
