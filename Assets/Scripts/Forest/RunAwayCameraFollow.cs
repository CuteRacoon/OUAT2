using UnityEngine;

public class RunAwayCameraFollow : MonoBehaviour
{
    [Header("Normal Follow")]
    public float followSpeed = 5f;

    [Header("Chase Follow")]
    public float chaseXSpeed = -2f;  // Скорость в режиме погони
    public float acceleration = 3f;
    private float currentChaseXSpeed = 0f;

    [Header("Auto-Speed Increase")]
    public float chaseSpeedStep = -0.5f;           // Насколько увеличивать каждый раз
    public float chaseSpeedIncreaseInterval = 5f;  // Интервал между увеличениями (сек)
    public float minChaseSpeed = -12f;             // Максимальное ускорение (вниз)

    private float speedIncreaseTimer = 0f;

    private Transform player;
    private float lastPlayerX;
    private float zOffset;
    private float xOffset;
    private float yOffset;

    private bool chaseMode = false; // Режим погони

    void Start()
    {
        InitializePlayerPosition();
    }
    private void OnEnable()
    {
        RunAwayTrigger.OnRunAwayTriggerEnter += HandleFailure;
    }
    void HandleFailure(RunAwayTrigger trigger)
    {
        SetChaseMode(false);
    }
    private void FixedUpdate()
    {
        if (player == null) return;

        Vector3 currentPos = transform.position;

        if (chaseMode)
        {
            speedIncreaseTimer += Time.fixedDeltaTime;
            if (speedIncreaseTimer >= chaseSpeedIncreaseInterval)
            {
                speedIncreaseTimer = 0f;
                chaseXSpeed += chaseSpeedStep; // шаг отрицательный, так что скорость растёт по модулю
                chaseXSpeed = Mathf.Max(chaseXSpeed, minChaseSpeed); // ограничение снизу
            }

            // Плавное приближение к нужной скорости
            currentChaseXSpeed = Mathf.MoveTowards(currentChaseXSpeed, chaseXSpeed, acceleration * Time.fixedDeltaTime);

            float newX = currentPos.x + currentChaseXSpeed * Time.fixedDeltaTime;
            float targetY = player.position.y + yOffset;
            float targetZ = player.position.z + zOffset;

            transform.position = new Vector3(newX,
                                             Mathf.Lerp(currentPos.y, targetY, 0.2f),
                                             Mathf.Lerp(currentPos.z, targetZ, 0.2f));
        }
        else
        {
            currentChaseXSpeed = 0f;
            speedIncreaseTimer = 0f;
            // Обычное слежение за игроком по всем осям
            Vector3 targetPosition = new Vector3(player.position.x + xOffset, player.position.y + yOffset, player.position.z + zOffset);
            transform.position = Vector3.Lerp(currentPos, targetPosition, followSpeed * Time.fixedDeltaTime);
        }
    }

    private void InitializePlayerPosition()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError("Игрок с тегом 'Player' не найден!");
            enabled = false;
            return;
        }

        player = playerObject.transform;
        lastPlayerX = player.position.x;
        zOffset = transform.position.z - player.position.z;
        xOffset = transform.position.x - player.position.x;
        yOffset = transform.position.y - player.position.y;
    }

    public void SetTargetPosition()
    {
        Vector3 targetPosition = new Vector3(player.position.x + xOffset, transform.position.y, player.position.z + zOffset);
        transform.position = targetPosition;
    }

    // Вызывай этот метод из кода, чтобы включать/выключать режим погони
    public void SetChaseMode(bool state)
    {
        chaseMode = state;
        if (!chaseMode)
        {
            currentChaseXSpeed = 0f; // сбрасываем скорость при выходе из режима
            speedIncreaseTimer = 0f;
        }
    }
    public void SetChaseSpeed(float newSpeed)
    {
        chaseXSpeed = newSpeed;
    }
}
