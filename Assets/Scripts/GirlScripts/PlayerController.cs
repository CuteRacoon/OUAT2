using System.Net.Sockets;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private BasicBehaviour basicBehaviour;
    private MoveBehaviour moveBehaviour;
    private bool canMove = true;
    private bool playerInsideOfTrigger = false;
    private bool isPositionLocked = false;
    private GameObject girlObject = null;

    [SerializeField] GameObject objectInHand;

    public static PlayerController Instance { get; private set; }
    private void OnEnable()
    {
        GameEvents.NeedToStopSprint += HandleSprintStop;
        GameEvents.NeedToStartSprint += HandleSprintStart;
    }
    public void LockPosition(bool state)
    {
        isPositionLocked = state;
        if (state)
        {
            // ������������� ���������� �������� � ��������
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.rotation = Quaternion.LookRotation(rb.transform.forward); // ������������� ������� �������
            rb.angularVelocity = Vector3.zero; // ���������� ��������
        }
    }
    public void SetActiveObjectInHands(bool state)
    {
        if (objectInHand != null)
        {
            objectInHand.SetActive(state);
        }
    }
    public bool IsPositionLocked()
    {
        return isPositionLocked;
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
        basicBehaviour = gameObject.GetComponent<BasicBehaviour>();
        moveBehaviour = gameObject.GetComponent<MoveBehaviour>();
        girlObject = transform.childCount > 0 ? transform.GetChild(0).gameObject : null;
    }
    private void HandleSprintStop()
    {
        SetSprintEnable(false);
    }
    private void HandleSprintStart()
    {
        SetSprintEnable(true);
    }
    public void SetSprintEnable(bool state)
    {
        basicBehaviour.SetSprintAllowed(state); // ���� state == true, �� ��������� ������
    }
    public void SetNewMovementSpeeds(float newWalkSpeed, float newRunSpeed, float newSprintSpeed)
    {
        moveBehaviour.walkSpeed = newWalkSpeed;
        moveBehaviour.runSpeed = newRunSpeed;
        moveBehaviour.sprintSpeed = newSprintSpeed;
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
        if (girlObject != null)
        {
            girlObject.SetActive(state);
        }
        canMove = state;
        //�nimator.applyRootMotion = state; // ����������� ���������
        //animator.enabled = state;

        PlayerAnimatorController animControl = GetComponent<PlayerAnimatorController>();
        if (!state) animControl.SetIdleState();
        gameObject.GetComponent<BasicBehaviour>().enabled = state;
        //gameObject.SetActive(state);
    }
    public void SetPlayerControl(bool state)
    {
        if (!state)
        {
            basicBehaviour.DisablePlayerControl();
        }
        else
        {
            basicBehaviour.EnablePlayerControl();
        }
    }
    public void SetTransform(Transform trans)
    {
        transform.localPosition = trans.position; // ��������� ��������� �������
        transform.rotation = trans.rotation;
        basicBehaviour.SetLastDirection(transform.forward);

        // ������������� ���������� ��������
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.rotation = trans.rotation;
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;

        basicBehaviour.ClearLastDirection();
    }

}