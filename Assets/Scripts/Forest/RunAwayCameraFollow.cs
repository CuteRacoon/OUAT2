using UnityEngine;

public class RunAwayCameraFollow : MonoBehaviour
{
    [Header("Normal Follow")]
    public float followSpeed = 5f;

    [Header("Chase Follow")]
    public float chaseXSpeed = -2f;  // �������� � ������ ������
    public float acceleration = 3f;
    private float currentChaseXSpeed = 0f;

    [Header("Auto-Speed Increase")]
    public float chaseSpeedStep = -0.5f;           // ��������� ����������� ������ ���
    public float chaseSpeedIncreaseInterval = 5f;  // �������� ����� ������������ (���)
    public float minChaseSpeed = -12f;             // ������������ ��������� (����)

    private float speedIncreaseTimer = 0f;

    private Transform player;
    private float lastPlayerX;
    private float zOffset;
    private float xOffset;
    private float yOffset;

    private bool chaseMode = false; // ����� ������

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
                chaseXSpeed += chaseSpeedStep; // ��� �������������, ��� ��� �������� ����� �� ������
                chaseXSpeed = Mathf.Max(chaseXSpeed, minChaseSpeed); // ����������� �����
            }

            // ������� ����������� � ������ ��������
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
            // ������� �������� �� ������� �� ���� ����
            Vector3 targetPosition = new Vector3(player.position.x + xOffset, player.position.y + yOffset, player.position.z + zOffset);
            transform.position = Vector3.Lerp(currentPos, targetPosition, followSpeed * Time.fixedDeltaTime);
        }
    }

    private void InitializePlayerPosition()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError("����� � ����� 'Player' �� ������!");
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

    // ������� ���� ����� �� ����, ����� ��������/��������� ����� ������
    public void SetChaseMode(bool state)
    {
        chaseMode = state;
        if (!chaseMode)
        {
            currentChaseXSpeed = 0f; // ���������� �������� ��� ������ �� ������
            speedIncreaseTimer = 0f;
        }
    }
    public void SetChaseSpeed(float newSpeed)
    {
        chaseXSpeed = newSpeed;
    }
}
