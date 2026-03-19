using UnityEngine;
using Mirror; // Phải có namespace này

// Thay đổi MonoBehaviour thành NetworkBehaviour
public class PlayerControl : NetworkBehaviour 
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Audio")]
    public AudioSource footstepAudio;

    [Header("References")]
    public FieldOfView fieldOfView; 

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private Vector2 lastMoveDir = Vector2.right;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (footstepAudio == null)
            footstepAudio = GetComponent<AudioSource>();

        // Tắt FOV nếu không phải là người chơi cục bộ để tránh nhìn thấy FOV của người khác
        if (fieldOfView != null && !isLocalPlayer)
        {
            fieldOfView.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        
        if (InGameMenuManager.isMenuOpen)
        {
            moveInput = Vector2.zero; 
            animator.SetBool("IsWalking", false); 
            return; 
        }

        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(horizontal, vertical).normalized;

        bool isWalking = moveInput.magnitude > 0.1f;
        animator.SetBool("IsWalking", isWalking);

        if (footstepAudio != null)
        {
            if (isWalking)
            {
                if (!footstepAudio.isPlaying)
                    footstepAudio.Play();
            }
            else
            {
                if (footstepAudio.isPlaying)
                    footstepAudio.Stop();
            }
        }

        if (isWalking)
        {
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
            lastMoveDir = moveInput;
        }
        else
        {
            animator.SetFloat("InputX", 0f);
            animator.SetFloat("InputY", 0f);
        }

        if (fieldOfView != null)
        {
            fieldOfView.transform.position = transform.position;
            fieldOfView.SetAimDirection(lastMoveDir);
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        rb.linearVelocity = moveInput * moveSpeed;
    }
}