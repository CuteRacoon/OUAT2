using System.Linq;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public GameObject stayingGirlsObj;
    public GameObject hintPanelsParent;

    private CameraBehaviour cameraBehaviour;
    private PlayerController playerController;
    private DialogueController dialogueController;
    private TriggerController[] triggerControllers;

    private Transform[] stayingGirls;
    private GameObject[] hintPanels;

    private Vector2[] defaultPanelPositions; // позиции по умолчанию
    private Vector2[] activePanelPositions;  // позиции при активной камере

    private int cameraAndTriggerIndex = -1;
    private int activeIndex = -1;
    public int windowDialogueIndex = 0;

    public bool playerInside = false;
    private bool somethingChanging = false;
    private bool inProcess = false;

    private bool learningCompleted = false;
    private bool showingLearningPanel = false;

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
        dialogueController = FindAnyObjectByType<DialogueController>();
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
    public void TurnStateHintPanels(bool state)
    {
        hintPanels[activeIndex].SetActive(state);
        activeIndex = -1;
    }

    public void SetActiveTrigger(int index)
    {
        if (index <= 0)
        {
            if (showingLearningPanel && !learningCompleted)
            {
                showingLearningPanel = false;
                dialogueController.HideAllPanels(); // скрыть панель при выходе
            }
            if (inProcess)
            {
                ResetInteraction();
            }    
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
            if (!learningCompleted)
            {
                dialogueController.LearningPanelText("Нажмите Е для взаимодействия");
                showingLearningPanel = true;
            }

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
                    if (cameraAndTriggerIndex == 2 && windowDialogueIndex > 0)
                    {
                        dialogueController.StopDialogue();
                    }
                }
            }
        }
    }

    private void ActivateInteraction()
    {
        if (showingLearningPanel && !learningCompleted)
        {
            showingLearningPanel = false;
            learningCompleted = true;
            dialogueController.HideAllPanels();
        }
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
        if (cameraAndTriggerIndex == 2)
        {
            switch (windowDialogueIndex)
            {
                case 0:
                    break;
                case 1:
                    dialogueController.PlayPartOfPlotWithDelay("window_dialogue_1", 3f);
                    break;
                case 2:
                    dialogueController.PlayPartOfPlotWithDelay("window_dialogue_2", 3f);
                    break;
            }
            windowDialogueIndex++;
        }
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
