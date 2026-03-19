using UnityEngine;
using Mirror;

public class Interactable : NetworkBehaviour
{
    public TaskUI taskUI;
    public SpriteOutline outline;
    public MonoBehaviour playerMovement;
    public Animator playerAnimator;
    public TaskCompleteUI taskCompleteUI;

    public MonoBehaviour progressObject;
    ITaskProgress progress;

    protected bool playerInRange = false;
    protected bool interacting = false;

    public string taskID;

    bool taskCompleted = false;

    public override void OnStartServer()
    {
        base.OnStartServer();

        // New run: clear completion state so tasks can be done again.
        taskCompleted = false;
        interacting = false;
        playerInRange = false;

        if (string.IsNullOrEmpty(taskID))
            taskID = "task_" + netId;

        if (TaskManager.Instance != null)
            TaskManager.Instance.RegisterTask(taskID);
    }

    void Start()
    {
        // Generate unique task ID if not set
        if (string.IsNullOrEmpty(taskID))
            taskID = "task_" + netId;

        // Only the server registers tasks
        if (TaskManager.Instance != null && TaskManager.Instance.isServer)
        {
            TaskManager.Instance.RegisterTask(taskID);
        }
    }

    void Update()
    {
        // Only the local player that is inside the trigger can start/stop interaction
        if (taskCompleted) return;

        if (playerInRange && !interacting && Input.GetKeyDown(KeyCode.E))
        {
            StartInteraction();
        }

        if (interacting && Input.GetKeyDown(KeyCode.Q))
        {
            StopInteraction();
        }
    }

    protected virtual void StartInteraction()
    {
        interacting = true;
        progress = progressObject as ITaskProgress;

        if (progress != null)
            progress.OnTaskCompleted += HandleTaskCompleted;

        progress?.ResetProgress();

        taskUI.SetProgressObject(progressObject);
        taskUI.OpenTask(progressObject);

        outline?.DisableOutline();

        // Lock player control while the task is active
        if (playerMovement != null)
        {
            // Hard stop any current movement by zeroing the Rigidbody2D velocity
            var movementComponent = playerMovement as Component;
            if (movementComponent != null)
            {
                var rb2d = movementComponent.GetComponent<Rigidbody2D>();
                if (rb2d != null)
                {
                    rb2d.linearVelocity = Vector2.zero;
                }
            }

            playerMovement.enabled = false;
        }

        if (playerAnimator != null)
        {
            // Ensure walking state is cleared when interaction begins
            playerAnimator.SetBool("IsWalking", false);
            playerAnimator.SetFloat("InputX", 0f);
            playerAnimator.SetFloat("InputY", 0f);
            playerAnimator.enabled = false;
        }
    }

    protected virtual void StopInteraction()
    {
        interacting = false;

        taskUI?.CloseTask();

        if (progress != null)
            progress.OnTaskCompleted -= HandleTaskCompleted;

        if (!taskCompleted)
        {
            progress?.ResetProgress();
        }

    
        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerAnimator != null)
            playerAnimator.enabled = true;

        if (playerInRange && !taskCompleted)
            outline?.EnableOutline();
    }

    void HandleTaskCompleted()
    {
       
        taskCompleted = true;

        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.RequestCompleteTask(taskID);
        }

        
        StopInteraction();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // Lazily cache movement/animation components from the player
            if (playerMovement == null)
            {
                var playerControl = other.GetComponent<PlayerControl>();
                if (playerControl != null)
                    playerMovement = playerControl;
            }

            if (playerAnimator == null)
            {
                var anim = other.GetComponent<Animator>();
                if (anim != null)
                    playerAnimator = anim;
            }

            if (!interacting && !taskCompleted)
                outline?.EnableOutline();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            outline?.DisableOutline();
        }
    }
}