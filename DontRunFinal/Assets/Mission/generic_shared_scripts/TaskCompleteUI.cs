using UnityEngine;
using TMPro;

public class TaskCompleteUI : MonoBehaviour
{
    public GameObject popup;
    public TextMeshProUGUI taskCountText; 
    public float showTime = 2f;

    
    public void ShowComplete(int currentTasks, int totalTasks)
    {
        if (taskCountText != null)
        {
            taskCountText.text = $"MISSION: COMPLETE TASKS\n{currentTasks} / {totalTasks}";
        }
        StartCoroutine(ShowRoutine());
    }

    System.Collections.IEnumerator ShowRoutine()
    {
        popup.SetActive(true);
        yield return new WaitForSeconds(showTime);
        popup.SetActive(false);
    }
}