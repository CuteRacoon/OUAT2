using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering;
using Unity.VisualScripting;

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

    private Vector2[] defaultPanelPositions; // ������� �� ���������
    private Vector2[] activePanelPositions;  // ������� ��� �������� ������

    private int cameraAndTriggerIndex = -1;
    private int activeIndex = -1;
    private int windowDialogueIndex = 0;
    private int recipeDialogueIndex = 0;

    public bool playerInside = false;
    private bool somethingChanging = false;
    private bool inProcess = false;

    private bool learningCompleted = false;
    private bool showingLearningPanel = false;
    public bool recipeChecked = false;
    private Coroutine windowDialogueCoroutine;

    void Start()
    {
        hintPanels = hintPanelsParent.transform.Cast<Transform>()
                                       .Select(t => t.gameObject)
                                       .ToArray();
        defaultPanelPositions = new Vector2[hintPanels.Length];
        activePanelPositions = new Vector2[hintPanels.Length];

        // ��������� ��� stayingGirlsObj �� �������
        stayingGirls = new Transform[stayingGirlsObj.transform.childCount];
        for (int i = 0; i < hintPanels.Length; i++)
        {
            stayingGirls[i] = stayingGirlsObj.transform.GetChild(i);
            RectTransform rt = hintPanels[i].GetComponent<RectTransform>();
            defaultPanelPositions[i] = rt.anchoredPosition;
            hintPanels[i].SetActive(false);
        }

        activePanelPositions[0] = new Vector2(850, 230);
        activePanelPositions[1] = new Vector2(-700, -230);

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
            // ������������� "�������" �� ��������
            if (triggerControllers[index].TryGetComponent<Collider>(out Collider col))
            {
                col.enabled = false;
            }
            triggerControllers[index].gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("������ �������� ��� ���������: " + index);
        }
    }

    public void SetActiveTrigger(int index)
    {
        if (index <= 0)
        {
            if (showingLearningPanel && !learningCompleted)
            {
                showingLearningPanel = false;
                dialogueController.HideAllPanels(); // ������ ������ ��� ������
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
                dialogueController.LearningPanelText("������� � ��� ��������������");
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
                dialogueController.HideAllPanels();
                showingLearningPanel = false;
                SetActiveTrigger(-1);
                return;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!inProcess)
                {
                    hintPanels[activeIndex].SetActive(false);
                    if (showingLearningPanel)
                    {
                        dialogueController.HideAllPanels();
                        showingLearningPanel = false;
                    }
                    ActivateInteraction();
                }
                else
                {
                    ResetInteraction();
                    hintPanels[activeIndex].SetActive(true);
                    dialogueController.StopDialogue();
                }
            }
        }
    }

    private void ActivateInteraction()
    {
        if (!learningCompleted)
        {
            learningCompleted = true;
            StartCoroutine(StartExitCoroutine("������� �, ����� ��������� � ���"));
        }
        cameraBehaviour.SwitchCamera(cameraAndTriggerIndex); // ������ ���������� � ������ �������� �������
        if (activeIndex < hintPanels.Length)
        {
            playerController.SetTransform(stayingGirls[activeIndex]);
            playerController.LockPosition(true);
        }
        else
        {
            Debug.LogWarning("activeIndex ������� �� ������� stayingGirls");
        }

        playerController.SetMovement(false);
        inProcess = true;
        if (cameraAndTriggerIndex == 1 && !recipeChecked)
        {
            dialogueController.PlayPartOfPlot($"recipe_hint_{recipeDialogueIndex + 2}");
            dialogueController.BlockSkippingForOneKnot();

            if (recipeDialogueIndex > 1) recipeDialogueIndex = 0;
            else recipeDialogueIndex++;

        }
        if (cameraAndTriggerIndex == 2)
        {
            if (windowDialogueCoroutine != null)
            {
                StopCoroutine(windowDialogueCoroutine);
            }
            windowDialogueCoroutine = StartCoroutine(HandleWindowInteraction());
        }
    }
    private IEnumerator StartExitCoroutine(string text)
    {
        dialogueController.HideAllPanels();
        yield return new WaitForSeconds(0.5f);

        // ���������� ������ ��������
        dialogueController.LearningPanelText(text);
        bool clicked = false;

        while (!clicked)
        {
            if (Input.GetKeyDown(KeyCode.E)) clicked = true;
            yield return null;
        }
        if (showingLearningPanel)
        {
            dialogueController.HideAllPanels();
            showingLearningPanel = false;
        }
    }
    private IEnumerator HandleWindowInteraction()
    {
        if (windowDialogueIndex == 0)
        {
            recipeChecked = true;
            windowDialogueIndex = 1;
            yield break;
        }

        string dialogueKey = $"window_dialogue_{windowDialogueIndex}";
        Debug.Log("�������� ������" + windowDialogueIndex);
        if (dialogueKey == null)
        {
            windowDialogueCoroutine = null;
            yield break;
        }

        float delay = 4f;
        dialogueController.PlayPartOfPlotWithDelay(dialogueKey, delay);

        yield return new WaitForSeconds(delay);

        bool started = false;
        yield return new WaitUntil(() =>
        {
            if (dialogueController.IsDialoguePlaying)
            {
                started = true;
                return true;
            }
            // ���� ����� ����� �� ������ � �������
            return !inProcess;
        });

        if (!started)
        {
            // ������ �� ������� � �� ����������� ������
            windowDialogueCoroutine = null;
            yield break;
        }
        // ��������� �������, ��� ���������� �������
        dialogueController.BlockSkippingForOneKnot();
        // ���, ���� �� ���������� ��� �������������� �� ����� ��������
        yield return new WaitUntil(() => !dialogueController.IsDialoguePlaying || !inProcess);
        windowDialogueIndex++;
        windowDialogueCoroutine = null;
    }

    public void SetPlayerPosition(int index)
    {
        playerController.SetMovement(false);
        playerController.SetTransform(stayingGirls[index]);
        playerController.LockPosition(true);
    }

    public void ResetInteraction()
    {
        if (showingLearningPanel)
        {
            dialogueController.HideAllPanels();
            showingLearningPanel = false;
        }
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
