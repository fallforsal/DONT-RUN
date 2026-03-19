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

        Invoke(nameof(ResetMapAfterWin), 5f);
    }
    [Server]
    void ResetMapAfterWin()
    {

        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        NetworkManager.singleton.ServerChangeScene(currentScene);
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

        // ÉP TÌM KẾM CẢ NHỮNG UI ĐANG BỊ ẨN (FindObjectsInactive.Include)
        var winPanel = FindFirstObjectByType<WinPanelManager>(FindObjectsInactive.Include);
        if (winPanel != null)
        {
            winPanel.gameObject.SetActive(true); // Phải bật nó lên trước
            winPanel.Show();
        }
        else
        {
            // Fallback to legacy UI if the project still uses it.
            if (winUI == null)
                winUI = FindFirstObjectByType<SurvivorWinUI>(FindObjectsInactive.Include);
            
            if (winUI != null)
            {
                winUI.gameObject.SetActive(true);
                winUI.Show("");
            }
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

