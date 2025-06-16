using UnityEngine;

public class ForestCameraManager : MonoBehaviour
{
    public static ForestCameraManager Instance { get; private set; }

    [Header("Камера игрока (дефолтная)")]
    public Camera playerCamera;

    [Header("Диалоговые камеры")]
    public Camera[] dialogueCameras;

    [Header("Камеры укрытия")]
    public Camera[] hidingCameras;

    [Header("Камеры монстра")]
    public Camera[] monsterCameras;

    [Header("Камера убегания")]
    public Camera runAwayCamera;

    private Camera currentCamera;
    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        DeactivateAllCameras();

        if (playerCamera == null)
        {
            Debug.LogError("Камера игрока не назначена!");
            enabled = false;
            return;
        }

        playerCamera.gameObject.SetActive(true);
        currentCamera = playerCamera;
    }
    public void SwitchToPlayerCamera()
    {
        DeactivateAllCameras();
        playerCamera.gameObject.SetActive(true);
        currentCamera = playerCamera;
    }
    public void SwitchToRunAwayCamera()
    {
        /*if (currentCamera != null && runAwayCamera != null)
        {
            runAwayCamera.transform.position = currentCamera.transform.position;
            runAwayCamera.transform.rotation = currentCamera.transform.rotation;
        }
        */
        DeactivateAllCameras();
        runAwayCamera.gameObject.SetActive(true);
        currentCamera = runAwayCamera;
    }
    public void SwitchToHidingCamera(int index)
    {
        if (!IsValidIndex(hidingCameras, index)) return;

        DeactivateAllCameras();
        hidingCameras[index].gameObject.SetActive(true);
        currentCamera = hidingCameras[index];
    }
    public void SwitchToDialogueCamera(int index)
    {
        if (!IsValidIndex(dialogueCameras, index)) return;

        DeactivateAllCameras();
        dialogueCameras[index].gameObject.SetActive(true);
        currentCamera = dialogueCameras[index];
    }
    public void SwitchToMonsterCamera(int index)
    {
        if (!IsValidIndex(monsterCameras, index)) return;

        DeactivateAllCameras();
        monsterCameras[index].gameObject.SetActive(true);
        currentCamera = monsterCameras[index];
    }
    public Camera GetCurrentCamera()
    {
        return currentCamera;
    }
    private void DeactivateAllCameras()
    {
        if (playerCamera != null)
            playerCamera.gameObject.SetActive(false);

        DisableArray(dialogueCameras);
        DisableArray(hidingCameras);
        DisableArray(monsterCameras);
        if (runAwayCamera != null)
            runAwayCamera.gameObject.SetActive(false);
    }
    private void DisableArray(Camera[] cams)
    {
        if (cams == null) return;
        foreach (var cam in cams)
        {
            if (cam != null)
                cam.gameObject.SetActive(false);
        }
    }
    private bool IsValidIndex(Camera[] array, int index)
    {
        if (array == null || index < 0 || index >= array.Length)
        {
            Debug.LogWarning($"Индекс камеры вне диапазона или массив не назначен: {index}");
            return false;
        }
        return true;
    }
}