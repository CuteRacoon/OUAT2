using UnityEngine;
public class PlayerAnimatorController : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;

    public Camera playerCamera;  // Камеру задаём в инспекторе

    private bool isHoldingLamp = false;
    public static PlayerAnimatorController Instance { get; private set; }

    void Update()
    {
        HandleMovementInput();
        HandleLampInput();
    }
    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D или стрелки
        float vertical = Input.GetAxis("Vertical");     // W/S или стрелки
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        bool isMoving = inputDirection.magnitude >= 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Анимация
        animator.SetBool("Walking", isMoving && !isRunning);
        animator.SetBool("Running", isMoving && isRunning);
    }

    void HandleLampInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isHoldingLamp = !isHoldingLamp;

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
}
