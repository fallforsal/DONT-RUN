using UnityEngine;
using Mirror;
using TMPro;

public class PlayerIdentity : NetworkBehaviour
{
    [Header("Sync Variables")]
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName = "";

    [SyncVar(hook = nameof(OnReadyStatusChanged))]
    public bool isReady = false;

    [Header("UI Reference")]
    public TextMeshProUGUI nameTagText; 

    public override void OnStartLocalPlayer()
    {
        string savedName = PlayerPrefs.GetString("CurrentPlayerName", "New Player");
        CmdSetPlayerName(savedName);
        
        isReady = false;
        LobbyUIBridge.localPlayerIdentity = this;

        // --- BẬT READY PANEL MỘT CÁCH AN TOÀN ---
        var manager = NetworkManager.singleton as GameNetworkManager;
        if (manager != null && manager.readyPanel != null)
        {
            manager.readyPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Ông chưa kéo ReadyPanel vào script GameNetworkManager ngoài Hierarchy!");
        }
    }

    public void ToggleReady()
    {
        if (isLocalPlayer) CmdSetReady(!isReady);
    }

    [Command]
    public void CmdSetReady(bool status)
    {
        isReady = status;
        var manager = NetworkManager.singleton as GameNetworkManager;
        if (manager != null) manager.CheckAllPlayersReady();
    }

    [Command]
    public void CmdSetPlayerName(string name)
    {
        playerName = name;
    }

    void OnNameChanged(string oldName, string newName) => UpdateUI();
    void OnReadyStatusChanged(bool oldStatus, bool newStatus) => UpdateUI();

    private void UpdateUI()
    {
        if (nameTagText != null)
        {
            nameTagText.text = playerName;
            nameTagText.color = isReady ? Color.green : Color.white;
        }
    }
}