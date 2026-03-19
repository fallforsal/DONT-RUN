using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TaskManager : NetworkBehaviour
{
    public static TaskManager Instance;

    readonly HashSet<string> registeredTasks = new HashSet<string>();
    readonly HashSet<string> completedTasks = new HashSet<string>();

    [SyncVar]
    public bool tasksFinished = false;

    [SyncVar]
    int totalTasks = 0;

    [SyncVar]
    int completedCount = 0;

    void Awake()
    {
        Instance = this;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        ResetServerState();

        // Re-register all scene tasks each time a new host starts.
        var tasks = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        foreach (var t in tasks)
        {
            if (t == null) continue;
            if (string.IsNullOrEmpty(t.taskID))
                t.taskID = "task_" + t.netId;

            RegisterTask(t.taskID);
        }

        // Reset doors/walls that were disabled by tasks in the previous run.
        // Include inactive objects because the wall/door may have been set inactive.
        var blockers = FindObjectsByType<RemoveWallWhenTasksDone>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var b in blockers)
        {
            if (b == null) continue;
            b.gameObject.SetActive(true);
        }
    }

    [Server]
    void ResetServerState()
    {
        registeredTasks.Clear();
        completedTasks.Clear();
        tasksFinished = false;
        totalTasks = 0;
        completedCount = 0;
    }

    public void RegisterTask(string taskID)
    {
        // Only the server maintains the authoritative task list
        if (!isServer) return;

        if (!registeredTasks.Contains(taskID))
        {
            registeredTasks.Add(taskID);
            totalTasks = registeredTasks.Count;
        }
    }

    /// <summary>
    /// Called by clients or server. On clients, forwards to server via Command.
    /// </summary>
    public void RequestCompleteTask(string taskID)
    {
        if (isServer)
        {
            CompleteTask(taskID);
            return;
        }

        // If TaskManager isn't network-spawned yet on this client, do nothing.
        // (This avoids a Mirror warning; the task will be re-attempted only if caller retries.)
        if (!isClient) return;

        CmdCompleteTask(taskID);
    }

    [Command(requiresAuthority = false)]
    void CmdCompleteTask(string taskID)
    {
        CompleteTask(taskID);
    }

    [Server]
    public void CompleteTask(string taskID)
    {
        if (tasksFinished) return;

        if (!registeredTasks.Contains(taskID)) return;

        if (completedTasks.Contains(taskID)) return;

        completedTasks.Add(taskID);
        completedCount = completedTasks.Count;

        Debug.Log($"Tasks: {completedCount}/{totalTasks}");

        if (completedCount >= totalTasks && totalTasks > 0)
        {
            tasksFinished = true;
            Debug.Log("All tasks completed!");
        }
    }

    public bool IsTaskCompleted(string taskID)
    {
        return completedTasks.Contains(taskID);
    }

    public int GetCompleted() => completedCount;

    public int GetTotal() => totalTasks;
}