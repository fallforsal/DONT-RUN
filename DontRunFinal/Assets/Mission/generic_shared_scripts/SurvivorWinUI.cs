using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SurvivorWinUI : MonoBehaviour
{
    [Header("UI refs")]
    public GameObject root;
    public TextMeshProUGUI messageTMP;
    public Text messageUGUI;

    void Awake()
    {
        if (root == null) root = gameObject;

        if (messageTMP == null) messageTMP = GetComponentInChildren<TextMeshProUGUI>(true);
        if (messageUGUI == null) messageUGUI = GetComponentInChildren<Text>(true);
    }

    public void Show(string message)
    {
        if (root != null) root.SetActive(true);
        if (!string.IsNullOrWhiteSpace(message))
        {
            if (messageTMP != null) messageTMP.text = message;
            if (messageUGUI != null) messageUGUI.text = message;
        }
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
    }
}

