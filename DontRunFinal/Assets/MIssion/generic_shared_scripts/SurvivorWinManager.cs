using UnityEngine;
using Mirror;

public class SurvivorWinManager : NetworkBehaviour
{
    public static SurvivorWinManager Instance;

    [Header("UI (optional)")]
    public SurvivorWinUI winUI;

    [SyncVar(hook = nameof(OnSurvivorWonChanged))]
    public bool survivorWon;

    bool appliedLocally;

    void Awake()
    {
        Instance = this;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        survivorWon = false;      // cho phép win lại ván mới
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        appliedLocally = false;   // cho phép ApplyWinLocal() chạy lại
                                  // Nếu muốn chắc ăn, có thể ẩn win panel ở đây:
        if (winUI == null)
            winUI = FindFirstObjectByType<SurvivorWinUI>();
        winUI?.Hide();
    }

    [Server]
    public void TriggerSurvivorWin()
    {
        if (survivorWon) return;
        survivorWon = true;
    }

    void OnSurvivorWonChanged(bool oldValue, bool newValue)
    {
        if (newValue)
            ApplyWinLocal();
    }

    void ApplyWinLocal()
    {
        if (appliedLocally) return;
        appliedLocally = true;

        // Prefer the dedicated win panel (buttons) if present.
        var winPanel = FindFirstObjectByType<WinPanelManager>();
        if (winPanel != null)
        {
            winPanel.Show();
        }
        else
        {
            // Fallback to legacy UI if the project still uses it.
            if (winUI == null)
                winUI = FindFirstObjectByType<SurvivorWinUI>();
            winUI?.Show("");
        }

        // Freeze player movement on this client.
        var players = FindObjectsByType<PlayerControl>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            if (p == null) continue;
            p.enabled = false;

            var rb = p.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            var anim = p.GetComponent<Animator>();
            if (anim != null) anim.SetBool("IsWalking", false);
        }
    }
}

