using UnityEngine;

public class RunAwayCameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Камера позади и выше игрока
    public float followSpeed = 5f;

    [Header("Chase Follow")]
    public float chaseXSpeed = -2f;
    public float acceleration = 3f;
    private float currentChaseXSpeed = 0f;

    [Header("Auto-Speed Increase")]
    public float chaseSpeedStep = -0.5f;
    public float chaseSpeedIncreaseInterval = 5f;
    public float minChaseSpeed = -12f;

    private float speedIncreaseTimer = 0f;

    private Transform player;
    private float xOffset;
    private float yOffset;
    private float zOffset;
    private bool chaseMode = false;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError("Игрок с тегом 'Player' не найден!");
            enabled = false;
            return;
        }

        player = playerObject.transform;

        // Берём смещение с ForestCameraManager
        if (ForestCameraManager.Instance.playerCamera != null)
        {
            Transform refCam = ForestCameraManager.Instance.playerCamera.transform;
            Vector3 offset = refCam.position - player.position;
            xOffset = offset.x;
            yOffset = offset.y;
            zOffset = offset.z;
        }
        else
        {
            xOffset = transform.position.x - player.position.x;
            yOffset = transform.position.y - player.position.y;
            zOffset = transform.position.z - player.position.z;
        }
    }

    private void OnEnable()
    {
        RunAwayTrigger.OnRunAwayTriggerEnter += HandleFailure;
    }

    void HandleFailure(RunAwayTrigger trigger)
    {
        SetChaseMode(false);
        // При поражении — мгновенно центрируем камеру на игроке
        CenterCameraOnPlayer();
    }
    private void CenterCameraOnPlayer()
    {
        // Камера в точности в позиции игрока + смещение, без плавного движения
        Vector3 centerPosition = new Vector3(
            player.position.x + xOffset,
            player.position.y + yOffset,
            player.position.z + zOffset
        );

        transform.position = centerPosition;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 currentPos = transform.position;

        if (chaseMode)
        {
            speedIncreaseTimer += Time.fixedDeltaTime;
            if (speedIncreaseTimer >= chaseSpeedIncreaseInterval)
            {
                speedIncreaseTimer = 0f;
                chaseXSpeed += chaseSpeedStep;
                chaseXSpeed = Mathf.Max(chaseXSpeed, minChaseSpeed);
            }

            currentChaseXSpeed = Mathf.MoveTowards(currentChaseXSpeed, chaseXSpeed, acceleration * Time.fixedDeltaTime);
            float newX = currentPos.x + currentChaseXSpeed * Time.fixedDeltaTime;

            Vector3 targetPos = new Vector3(
                newX,
                Mathf.Lerp(currentPos.y, player.position.y + yOffset, 0.2f),
                Mathf.Lerp(currentPos.z, player.position.z + zOffset, 0.2f)
            );

            transform.position = targetPos;
        }
        else
        {
            currentChaseXSpeed = 0f;
            speedIncreaseTimer = 0f;

            Vector3 targetPos = player.position + new Vector3(xOffset, yOffset, zOffset);
            transform.position = Vector3.Lerp(currentPos, targetPos, followSpeed * Time.fixedDeltaTime);
        }
    }


    public void SetChaseMode(bool state)
    {
        chaseMode = state;
        if (!chaseMode)
        {
            currentChaseXSpeed = 0f;
            speedIncreaseTimer = 0f;
        }
    }

    public void SetChaseSpeed(float newSpeed)
    {
        chaseXSpeed = newSpeed;
    }
}
