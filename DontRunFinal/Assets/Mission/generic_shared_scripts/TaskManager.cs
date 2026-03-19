using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TaskManager : NetworkBehaviour
{
    public static TaskManager Instance;

    // 1. DÙNG SYNCHASHSET: Server thêm task vào đây, mọi Client tự động có bản sao y hệt!
    public readonly SyncHashSet<string> completedTasks = new SyncHashSet<string>();
    readonly HashSet<string> registeredTasks = new HashSet<string>();

    // 2. DÙNG HOOK: Khi Server báo tasksFinished = true, tất cả Client tự động giật sập tường!
    [SyncVar(hook = nameof(OnTasksFinishedChanged))]
    public bool tasksFinished = false;

    [SyncVar(hook = nameof(OnTotalTasksChanged))]
    public int totalTasks = 0;

    // 3. DÙNG HOOK: Khi số lượng thay đổi, tự động gọi UI cập nhật
    [SyncVar(hook = nameof(OnCompletedCountChanged))]
    public int completedCount = 0;
    void OnTotalTasksChanged(int oldTotal, int newTotal)
    {
        // Khi Client nhận được tổng số Task thực tế từ Server (VD: 2), lập tức bật UI
        TaskCompleteUI ui = FindFirstObjectByType<TaskCompleteUI>(FindObjectsInactive.Include);
        if (ui != null && newTotal > 0)
        {
            ui.ShowComplete(completedCount, newTotal);
        }
    }
    void Awake()
    {
        Instance = this;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        ResetServerState();

        // CẢNH BÁO LOGIC: Đảm bảo mọi object có script 'Interactable' ĐỀU LÀ TASK. 
        // Nếu ông có cái cửa nào cũng dùng script Interactable, nó sẽ cộng dồn khiến tổng số task bị sai!
        var tasks = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        foreach (var t in tasks)
        {
            if (t == null) continue;
            if (string.IsNullOrEmpty(t.taskID))
                t.taskID = "task_" + t.netId;

            RegisterTask(t.taskID);
        }

        var blockers = FindObjectsByType<RemoveWallWhenTasksDone>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var b in blockers)
        {
            if (b == null) continue;
            b.gameObject.SetActive(true);
        }
    }
    void Start()
    {
        TaskCompleteUI ui = FindFirstObjectByType<TaskCompleteUI>(FindObjectsInactive.Include);
        if (ui != null)
        {
            ui.ShowComplete(completedCount, totalTasks);
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
        if (!isServer) return;

        if (!registeredTasks.Contains(taskID))
        {
            registeredTasks.Add(taskID);
            totalTasks = registeredTasks.Count;
        }
    }

    public void RequestCompleteTask(string taskID)
    {
        if (isServer)
        {
            CompleteTask(taskID);
            return;
        }

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
        completedCount = completedTasks.Count; // Lệnh này sẽ kích hoạt OnCompletedCountChanged trên mọi máy

        Debug.Log($"[Server] Tasks: {completedCount}/{totalTasks}");

        if (completedCount >= totalTasks && totalTasks > 0)
        {
            tasksFinished = true; // Lệnh này sẽ kích hoạt OnTasksFinishedChanged trên mọi máy
            Debug.Log("[Server] All tasks completed!");
        }
    }

    public bool IsTaskCompleted(string taskID)
    {
        return completedTasks.Contains(taskID);
    }

    public int GetCompleted() => completedCount;
    public int GetTotal() => totalTasks;

   

    void OnCompletedCountChanged(int oldCount, int newCount)
    {
        
        TaskCompleteUI ui = FindFirstObjectByType<TaskCompleteUI>();
        if (ui != null)
        {
            ui.ShowComplete(newCount, totalTasks);
        }
    }

    void OnTasksFinishedChanged(bool oldVal, bool newVal)
    {
        if (newVal == true)
        {
            Debug.Log("[System] Xóa bỏ tường/cửa chắn WinZone trên toàn Server!");
            
            var blockers = FindObjectsByType<RemoveWallWhenTasksDone>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var b in blockers)
            {
                if (b != null) b.gameObject.SetActive(false);
            }
        }
    }
}