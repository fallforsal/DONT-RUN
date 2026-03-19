using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using Mirror.Discovery;

public class WinPanelManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject winPanelRoot;

    [Header("Countdown (optional)")]
    public float autoReturnSeconds = 10f;
    public TextMeshProUGUI countdownTMP;
    public Text countdownUGUI;
    public string countdownFormat = "Returning to menu in {0}s";

    [Header("Optional: LAN advertise (Host only)")]
    public NetworkDiscovery networkDiscovery;

    [Header("Optional: freeze local player input when win shows")]
    public bool freezeLocalPlayer = true;

    bool shown;
    Coroutine countdownRoutine;

    public void Show()
    {
        if (shown) return;
        shown = true;

        if (winPanelRoot == null) winPanelRoot = gameObject;
        winPanelRoot.SetActive(true);

        if (countdownRoutine != null) StopCoroutine(countdownRoutine);
        if (autoReturnSeconds > 0f && (countdownTMP != null || countdownUGUI != null))
            countdownRoutine = StartCoroutine(CountdownAndReturn());

        if (!freezeLocalPlayer) return;

        // Freeze local player movement on this client (if present).
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

    public void Hide()
    {
        if (winPanelRoot == null) winPanelRoot = gameObject;
        winPanelRoot.SetActive(false);

        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
        }
    }

    System.Collections.IEnumerator CountdownAndReturn()
    {
        int seconds = Mathf.CeilToInt(autoReturnSeconds);
        while (seconds > 0)
        {
            string text = string.Format(countdownFormat, seconds);
            if (countdownTMP != null) countdownTMP.text = text;
            if (countdownUGUI != null) countdownUGUI.text = text;

            yield return new WaitForSeconds(1f);
            seconds--;
        }

        // Same behavior as pressing Exit.
        OnClickExit();
    }

    // Hook this to the Exit button.
    public void OnClickExit()
    {
        Hide();

        if (NetworkServer.active)
            NetworkManager.singleton.StopHost();
        else if (NetworkClient.isConnected)
            NetworkManager.singleton.StopClient();
    }

    // Hook this to the Play Again button.
    // Current game rule: leave match -> back to host/lobby, then start a fresh run.
    public void OnClickPlayAgain()
    {
        Hide();

        if (NetworkServer.active)
        {
            StartCoroutine(HostAgainRoutine());
        }
        else if (NetworkClient.isConnected)
        {
            // For clients, "play again" means leave back to menu.
            NetworkManager.singleton.StopClient();
        }
    }

    System.Collections.IEnumerator HostAgainRoutine()
    {
        var nm = NetworkManager.singleton;
        if (nm == null) yield break;

        nm.StopHost();

        // Wait until network is fully stopped before starting again.
        float timeout = 5f;
        while ((NetworkServer.active || NetworkClient.isConnected) && timeout > 0f)
        {
            timeout -= Time.unscaledDeltaTime;
            yield return null;
        }

        nm.StartHost();

        // Re-advertise for LAN discovery (optional).
        if (networkDiscovery == null) networkDiscovery = FindFirstObjectByType<NetworkDiscovery>();
        networkDiscovery?.AdvertiseServer();
    }
}

