using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    public Text missionText;

    [Header("Opening behavior")]
    public bool showOnStart = true;   // show the panel once when the scene starts

    bool hasShownOnStart = false;

    void Start()
    {
        if (showOnStart && !hasShownOnStart)
        {
            gameObject.SetActive(true);
            hasShownOnStart = true;
        }
    }

    void Update()
    {
        // Close the mission panel with Q
        if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Q))
        {
            gameObject.SetActive(false);
            return;
        }

        if (TaskManager.Instance != null && !TaskManager.Instance.tasksFinished)
        {
            missionText.text =
                "MISSION:\nComplete Tasks\n" +
                TaskManager.Instance.GetCompleted() +
                " / " +
                TaskManager.Instance.GetTotal();
        }
        else if (TaskManager.Instance != null)
        {
            missionText.text =
                "MISSION:\nEscape through the Main Doors!";
        }
    }
}