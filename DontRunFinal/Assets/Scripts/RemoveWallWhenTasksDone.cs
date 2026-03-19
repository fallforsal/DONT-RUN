using UnityEngine;

public class RemoveWallWhenTasksDone : MonoBehaviour
{
    void Update()
    {
        if (TaskManager.Instance != null && TaskManager.Instance.tasksFinished)
        {
            gameObject.SetActive(false);
        }
    }
}