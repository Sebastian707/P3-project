using UnityEngine;

public class PlayerPrefLoader : MonoBehaviour
{
    void Start()
    {
        float saved = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
        GetComponent<StarterAssets.PlayerController>().mouseSensitivity = saved;
        Debug.Log("Mouse Sensitivity loaded" + saved);
    }
}