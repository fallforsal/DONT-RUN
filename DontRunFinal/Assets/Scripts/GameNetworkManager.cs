using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class GameNetworkManager : NetworkManager
{
    [Header("UI References (Kéo thả vào đây)")]
    public GameObject lobbyPanel;
    public GameObject pauseMenuPanel;
    public GameObject readyPanel; 

    [Header("Mission UI")]
    public GameObject missionPanel; 
    
    [Header("Panels")]
    public GameObject loginRegisterPanel;
    
    public override void Start()
    {
      
        base.Start();

       
        if (Application.isBatchMode)
        {
            Debug.Log("=====================================================");
            Debug.Log("[SYSTEM] PHÁT HIỆN CHẾ ĐỘ HEADLESS SERVER (CMD)!");
            Debug.Log("[SYSTEM] ĐANG TỰ ĐỘNG MỞ CỔNG 7777 VÀ LÀM THẨM PHÁN...");
            Debug.Log("=====================================================");
            
            StartServer(); 
        }
    }
    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged(); 

        Debug.Log("[Client] Map đã reset xong! Bỏ qua Join, chuẩn bị Ready...");

        
        if (loginRegisterPanel != null) loginRegisterPanel.SetActive(false);
        
        
        if (lobbyPanel != null) lobbyPanel.SetActive(false);
        
        
        if (readyPanel != null) readyPanel.SetActive(true); 

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (missionPanel != null) missionPanel.SetActive(false); 
        
       
        if (GameOverManager.instance != null) GameOverManager.instance.HideSpectatorUI();
        
        InGameMenuManager.isMenuOpen = false;
    }
    // =========================================================

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect(); 
        Debug.Log("Mất kết nối với Host. Đang dọn dẹp và quay về Sảnh...");

        FindFirstObjectByType<WinPanelManager>()?.Hide();
        FindFirstObjectByType<SurvivorWinUI>()?.Hide();

        if (lobbyPanel != null) lobbyPanel.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (readyPanel != null) readyPanel.SetActive(false); 
        if (missionPanel != null) missionPanel.SetActive(false); 
        InGameMenuManager.isMenuOpen = false;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        FindFirstObjectByType<WinPanelManager>()?.Hide();
        FindFirstObjectByType<SurvivorWinUI>()?.Hide();

        if (lobbyPanel != null) lobbyPanel.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (readyPanel != null) readyPanel.SetActive(false);
        if (missionPanel != null) missionPanel.SetActive(false);
        InGameMenuManager.isMenuOpen = false;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (missionPanel != null)
        {
            missionPanel.SetActive(true);
        }
    }

    public void CheckAllPlayersReady()
    {
        if (!NetworkServer.active) return;

        PlayerIdentity[] players = FindObjectsByType<PlayerIdentity>(FindObjectsSortMode.None);
        int readyCount = 0;

        foreach (var p in players)
        {
            if (p.isReady) readyCount++;
        }

        if (readyCount > 0 && readyCount == players.Length)
        {
            StartBossBattle();
        }
    }

    void StartBossBattle()
    {
        BossController boss = FindFirstObjectByType<BossController>();
        if (boss != null)
        {
            boss.isGameStarted = true; 
            Debug.Log("ALL READY! BOSS IS COMING!");
        }
    }

    public void CheckGlobalGameOver()
    {
        if (!NetworkServer.active) return; 

        PlayerHealth[] players = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
        bool allDead = true;

        foreach (var p in players)
        {
            if (p.currentHealth > 0) 
            {
                allDead = false; 
                break; 
            }
        }

        if (allDead && players.Length > 0)
        {
            Debug.Log("Reset Server");
            Invoke(nameof(EndGameAndReturnToLobby), 3f);
        }
    }

    [Server]
    private void EndGameAndReturnToLobby()
    {
       
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        ServerChangeScene(currentScene);
    }
}