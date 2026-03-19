using UnityEngine;
using Mirror;

public class GameNetworkManager : NetworkManager
{
    [Header("UI References (Kéo thả vào đây)")]
    public GameObject lobbyPanel;
    public GameObject pauseMenuPanel;
    public GameObject readyPanel; // THÊM DÒNG NÀY ĐỂ QUẢN LÝ TẬP TRUNG

    [Header("Mission UI")]
    public GameObject missionPanel; // Bảng nhiệm vụ hiển thị khi vào trận

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect(); 
        Debug.Log("Mất kết nối với Host. Đang dọn dẹp và quay về Sảnh...");

        // Ensure any end-game UI is hidden when returning to lobby.
        FindFirstObjectByType<WinPanelManager>()?.Hide();
        FindFirstObjectByType<SurvivorWinUI>()?.Hide();

        if (lobbyPanel != null) lobbyPanel.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (readyPanel != null) readyPanel.SetActive(false); // Dọn luôn bảng Ready
        if (missionPanel != null) missionPanel.SetActive(false); // Ẩn bảng nhiệm vụ khi rời trận
        InGameMenuManager.isMenuOpen = false;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        // Ensure any end-game UI is hidden when returning to lobby.
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

        // Khi client join thành công (đăng nhập + vào trận),
        // bật bảng nhiệm vụ cho người chơi.
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

        // Nếu tất cả đã ready thì xuất chiến
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
            boss.isGameStarted = true; // Server ra lệnh
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
            Debug.Log("[Server] TOÀN DIỆT! Giải tán phòng sau 3 giây...");
            Invoke(nameof(EndGameAndReturnToLobby), 3f);
        }
    }

    [Server]
    private void EndGameAndReturnToLobby()
    {
        // Lệnh này đóng Server. Mirror sẽ tự hủy toàn bộ Player và dọn rác mạng.
        // Đồng thời kích hoạt hàm OnStopServer() mà ông đã cấu hình để bật LobbyPanel lên.
        StopHost(); 
    }
}