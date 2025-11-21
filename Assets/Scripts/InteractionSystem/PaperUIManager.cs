using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PaperUIManager : MonoBehaviour
{
    public static PaperUIManager Instance;

    [Header("UI References")]
    public GameObject paperPanel;
    public TextMeshProUGUI paperContentText;
    public Image paperBackgroundImage;

    public static bool PaperIsOpen = false;

    private void Awake()
    {
        Instance = this;
        paperPanel.SetActive(false);

        Button panelButton = paperPanel.GetComponent<Button>();
        if (panelButton == null)
        {
            panelButton = paperPanel.AddComponent<Button>();
        }
        panelButton.onClick.AddListener(ClosePaper);
    }

    public void ShowPaper(PaperItem paper)
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        paperContentText.color = paper.color;
        paperContentText.text = paper.paperText;
        if (paper.font != null)
        {
            paperContentText.font = paper.font;
        }

        if (paper.backgroundImage != null)
        {
            paperBackgroundImage.sprite = paper.backgroundImage;
            paperBackgroundImage.enabled = true;
        }
        else
        {
            paperBackgroundImage.enabled = false;
        }

        paperPanel.SetActive(true);
        PaperIsOpen = true;
    }

    public void ClosePaper()
    {
        paperPanel.SetActive(false);
        PaperIsOpen = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
