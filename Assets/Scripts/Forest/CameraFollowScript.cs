using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    public float speed = 5f;        // �������� �������� ������
    private Transform player;       // ������ �� Transform ������
    private float lastPlayerX;
    private float zOffset;
    private float xOffset;
    private float yOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializePlayerPosition();
    }
    private void FixedUpdate()
    {
        if (player == null) return; // �������, ���� ����� �� ������
        float playerX = player.position.x;
        float playerY = player.position.y;

        /*// ������� ������ ������ ���� ����� ��������� ������
        if (playerX > lastPlayerX)
        {*/
        // ������� ������� ������ (��������� � X ����������� ������, ��������� ��� ���������)
        Vector3 targetPosition = new Vector3(playerX+ xOffset, playerY + yOffset, player.position.z + zOffset);

            // ������� ����������� ������ � ������� �������
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);
            //transform.position = targetPosition;
            // ���������� ������� ���������� X ������
            lastPlayerX = playerX;
        //}
    }
    private void InitializePlayerPosition()
    {
        // ����� ������ �� ���� "Player"
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError("����� � ����� 'Player' �� ������!");
            enabled = false; // ��������� ������, ����� �������� ������
            return;
        }

        player = playerObject.transform;
        lastPlayerX = player.position.x; // �������������� ��������� �������� ������
        zOffset = transform.position.z - player.position.z;
        xOffset = transform.position.x - player.position.x;
        yOffset = transform.position.y - player.position.y;
    }
    public void SetTargetPosition()
    {
        float playerX = player.position.x;
        Vector3 targetPosition = new Vector3(playerX + xOffset, transform.position.y, player.position.z + zOffset);
        transform.position = targetPosition;
    }
}
