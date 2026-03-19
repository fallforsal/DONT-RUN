using UnityEngine;
using Mirror;

// Put this on a GameObject that has a Trigger collider (2D or 3D).
// Server-authoritative: only the host/server triggers the win.
public class SurvivorWinZone : MonoBehaviour
{
    [Tooltip("If not set, uses SurvivorWinManager.Instance.")]
    public SurvivorWinManager winManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!NetworkServer.active) return; // server-authoritative
        if (!other.CompareTag("Player")) return;
        if (TaskManager.Instance == null || !TaskManager.Instance.tasksFinished) return;

        var mgr = winManager != null ? winManager : SurvivorWinManager.Instance;
        if (mgr == null) mgr = FindFirstObjectByType<SurvivorWinManager>();
        if (mgr != null)
        {
            Debug.Log("[WinZone] Player entered (2D). Triggering survivor win.");
            mgr.TriggerSurvivorWin();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!NetworkServer.active) return; // server-authoritative
        if (!other.CompareTag("Player")) return;
        if (TaskManager.Instance == null || !TaskManager.Instance.tasksFinished) return;

        var mgr = winManager != null ? winManager : SurvivorWinManager.Instance;
        if (mgr == null) mgr = FindFirstObjectByType<SurvivorWinManager>();
        if (mgr != null)
        {
            Debug.Log("[WinZone] Player entered (3D). Triggering survivor win.");
            mgr.TriggerSurvivorWin();
        }
    }
}

