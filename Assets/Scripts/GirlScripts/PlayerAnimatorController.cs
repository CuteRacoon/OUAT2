using UnityEngine;
public class PlayerAnimatorController : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;

    public Camera playerCamera;  // Камеру задаём в инспекторе

    private bool isHoldingLamp = false;
    public static PlayerAnimatorController Instance { get; private set; }

    private bool isChasingMode = false;
    private BasicBehaviour basicManager;

    private bool isAutoMoving = false;

    void Update()
    {
        if (!basicManager.IsInputLocked)
            HandleMovementInput();
        //HandleLampInput();
    }
    private void Awake()
    {
        basicManager = gameObject.GetComponent<BasicBehaviour>();
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        GameEvents.StartChasing += HandleChasingStart;
        GameEvents.StopChasing += HandleChasingStop;
    }
    void HandleChasingStart()
    {
        isChasingMode = true;
    }
    void HandleChasingStop()
    {
        isChasingMode = false;
    }
    void HandleMovementInput()
    {
        if (isAutoMoving)
        {
            animator.SetBool("Walking", true);
            animator.SetBool("Running", false);
            return;
        }
        float horizontal = Input.GetAxis("Horizontal"); // A/D или стрелки
        float vertical = Input.GetAxis("Vertical");     // W/S или стрелки
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        bool isMoving = inputDirection.magnitude >= 0.1f;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isChasingMode)
        {
            isRunning = true;
        }

        // Передаём все флаги в аниматор
        animator.SetBool("Walking", isMoving && !isRunning);
        animator.SetBool("Running", isMoving && isRunning);
        animator.SetBool("Chasing", isChasingMode);
    }

    public void PlayLampAnimation(bool state)
    {
         isHoldingLamp = state;

         if (isHoldingLamp)
         {
            animator.SetTrigger("Lighting");
            animator.SetBool("Holding", true);
         }
         else
         {
            animator.SetTrigger("Unlighting");
            animator.SetBool("Holding", false);
         }
    }
    public void SetHandAnimate(bool state)
    {
        if (state)
        {
            animator.SetBool("Holding", true);
        }
        else
        {
            animator.SetBool("Holding", false);
        }
    }
    public void SetIdleState()
    {
        animator.SetBool("Walking", false);
        animator.SetBool("Running", false);
    }
    public void PlayStepAnimation(bool isRunning = false)
    {
        animator.SetBool("Walking", !isRunning);
        animator.SetBool("Running", isRunning);
        isAutoMoving = true;
    }
    public void StopStepAnimation()
    {
        animator.SetBool("Walking", false);
        animator.SetBool("Running", false);
        isAutoMoving = false;
    }
}
