﻿using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

// MoveBehaviour inherits from GenericBehaviour. This class corresponds to basic walk and run behaviour, it is the default behaviour.
public class MoveBehaviour : GenericBehaviour
{
    public float walkSpeed = 0.15f;                 // Default walk speed.
    public float runSpeed = 1.0f;                   // Default run speed.
    public float sprintSpeed = 2.0f;                // Default sprint speed.
    public float speedDampTime = 0.1f;              // Default damp time to change the animations based on current speed.

    private float speed;               // Moving speed.

    public float walkMultiplier = 0.5f;
    public float sprintMultiplier = 0.8f;
    private float speedMultiplier = 1.0f;

    private bool isAutoMoving = false;
    private Vector3 autoMoveDirection = Vector3.zero;
    private float autoMoveSpeed = 0f;


    private bool isChasingMode = false;

    // Start is always called after any Awake functions.
    void Start()
    {
        GameEvents.StartChasing += HandleChasingStart;
        GameEvents.StopChasing += HandleChasingStop;
        // Subscribe and register this behaviour as the default behaviour.
        behaviourManager.SubscribeBehaviour(this);
        behaviourManager.RegisterDefaultBehaviour(this.behaviourCode);
        StartCoroutine(SpeedsAnimeCoroutine());
    }
    void HandleChasingStart()
    {
        isChasingMode = true;
    }
    void HandleChasingStop()
    {
        isChasingMode = false;
    }
    public void StartAutoMove(Vector3 direction, float speed)
    {
        isAutoMoving = true;
        autoMoveDirection = direction.normalized;
        autoMoveSpeed = speed;
    }
    public void StopAutoMove()
    {
        isAutoMoving = false;
    }

    IEnumerator SpeedsAnimeCoroutine()
    {
        yield return null;
        behaviourManager.GetAnim.SetFloat("WalkAnimSpeed", walkSpeed * walkMultiplier);
        behaviourManager.GetAnim.SetFloat("SprintAnimSpeed", sprintSpeed * sprintMultiplier);
    }
    // LocalFixedUpdate overrides the virtual function of the base class.
    public override void LocalFixedUpdate()
    {
        // Call the basic movement manager.
        MovementManagement(behaviourManager.GetH, behaviourManager.GetV);
    }
    public void SetSpeedMultiplier(float value)
    {
        speedMultiplier = Mathf.Clamp01(value); // Безопасность: от 0 до 1
    }

    public void ResetSpeedMultiplier()
    {
        speedMultiplier = 1.0f;
    }

    void MovementManagement(float horizontal, float vertical)
    {
        // Если включён автоход, переопределяем ввод
        if (isAutoMoving)
        {
            horizontal = autoMoveDirection.x;
            vertical = autoMoveDirection.z; // Мы игнорируем Y
        }
        Vector3 direction = Rotating(horizontal, vertical);

        Vector2 inputDir = new Vector2(horizontal, vertical);
        float inputMagnitude = Vector2.ClampMagnitude(inputDir, 1f).magnitude;
        float targetSpeed;

        // В режиме погони игрок "бежит" даже без шифта
        if (isChasingMode)
        {
            targetSpeed = behaviourManager.IsSprinting() ? sprintSpeed : walkSpeed;
        }
        else
        {
            targetSpeed = behaviourManager.IsSprinting() ? sprintSpeed : (runSpeed * speedMultiplier);
        }

        // Плавная интерполяция скорости: Lerp от текущей к целевой
        float desiredSpeed = inputMagnitude * targetSpeed;
        // Если автодвижение, использовать его скорость
        if (isAutoMoving)
        {
            desiredSpeed = autoMoveSpeed;
        }

        speed = Mathf.Lerp(speed, desiredSpeed, 0.5f);

        // Применяем движение через velocity, чтобы сохранить гравитацию
        Vector3 currentVelocity = behaviourManager.GetRigidBody.linearVelocity;
        Vector3 velocity = direction * speed;
        velocity.y = currentVelocity.y; // Сохраняем текущую вертикальную скорость

        behaviourManager.GetRigidBody.linearVelocity = velocity;


        // Передача параметра в аниматор
        //behaviourManager.GetAnim.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
    }



    // Rotate the player to match correct orientation, according to camera and key pressed.
    Vector3 Rotating(float horizontal, float vertical)
    {
        // Get camera forward direction, without vertical component.
        Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);

        // Player is moving on ground, Y component of camera facing is not relevant.
        forward.y = 0.0f;
        forward = forward.normalized;

        // Calculate target direction based on camera forward and direction key.
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        Vector3 targetDirection = forward * vertical + right * horizontal;
        if (behaviourManager.IsInputLocked)
        {
            return Vector3.zero;
        }
        if (behaviourManager.GetPlayerController() != null &&
        behaviourManager.GetPlayerController().IsPositionLocked())
        {
            return Vector3.zero; // Не вращаем персонажа
        }

        // Lerp current direction to calculated target direction.
        if ((behaviourManager.IsMoving() && targetDirection != Vector3.zero && !behaviourManager.GetPlayerController().IsPositionLocked()))
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            Quaternion newRotation = Quaternion.Slerp(behaviourManager.GetRigidBody.rotation, targetRotation, behaviourManager.turnSmoothing);
            behaviourManager.GetRigidBody.MoveRotation(newRotation);
            behaviourManager.SetLastDirection(targetDirection);
        }
        // If idle, Ignore current camera facing and consider last moving direction.
        if (!(Mathf.Abs(horizontal) > 0.9 || Mathf.Abs(vertical) > 0.9))
        {
            behaviourManager.Repositioning();
        }

        return targetDirection;
    }
}
