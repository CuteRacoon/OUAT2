using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private bool canMove = true;
    private bool playerInsideOfTrigger = false;
    private bool isPositionLocked = false;

    public void LockPosition(bool state)
    {
        isPositionLocked = state;
    }
    public bool IsPositionLocked()
    {
        return isPositionLocked;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public bool GetPlayerInside()
    {
        return playerInsideOfTrigger;
    }
    public void SetPlayerInside(bool state)
    {
        playerInsideOfTrigger = state;
    }
    public void SetMovement(bool state)
    {
        canMove = state;
        //аnimator.applyRootMotion = state; // Инвертируем состояние
        //animator.enabled = state;

        PlayerAnimatorController animControl = GetComponent<PlayerAnimatorController>();
        if (!state) animControl.SetIdleState();
        gameObject.GetComponent<BasicBehaviour>().enabled = state;
        //gameObject.SetActive(state);
    }
    public void SetTransform(Transform trans)
    {
        transform.localPosition = trans.position; // учитываем локальную позицию
        transform.rotation = trans.rotation;
        BasicBehaviour basic = gameObject.GetComponent<BasicBehaviour>();
        basic.ClearLastDirection();
    }

}