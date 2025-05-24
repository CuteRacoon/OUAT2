using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ActionManager : MonoBehaviour
{
    private DialogueManager dialogueController;
    private CameraManager cameraBehaviour;
    private InteractionManager interactionController;

    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject prehistoryCanvas;
    [SerializeField] private GameObject endPotion;
    [SerializeField] private GameObject cutScene;
    public static ActionManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueController = FindAnyObjectByType<DialogueManager>();
        cameraBehaviour = FindAnyObjectByType<CameraManager>();
        interactionController = FindAnyObjectByType<InteractionManager>();

        cutScene.gameObject.SetActive(false);
        // ��� ����� �������������
        //gameCanvas.SetActive(false);
        //prehistoryCanvas.SetActive(true);
    }

    public void StartBeginningDialogue()
    {
        cameraBehaviour.SwitchCamera(3);
        interactionController.SetPlayerPosition(2);
        interactionController.SetActiveTrigger(-1);

        StartCoroutine(startDialogueNearBake());
    }
    private IEnumerator FadeOutBackgroundImage()
    {
        float duration = 1f;
        float elapsed = 0f;
        Image backgroundImage = prehistoryCanvas.GetComponentInChildren<Image>();
        Color originalColor = backgroundImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            backgroundImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }
    private IEnumerator startDialogueNearBake()
    {
        yield return StartCoroutine(FadeOutBackgroundImage());

        gameCanvas.SetActive(true);
        prehistoryCanvas.SetActive(false);
        
        yield return new WaitForSeconds(3f);
        
        dialogueController.PlayPartOfPlot("beginning");

        while (dialogueController.IsDialoguePlaying)
        {
            yield return null;
        }

        interactionController.ResetInteraction();
        interactionController.DisableTriggerByIndex(2);

        StartCoroutine(showFirstLearningPhrase());
    }
    private IEnumerator showFirstLearningPhrase()
    {
        yield return new WaitForSeconds(1f);
        dialogueController.LearningPanelText("��� ����������� ����������� ������� W, A, S, D ��� ���������");

        // ���, ���� ����� ����� ��������
        bool moved = false;
        while (!moved)
        {
            // ��������� ������� ������ W/A/S/D ��� �������
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                moved = true;
            yield return null;
        }
        // ��������� ��� 0.5 ������� ����� ��������
        yield return new WaitForSeconds(0.5f);
        dialogueController.HideAllPanels();

        yield return StartCoroutine(showFirstRecipeHint());
    }
    private IEnumerator showFirstRecipeHint()
    {
        yield return new WaitForSeconds(1f);
        dialogueController.PlayPartOfPlot("recipe_hint_1");
    }
    public void StartPotionCutScene()
    {
        StartCoroutine(PotionCutScene());
    }
    private IEnumerator PotionCutScene()
    {
        dialogueController.LearningPanelText("������� Q, ����� ����� ����� � ����");
        bool clicked = false;
        while (!clicked)
        {
            if (Input.GetKeyDown(KeyCode.Q)) clicked = true;
            yield return null;
        }
        dialogueController.HideAllPanels();
        endPotion.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        //Debug.Log("�������� ���-����� � ������");
        cutScene.gameObject.SetActive(true);
        // ���, ���� ����� �� ����������
        yield return new WaitForSeconds(22f);
        cameraBehaviour.SwitchCamera(0);
        interactionController.ResetInteraction();
        cutScene.gameObject.SetActive(false);
    }
}
