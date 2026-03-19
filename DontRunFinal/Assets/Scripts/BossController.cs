using UnityEngine;
using Mirror;

public class BossController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3.5f;
    public float detectionRadius = 15f;
    
    [Header("Damage Settings")]
    public int damageAmount = 1;

    // Hook này sẽ tự động chạy trên mọi máy khi biến thay đổi
    [SyncVar(hook = nameof(OnGameStarted))] 
    public bool isGameStarted = false;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform currentTarget;
    private Vector2 moveDirection;
    private Vector3 startPosition;

    void Awake()
    {
        startPosition = transform.position;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        // Đưa Boss về vị trí ban đầu và bắt nó đứng yên chờ Ready
        transform.position = startPosition;
        isGameStarted = false; 
    }
    
    void Update()
    {
        // Boss chỉ đuổi khi Server cho phép
        if (!isServer || !isGameStarted) return;

        FindNearestPlayer();

        if (currentTarget != null) moveDirection = (currentTarget.position - transform.position).normalized;
        else moveDirection = Vector2.zero;

        UpdateAnimation(moveDirection);
    }

    void FixedUpdate()
    {
        if (!isServer || !isGameStarted) 
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        if (currentTarget != null) rb.linearVelocity = moveDirection * moveSpeed;
        else rb.linearVelocity = Vector2.zero;
    }

    // --- LOGIC TẮT UI KHI BOSS XUẤT HIỆN ---
    void OnGameStarted(bool oldVal, bool newVal)
    {
        if (newVal == true) // Khi game bắt đầu
        {
            var manager = NetworkManager.singleton as GameNetworkManager;
            if (manager != null && manager.readyPanel != null)
            {
                manager.readyPanel.SetActive(false); // Tắt bảng chờ đi
            }
        }
    }

    private void FindNearestPlayer()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        float closestDistance = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                PlayerHealth pHealth = hitCollider.GetComponent<PlayerHealth>();
                if (pHealth != null && pHealth.currentHealth > 0)
                {
                    float distance = Vector2.Distance(transform.position, hitCollider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPlayer = hitCollider.transform;
                    }
                }
            }
        }
        currentTarget = closestPlayer;
    }

    private void UpdateAnimation(Vector2 dir)
    {
        bool isWalking = dir.magnitude > 0.1f;
        animator.SetBool("IsWalking", isWalking);
        if (isWalking)
        {
            animator.SetFloat("InputX", dir.x);
            animator.SetFloat("InputY", dir.y);
            animator.SetFloat("LastInputX", dir.x);
            animator.SetFloat("LastInputY", dir.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer) return; 
        DealDamageIfPlayer(collision.gameObject);
    }

    // Nếu Boss đứng đè lên người chơi, nó vẫn tiếp tục cắn
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isServer) return;
        DealDamageIfPlayer(collision.gameObject);
    }

    private void DealDamageIfPlayer(GameObject target)
    {
        if (target.CompareTag("Player"))
        {
            PlayerHealth health = target.GetComponent<PlayerHealth>();
            if (health != null) 
            {
                health.TakeDamage(damageAmount);
            }
        }
    }

    // THÊM 2 HÀM NÀY VÀO ĐỂ BẮT VA CHẠM CỨNG (COLLISION)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isServer) return; 
        DealDamageIfPlayer(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isServer) return;
        DealDamageIfPlayer(collision.gameObject);
    }
}