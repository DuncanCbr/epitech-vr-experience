using UnityEngine;
using TMPro;

public class RobotSpeechBubble : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GameObject bubbleRoot;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Contenu")]
    [TextArea(3, 6)]
    [SerializeField] private string message = "Bienvenue à EPITECH !\nSuis-moi pour découvrir l'école.";

    void Start()
    {
        SetMessage(message);
    }

    public void SetMessage(string text)
    {
        if (messageText != null)
            messageText.text = text;
    }

    public void ShowBubble(bool visible)
    {
        if (bubbleRoot != null)
            bubbleRoot.SetActive(visible);
    }
}
