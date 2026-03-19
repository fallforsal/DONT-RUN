using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 1;
    
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int currentHealth;

    private PlayerControl playerControl;
    private bool isDead = false; // Cờ an toàn chặn spam sát thương

    void Start()
    {
        playerControl = GetComponent<PlayerControl>();
    }

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    [Server]
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        Debug.Log($"[PlayerHealth] Máu: {newHealth}/{maxHealth}");
    }

    [Server]
    private void Die()
    {
        Debug.Log($"[Server] {gameObject.name} tử trận!");
        RpcOnDied();

        var manager = NetworkManager.singleton as GameNetworkManager;
        if (manager != null) manager.CheckGlobalGameOver();
    }

    [ClientRpc]
    private void RpcOnDied()
    {
        if (playerControl != null) playerControl.enabled = false;

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("IsWalking", false);
            anim.SetFloat("InputX", 0);
            anim.SetFloat("InputY", 0);
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false; // Tắt va chạm để Boss đi xuyên qua xác

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1f, 1f, 1f, 0.4f);

        // NẾU LÀ MÁY CỦA CHÍNH MÌNH -> Bật Spectator xem người khác chơi
        if (isLocalPlayer && GameOverManager.instance != null)
        {
            GameOverManager.instance.ShowSpectatorUI(); 
        }
    }
}