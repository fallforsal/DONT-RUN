using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    [Header("Spectator UI (Chỉ cần Text, không dùng Panel to)")]
    public GameObject spectatorOverlay; 
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
       
        if (spectatorOverlay != null) spectatorOverlay.SetActive(false);
    }

    public void ShowSpectatorUI()
    {
     
        if (spectatorOverlay != null) spectatorOverlay.SetActive(true);
        SpectateNextPlayer(); 
    }

    public void HideSpectatorUI()
    {
        if (spectatorOverlay != null) spectatorOverlay.SetActive(false);
    }


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
        
      
        PlayerIdentity targetIdentity = target.GetComponent<PlayerIdentity>();
        if (spectatingNameText != null && targetIdentity != null)
        {
            spectatingNameText.text = "Đang xem: " + targetIdentity.playerName;
        }

     
        if (Mirror.NetworkClient.localPlayer != null)
        {
            LocalCameraSetup localCam = Mirror.NetworkClient.localPlayer.GetComponent<LocalCameraSetup>();
            if (localCam != null) localCam.SetCameraTarget(target);
        }
    }

    private void UpdateAlivePlayersList()
    {
        alivePlayers.Clear();
       
        PlayerHealth[] allPlayers = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
        foreach (var p in allPlayers)
        {
           
            if (p.currentHealth > 0) alivePlayers.Add(p);
        }
    }
}