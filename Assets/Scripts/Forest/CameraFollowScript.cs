using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CameraFollowScript : MonoBehaviour
{
    public float speed = 5f;        // Скорость движения камеры
    private Transform player;       // Ссылка на Transform игрока
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
        if (player == null) return; // Выходим, если игрок не найден
        float playerX = player.position.x;

        // Двигаем камеру только если игрок двигается вперед
        if (playerX > lastPlayerX)
        {
            // Целевая позиция камеры (совпадает с X координатой игрока, остальное без изменений)
            Vector3 targetPosition = new Vector3(playerX+ xOffset, transform.position.y, player.position.z + zOffset);

            // Плавное перемещение камеры к целевой позиции
            //transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            transform.position = targetPosition;
            // Запоминаем текущую координату X игрока
            lastPlayerX = playerX;
        }
    }
    private void InitializePlayerPosition()
    {
        // Поиск игрока по тегу "Player"
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError("Игрок с тегом 'Player' не найден!");
            enabled = false; // Отключаем скрипт, чтобы избежать ошибок
            return;
        }

        player = playerObject.transform;
        lastPlayerX = player.position.x; // Инициализируем стартовой позицией игрока
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
