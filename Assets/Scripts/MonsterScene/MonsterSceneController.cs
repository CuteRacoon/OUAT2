using System.Collections;
using UnityEngine;

public class MonsterSceneController : MonoBehaviour
{
    private PlayerController playerController;
    private BasicBehaviour basicBehaviour; // Drag the player's BasicBehaviour here in the Inspector
    //private Animation girlMovementAnime;
    public GameObject girlComponent;
    private CameraBehaviour cameraBehaviour;
    private CameraFollowScript cameraFollowScript;

    public float teleportDistanceZ = 20f;
    public float runDuration = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = girlComponent.GetComponentInChildren<PlayerController>();
        basicBehaviour = girlComponent.GetComponentInChildren<BasicBehaviour>();
        //girlMovementAnime = girlComponent.GetComponentInChildren<Animation>();
        cameraBehaviour = FindAnyObjectByType<CameraBehaviour>();
        cameraFollowScript = FindAnyObjectByType<CameraFollowScript>();
    }
    public void StartRunning()
    {
        basicBehaviour.StartGirlInMonsterSceneAnimation(0.5f, 0.5f);
        basicBehaviour.LockTempBehaviour(basicBehaviour.GetHashCode());
        Camera monsterCamera = cameraBehaviour.GetCameraByIndex(1);
        Animation anime = monsterCamera.GetComponent<Animation>();
        anime.Play("MonsterCameraAnimation");
    }
    public void StopRunning()
    {
        basicBehaviour.EndGirlInMonsterSceneAnimation();
    }
    void TeleportPlayer()
    {
        // Получаем камеру
        Camera monsterCamera = cameraBehaviour.GetCameraByIndex(1);

        // Проверяем, что камера не null
        if (monsterCamera == null)
        {
            Debug.LogError("Monster camera не найдена!");
            return;
        }

        // Получаем текущую позицию игрока
        Vector3 playerPosition = girlComponent.transform.position;

        // Изменяем только координату Z
        playerPosition.z = monsterCamera.transform.position.z + teleportDistanceZ;

        // Устанавливаем новую позицию игрока
        girlComponent.transform.position = playerPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController != null)
        {
            bool playerInside = playerController.GetPlayerInside();
            if (playerInside)
            {
                Debug.Log("Запускаю анимацию с монстром");
                StartCoroutine(PlayGirlAnimation());
                playerController.SetPlayerInside(false);
                return;
            }
        }
    }
    IEnumerator PlayGirlAnimation()
    {
        // 1.Переключаем камеру
        cameraBehaviour.SwitchCamera(1);

        // 2. Телепортируем игрока
        TeleportPlayer();

        // 3. Запускаем бег по прямой
        StartRunning();

        // 4. Ждем указанное время
        yield return new WaitForSeconds(runDuration);

        // 5. Останавливаем бег и возвращаем управление игроку
        StopRunning();
        cameraBehaviour.SwitchCamera(0);
        cameraFollowScript.SetTargetPosition();
        yield return null;
    }
}
