using System.Collections;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class MonsterSceneController : MonoBehaviour
{
    private PlayerController playerController;
    private BasicBehaviour basicBehaviour; // Drag the player's BasicBehaviour here in the Inspector
    //private Animation girlMovementAnime;
    public GameObject girlComponent;
    public Transform stayingGirlParentObj;
    private Transform[] stayingGirls;

    private CameraFollowScript cameraFollowScript;

    public float teleportDistanceZ = 20f;
    public float runDuration = 3f;

    private MonsterTrigger currentTrigger;
    private int currentTriggerIndex = -1;
    private bool lampState;
    private GameObject canvasObj;

    public static MonsterSceneController Instance { get; private set; }
    private void OnAwake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = girlComponent.GetComponentInChildren<PlayerController>();
        basicBehaviour = girlComponent.GetComponentInChildren<BasicBehaviour>();
        cameraFollowScript = FindAnyObjectByType<CameraFollowScript>();

        int childCount = stayingGirlParentObj.childCount;
        stayingGirls = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            Transform child = stayingGirlParentObj.GetChild(i);
            stayingGirls[i] = child;
            child.gameObject.SetActive(false);
        }
        MonsterTrigger.OnMonsterTriggerEnter += HandlePlayerEnterTrigger;
    }
    void OnDestroy()
    {
        MonsterTrigger.OnMonsterTriggerEnter -= HandlePlayerEnterTrigger;
    }
    void HandlePlayerEnterTrigger(MonsterTrigger trigger)
    {
        currentTrigger = trigger;
        currentTriggerIndex = trigger.index;
        lampState = LampController.Instance.IsLampOn;

        canvasObj = currentTrigger.transform.GetChild(0).gameObject;
    }
    public void StartRunning()
    {
        if (lampState) GameEvents.RaiseCannotDisplayLampBar();
        basicBehaviour.StartGirlInMonsterSceneAnimation(0.5f, 0.5f);
        basicBehaviour.LockTempBehaviour(basicBehaviour.GetHashCode());
        
    }
    public void StopRunning()
    {
        basicBehaviour.EndGirlInMonsterSceneAnimation();
    }
    void TeleportPlayer()
    {
        if (currentTriggerIndex < 0 || currentTriggerIndex >= stayingGirls.Length)
        {
            Debug.LogError("Некорректный индекс stayingGirl для телепортации.");
            return;
        }

        Transform targetGirl = stayingGirls[currentTriggerIndex];
        if (targetGirl == null)
        {
            Debug.LogError("Не удалось найти stayingGirl по индексу.");
            return;
        }

        girlComponent.transform.position = targetGirl.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController != null && playerController.GetPlayerInside())
        {
            Debug.Log("Запускаю анимацию с монстром");
            StartCoroutine(PlayGirlAnimation());
            playerController.SetPlayerInside(false);
        }
    }
    IEnumerator PlayGirlAnimation()
    {
        // 1.Переключаем камеру
        ForestCameraManager.Instance.SwitchToMonsterCamera(currentTriggerIndex);
        canvasObj.SetActive(true);

        // 2. Телепортируем игрока
        TeleportPlayer();

        // 3. Запускаем бег по прямой
        StartRunning();

        // 4. Получаем длительность анимации камеры
        float waitTime = runDuration;
        Animation cameraAnimation = ForestCameraManager.Instance.GetCurrentCamera().GetComponent<Animation>();
        if (cameraAnimation != null && cameraAnimation.clip != null)
        {
            waitTime = cameraAnimation.clip.length;
        }
        // 4. Ждем указанное время
        yield return new WaitForSeconds(waitTime);

        // 5. Останавливаем бег и возвращаем управление игроку
        StopRunning();
        ForestCameraManager.Instance.SwitchToPlayerCamera();

        cameraFollowScript.SetTargetPosition();
        if (currentTrigger != null)
        {
            currentTrigger.canInteract = false;
            currentTrigger = null; // очищаем ссылку
        }
        currentTriggerIndex = -1;
        yield return null;
        if (lampState)
        {
            GameEvents.RaiseCanDisplayLampBar();
        }
        canvasObj.SetActive(false);
        canvasObj = null;
    }
}
