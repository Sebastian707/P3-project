using UnityEngine;
using TMPro;

public class PaperItem : Interactable
{
    [TextArea(5, 10)]
    public string paperText;

    public Sprite backgroundImage; 
    public TMP_FontAsset font;
    public Color color;

    protected override void Interact()
    {
    

        PaperUIManager.Instance.ShowPaper(this);
    }
}
