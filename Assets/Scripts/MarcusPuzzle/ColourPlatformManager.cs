// ColourPlatformManager.cs
using UnityEngine;

public enum PlatformColor { Red, Green, Blue, Yellow }

public class ColourPlatformManager : MonoBehaviour
{
    public static ColourPlatformManager Instance;
    public PlatformColor ActiveColor = PlatformColor.Red;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetActiveColor(PlatformColor newColor)
    {
        ActiveColor = newColor;
        // Notify all color platforms
        foreach (var platform in FindObjectsOfType<ColourPlatform>())
            platform.UpdatePlatformState();
    }

    // Add this inside your ColourPlatformManager.cs temporarily:
    [ContextMenu("Test Toggle Colour")]
    public void TestToggle()
    {
        if (ActiveColor == PlatformColor.Red) SetActiveColor(PlatformColor.Green);
        else if (ActiveColor == PlatformColor.Green) SetActiveColor(PlatformColor.Blue);
        else SetActiveColor(PlatformColor.Red);
    }

}
