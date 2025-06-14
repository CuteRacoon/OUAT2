using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    public Animator animator;

    private bool isHoldingLamp = false;

    void Update()
    {
        HandleMovementInput();
        HandleLampInput();
    }

    void HandleMovementInput()
    {
        bool isWPressed = Input.GetKey(KeyCode.W);
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        animator.SetBool("Walking", isWPressed && !isShiftPressed);
        animator.SetBool("Running", isWPressed && isShiftPressed);
    }

    void HandleLampInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            isHoldingLamp = !isHoldingLamp;

            if (isHoldingLamp)
            {
                animator.SetTrigger("Lighting");     // ����������� ���������� �����
                animator.SetBool("Holding", true);   // ��������� � ��������� ����
            }
            else
            {
                animator.SetTrigger("Unlighting");   // ����������� �������� �����
                animator.SetBool("Holding", false);  // ������� �� ����
            }
        }
    }
}