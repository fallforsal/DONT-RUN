using UnityEngine;

public class TaskUI : MonoBehaviour
{
    ITaskProgress progress;

    public void SetProgressObject(MonoBehaviour obj)
    {
   
        if (progress != null)
            progress.OnTaskCompleted -= OnTaskFinished;

        progress = obj as ITaskProgress;

        if (progress != null)
            progress.OnTaskCompleted += OnTaskFinished;
    }

    public void OpenTask(MonoBehaviour obj)
    {
        SetProgressObject(obj);
        gameObject.SetActive(true);
    }

    void OnTaskFinished()
    {
        CloseTask();
    }

    public void CloseTask()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (progress != null)
            progress.OnTaskCompleted -= OnTaskFinished;
    }
}