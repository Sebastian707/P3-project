using UnityEngine;
using TMPro;

public class UIInfoManager : MonoBehaviour
{

    public static UIInfoManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI messageText;
    public float messageDuration = 2f;

    private float messageTimer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (messageText != null && messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0)
                messageText.text = "";
        }
    }

    public void ShowMessage(string text)
    {
        if (messageText != null)
        {
            messageText.text = text;
            messageTimer = messageDuration;
        }
        else
        {
            Debug.Log(text);
        }
    }
}
