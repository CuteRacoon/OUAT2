using UnityEngine;
public class PlayerAnimatorController : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;

    public Camera playerCamera;  // ������ ����� � ����������

    private bool isHoldingLamp = false;

    void Update()
    {
        HandleMovementInput();
        HandleLampInput();
    }

    void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D ��� �������
        float vertical = Input.GetAxis("Vertical");     // W/S ��� �������
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        bool isMoving = inputDirection.magnitude >= 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // ��������
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
    public void SetIdleState()
    {
        animator.SetBool("Walking", false);
        animator.SetBool("Running", false);
    }
}
