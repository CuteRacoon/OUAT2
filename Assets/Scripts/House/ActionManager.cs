using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ActionManager : MonoBehaviour
{
    private DialogueManager dialogueController;
    private CameraManager cameraBehaviour;
    private InteractionManager interactionController;

    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject prehistoryCanvas;
    [SerializeField] private GameObject endPotion;
    [SerializeField] private GameObject cutScene;
    [SerializeField] private GameObject[] lights = new GameObject[2];
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
        lights[0].SetActive(true);
        lights[1].SetActive(false);

        // При билде раскомментить
        //gameCanvas.SetActive(false);
        //prehistoryCanvas.SetActive(true);
    }
    private void OnEnable()
    {
        MiniGameLogicManager.CanStartPotionScene += StartPotionCutScene;
    }

    private void OnDisable()
    {
        MiniGameLogicManager.CanStartPotionScene -= StartPotionCutScene;
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
        interactionController.SetCanInteractOfTriggerByIndex(2, false);

        StartCoroutine(showFirstLearningPhrase());
    }
    private IEnumerator showFirstLearningPhrase()
    {
        yield return new WaitForSeconds(1f);
        dialogueController.LearningPanelText("Для перемещения используйте клавиши W, A, S, D или стрелочки");

        // Ждём, пока игрок начнёт движение
        bool moved = false;
        while (!moved)
        {
            // Проверяем нажатие клавиш W/A/S/D или стрелок
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                moved = true;
            yield return null;
        }
        // Подождать ещё 0.5 секунды перед скрытием
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
        StartCoroutine(PotionGetting());
    }
    private IEnumerator PotionGetting()
    {
        dialogueController.LearningPanelText("Нажмите Q, чтобы взять зелье в руки");
        bool clicked = false;
        while (!clicked)
        {
            if (Input.GetKeyDown(KeyCode.Q)) clicked = true;
            yield return null;
        }
        PlayerAnimatorController.Instance.SetHandAnimate(true);
        dialogueController.HideAllPanels();
        endPotion.SetActive(false);
        yield return new WaitForSeconds(1f);
        cameraBehaviour.SwitchCamera(0);
        PlayerController.Instance.SetActiveObjectInHands(true);
        //TestCutScene();
        interactionController.ResetInteraction();
        interactionController.SetCanInteractOfTriggerByIndex(2, true);
        interactionController.SetCanInteractOfTriggerByIndex(0, false);
        interactionController.SetCanInteractOfTriggerByIndex(1, false);
    }
    public void StartCutScene()
    {
        StartCoroutine(TestCutSceneCoroutine());
    }
    private IEnumerator TestCutSceneCoroutine()
    {
        cameraBehaviour.SwitchCamera(0);
        yield return null;
        Camera camera = cameraBehaviour.GetCurrentCamera();
        var volume = camera.GetComponent<Volume>();

        InteractionManager.Instance.SetInputLocked(true);
        if (volume != null)
        {
            volume.enabled = true;
        }
        cutScene.gameObject.SetActive(true);
        dialogueController.PlayPartOfPlot("cut_scene");
        // Ждём, пока видео не закончится
        yield return new WaitForSeconds(70f);
        cutScene.gameObject.SetActive(false);

        StartCoroutine(BakeCameraAnimation());
    }
    private IEnumerator BakeCameraAnimation()
    {
        lights[1].SetActive(true);
        lights[0].SetActive(false);
        cameraBehaviour.SwitchCamera(3);
        Camera camera = cameraBehaviour.GetCurrentCamera();
        Animation bakeCameraAnime = camera.GetComponent<Animation>();
        bakeCameraAnime.Play("BakeCamera");
        yield return new WaitUntil(() => !bakeCameraAnime.isPlaying);
        yield return null;
        bakeCameraAnime.Play("BakeCamera2");
        DialogueManager.Instance.PlayPartOfPlot("girl_thoughts");
        while (DialogueManager.Instance.IsDialoguePlaying)
        {
            yield return null;
        }
        bakeCameraAnime.Stop();

        // Плавное увеличение интенсивности Volume
        Volume volume = camera.GetComponent<Volume>();
        volume.enabled = true;
        if (volume != null)
        {
            float duration = 2f;
            float elapsed = 0f;
            volume.weight = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                volume.weight = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }

            volume.weight = 1f; // Убедимся, что точно 1
        }
        SceneManager.LoadScene("Forest");

        // РАСКОММИТИТЬ ДЛЯ ПОИСКА ЛАМПЫ
        /*InteractionManager.Instance.SetInputLocked(false);
        PlayerAnimatorController.Instance.SetHandAnimate(false);
        PlayerController.Instance.SetActiveObjectInHands(false);
        cameraBehaviour.SwitchCamera(0);
        interactionController.ResetInteraction();
        dialogueController.HideAllPanels();*/
    }
    public void PlayBakeCameraAnimation()
    {
        StartCoroutine(BakeCameraAnimation());
    }
}
