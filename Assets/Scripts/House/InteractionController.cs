using System.Linq;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public GameObject stayingGirlsObj;
    public GameObject hintPanelsParent;

    private CameraBehaviour cameraBehaviour;
    private PlayerController playerController;
    private Transform[] stayingGirls;
    private GameObject[] hintPanels;
    private TriggerController[] triggerControllers;

    private Vector2[] defaultPanelPositions; // позиции по умолчанию
    private Vector2[] activePanelPositions;  // позиции при активной камере

    private int cameraAndTriggerIndex = 3;
    private int activeIndex = -1;

    private bool playerInside = false;
    private bool somethingChanging = false;
    private bool inProcess = false;

    void Start()
    {
        hintPanels = hintPanelsParent.transform.Cast<Transform>()
                                       .Select(t => t.gameObject)
                                       .ToArray();
        defaultPanelPositions = new Vector2[hintPanels.Length];
        activePanelPositions = new Vector2[hintPanels.Length];

        // Исключаем сам stayingGirlsObj из массива
        stayingGirls = new Transform[stayingGirlsObj.transform.childCount];
        for (int i = 0; i < stayingGirls.Length; i++)
        {
            stayingGirls[i] = stayingGirlsObj.transform.GetChild(i);
            RectTransform rt = hintPanels[i].GetComponent<RectTransform>();
            defaultPanelPositions[i] = rt.anchoredPosition;
            hintPanels[i].SetActive(false);
        }

        activePanelPositions[0] = new Vector2(850, 230);
        activePanelPositions[1] = new Vector2(-700, -230);
        activePanelPositions[2] = new Vector2(700, -70); // добавь, если нужно

        cameraBehaviour = FindAnyObjectByType<CameraBehaviour>();
        playerController = FindAnyObjectByType<PlayerController>();
        triggerControllers = FindObjectsByType<TriggerController>(FindObjectsSortMode.None)
            .OrderBy(tc => tc.index)
            .ToArray();
    }
    public void DisableTriggerByIndex(int index)
    {
        if (index >= 0 && index < triggerControllers.Length)
        {
            // Принудительно "выходим" из триггера
            if (triggerControllers[index].TryGetComponent<Collider>(out Collider col))
            {
                col.enabled = false;
            }
            triggerControllers[index].gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Индекс триггера вне диапазона: " + index);
        }
    }

    public void SetActiveTrigger(int index)
    {
        if (index <= 0)
        {
            if (inProcess)
                ResetInteraction();

            if (activeIndex >= 0 && hintPanels[activeIndex] != null)
                hintPanels[activeIndex].SetActive(false);

            activeIndex = -1;
            somethingChanging = false;
            return;
        }

        if (activeIndex >= 0 && activeIndex < hintPanels.Length)
        {
            hintPanels[activeIndex].SetActive(false);
        }

        activeIndex = index - 1;
        cameraAndTriggerIndex = index;
        somethingChanging = true;

        if (activeIndex >= 0 && activeIndex < hintPanels.Length)
        {
            hintPanels[activeIndex].SetActive(true);
            hintPanels[activeIndex].GetComponent<RectTransform>().anchoredPosition = defaultPanelPositions[activeIndex];
        }
    }
    public bool IsCurrentTrigger(int index)
    {
        return activeIndex == index - 1;
    }

    void Update()
    {
        if (activeIndex >= 0)
        {
            playerInside = playerController.GetPlayerInside();

            if (somethingChanging)
            {
                hintPanels[activeIndex].SetActive(playerInside);
                somethingChanging = false;
            }

            if (!playerInside)
            {
                /*if (inProcess)
                    ResetInteraction();*/
                hintPanels[activeIndex].SetActive(false);
                SetActiveTrigger(-1);
                return;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!inProcess)
                {
                    MoveHintPanel();
                    ActivateInteraction();
                }
                else
                {
                    ResetInteraction();
                }
            }
        }
    }

    private void ActivateInteraction()
    {
        cameraBehaviour.SwitchCamera(cameraAndTriggerIndex); // Камера сдвигается с учётом смещения индекса
        if (activeIndex < stayingGirls.Length)
        {
            playerController.SetTransform(stayingGirls[activeIndex]);
            playerController.LockPosition(true);
        }
        else
        {
            Debug.LogWarning("activeIndex выходит за границы stayingGirls");
        }

        playerController.SetMovement(false);
        inProcess = true;
    }
    public void SetPlayerPosition(int index)
    {
        playerController.SetMovement(false);
        playerController.SetTransform(stayingGirls[index]);
        playerController.LockPosition(true);
    }
    private void MoveHintPanel()
    {
        if (activeIndex >= 0 && activeIndex < hintPanels.Length)
        {
            hintPanels[activeIndex].GetComponent<RectTransform>().anchoredPosition = activePanelPositions[activeIndex];
        }
    }

    public void ResetInteraction()
    {
        playerController.SetMovement(true);
        playerController.LockPosition(false);

        cameraBehaviour.SwitchCamera(0);
        inProcess = false;

        if (activeIndex >= 0 && activeIndex < hintPanels.Length)
        {
            hintPanels[activeIndex].GetComponent<RectTransform>().anchoredPosition = defaultPanelPositions[activeIndex];
        }
    }
}
