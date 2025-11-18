using UnityEngine;
using UnityEngine.Video; // required for VideoPlayer

public class PauseScript : MonoBehaviour
{
    public GameObject pauseScreen;
    public CharacterController playerController;
    public KeyCode pauseKey = KeyCode.Escape;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void Awake()
    {
        pauseScreen.SetActive(false);
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        if (Input.GetKeyDown(pauseKey))
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
       

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
    }
}
