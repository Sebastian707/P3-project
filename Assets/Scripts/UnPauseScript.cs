using StarterAssets;
using UnityEngine;
using UnityEngine.Video;

public class UnpauseButtonScript : MonoBehaviour
{
    public CharacterController playerController;
    public KeyCode unpauseKey = KeyCode.Escape;
    GameObject pauseScreen;

    private void Awake()
    {
        pauseScreen = GameObject.Find("PauseScreen");
    }

    private void Update()
    {
        if (pauseScreen.activeSelf && Input.GetKeyDown(unpauseKey))
        {
            Unpause();
        }
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
