using UnityEngine;
public class PlayerAnimatorController : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;

    public Camera playerCamera;  // ������ ����� � ����������

    private bool isHoldingLamp = false;
    public static PlayerAnimatorController Instance { get; private set; }

    private bool isChasingMode = false;

    void Update()
    {
        HandleMovementInput();
        //HandleLampInput();
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
        float horizontal = Input.GetAxis("Horizontal"); // A/D ��� �������
        float vertical = Input.GetAxis("Vertical");     // W/S ��� �������
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        bool isMoving = inputDirection.magnitude >= 0.1f;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isChasingMode)
        {
            isRunning = true;
        }

        // ������� ��� ����� � ��������
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
}
