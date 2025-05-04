using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CameraFollowScript : MonoBehaviour
{
    public float speed = 5f;        // �������� �������� ������
    private Transform player;       // ������ �� Transform ������
    private float lastPlayerX;
    private float zOffset;
    private float xOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializePlayerPosition();
    }
    private void FixedUpdate()
    {
        if (player == null) return; // �������, ���� ����� �� ������
        float playerX = player.position.x;

        // ������� ������ ������ ���� ����� ��������� ������
        if (playerX > lastPlayerX)
        {
            // ������� ������� ������ (��������� � X ����������� ������, ��������� ��� ���������)
            Vector3 targetPosition = new Vector3(playerX+ xOffset, transform.position.y, player.position.z + zOffset);

            // ������� ����������� ������ � ������� �������
            //transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            transform.position = targetPosition;
            // ���������� ������� ���������� X ������
            lastPlayerX = playerX;
        }
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
    }
    public void SetTargetPosition()
    {
        float playerX = player.position.x;
        Vector3 targetPosition = new Vector3(playerX + xOffset, transform.position.y, player.position.z + zOffset);
        transform.position = targetPosition;
    }
}
