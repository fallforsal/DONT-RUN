using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    [Header("Spectator UI (Chỉ cần Text, không dùng Panel to)")]
    public GameObject spectatorOverlay; // Kéo 1 cái Canvas/Panel nhỏ xíu chứa Text vào đây, hoặc để trống
    public TextMeshProUGUI spectatingNameText; 

    private List<PlayerHealth> alivePlayers = new List<PlayerHealth>();
    private int currentSpectateIndex = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Khởi đầu game thì tắt UI theo dõi đi
        if (spectatorOverlay != null) spectatorOverlay.SetActive(false);
    }

    public void ShowSpectatorUI()
    {
        // TUYỆT ĐỐI KHÔNG BẬT ENDGAME PANEL NỮA
        // Chỉ bật cái UI nhỏ hiển thị tên người đang xem (nếu có)
        if (spectatorOverlay != null) spectatorOverlay.SetActive(true);
        SpectateNextPlayer(); // Chuyển góc nhìn ngay lập tức
    }

    public void HideSpectatorUI()
    {
        if (spectatorOverlay != null) spectatorOverlay.SetActive(false);
    }

    // Nút "Chuyển Camera" trên UI (nếu ông có làm) sẽ gọi hàm này
    public void SpectateNextPlayer()
    {
        UpdateAlivePlayersList();

        if (alivePlayers.Count == 0)
        {
            if (spectatingNameText != null) spectatingNameText.text = "TOÀN DIỆT! Đang về sảnh...";
            return; 
        }

        currentSpectateIndex++;
        if (currentSpectateIndex >= alivePlayers.Count) currentSpectateIndex = 0;

        Transform target = alivePlayers[currentSpectateIndex].transform;
        
        // Cập nhật tên hiển thị người đang bị soi
        PlayerIdentity targetIdentity = target.GetComponent<PlayerIdentity>();
        if (spectatingNameText != null && targetIdentity != null)
        {
            spectatingNameText.text = "Đang xem: " + targetIdentity.playerName;
        }

        // Ép Camera của máy mình bám theo mục tiêu mới
        if (Mirror.NetworkClient.localPlayer != null)
        {
            LocalCameraSetup localCam = Mirror.NetworkClient.localPlayer.GetComponent<LocalCameraSetup>();
            if (localCam != null) localCam.SetCameraTarget(target);
        }
    }

    private void UpdateAlivePlayersList()
    {
        alivePlayers.Clear();
        // Dùng FindObjectsByType thay vì FindObjectsOfType để không bị báo lỗi vàng Obsolete
        PlayerHealth[] allPlayers = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
        foreach (var p in allPlayers)
        {
            // Ai còn sống (máu > 0) thì đưa vào danh sách để Camera quay tới
            if (p.currentHealth > 0) alivePlayers.Add(p);
        }
    }
}