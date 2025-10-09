using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class ActionManager : MonoBehaviour
{
    private CameraManager cameraBehaviour;
    private InteractionManager interactionController;

    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject prehistoryCanvas;
    [SerializeField] private GameObject endPotion;
    [SerializeField] private GameObject cutScene;
    [SerializeField] private GameObject[] lights = new GameObject[2];
    [SerializeField] private GameObject goose;

    public GameObject horrorSounds;
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private GameObject sittingBoy;

    private string environmentVolumeParam = "EnvironmentVolume";
    private AudioSource gooseChawk;

    public bool isNight = false;
    public bool brotherStoled = false;
    public static ActionManager Instance { get; private set; }
    private bool stopAllGooseActions = false;
    private Coroutine volumeRoutine;
    private bool hasPlayedGirlThoughts2 = false;


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
        cameraBehaviour = FindAnyObjectByType<CameraManager>();
        interactionController = FindAnyObjectByType<InteractionManager>();

        cutScene.gameObject.SetActive(false);
        lights[0].SetActive(true);
        lights[1].SetActive(false);
        isNight = false;

        // При билде раскомментить
        if (prehistoryCanvas.activeSelf)
        {
            StartPreHistory();
        }
    }
    private void StartPreHistory()
    {
        InkQuestManager.Instance.SetQuestUIVisible(false);
        gameCanvas.SetActive(false);
        prehistoryCanvas.SetActive(true);
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
        interactionController.SetCanInteractOfTriggerByIndex(2, false);

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
        interactionController.SetCanInteractOfTriggerByIndex(2, false);

        gameCanvas.SetActive(true);
        prehistoryCanvas.SetActive(false);

        yield return new WaitForSeconds(3f);
        
        DialogueManager.Instance.PlayPartOfPlot("beginning");

        while (DialogueManager.Instance.IsDialoguePlaying)
        {
            yield return null;
        }

        interactionController.ResetInteraction();
        interactionController.SetCanInteractOfTriggerByIndex(2, true);
        InkQuestManager.Instance.SetQuestUIVisible(true);

        StartCoroutine(showFirstLearningPhrase());
    }
    private IEnumerator showFirstLearningPhrase()
    {
        yield return new WaitForSeconds(1f);
        DialogueManager.Instance.LearningPanelText("Для перемещения используйте клавиши W, A, S, D или стрелочки");

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
        DialogueManager.Instance.HideAllPanels();

        yield return StartCoroutine(showFirstRecipeHint());
    }
    private IEnumerator showFirstRecipeHint()
    {
        yield return new WaitForSeconds(1f);
        DialogueManager.Instance.PlayPartOfPlot("recipe_hint_1");
    }
    public void StartPotionCutScene()
    {
        StartCoroutine(PotionGetting());
    }
    private IEnumerator PotionGetting()
    {
        InteractionManager.Instance.SetInputLocked(true); //заблокируем выход из триггера
        DialogueManager.Instance.LearningPanelText("Нажмите Q, чтобы взять зелье в руки");
        InkQuestManager.Instance.SetAdditionalTaskByIndex(3);
        BoyController.Instance.SitDown();
        sittingBoy.SetActive(true);
        bool clicked = false;
        while (!clicked)
        {
            if (Input.GetKeyDown(KeyCode.Q)) clicked = true;
            yield return null;
        }
        PlayerAnimatorController.Instance.SetHandAnimate(true);
        DialogueManager.Instance.HideAllPanels();
        endPotion.SetActive(false);
        yield return new WaitForSeconds(1f);
        cameraBehaviour.SwitchCamera(0);
        PlayerController.Instance.SetActiveObjectInHands(true);
        InteractionManager.Instance.SetInputLocked(false); // разблокируем выход из триггера
        //TestCutScene();
        interactionController.ResetInteraction();
        interactionController.SetCanInteractOfTriggerByIndex(0, false);
        interactionController.SetCanInteractOfTriggerByIndex(1, false);
        interactionController.SetCanInteractOfTriggerByIndex(2, true);
    }
    public void StartCutScene()
    {
        if (masterMixer != null)
        {
            masterMixer.SetFloat(environmentVolumeParam, -60f);
        }
        StartCoroutine(TestCutSceneCoroutine());
        sittingBoy.SetActive(false);
        BoyController.Instance.LieDown();
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
        DialogueManager.Instance.PlayPartOfPlot("cut_scene");
        DialogueManager.Instance.BlockSkippingForOneKnot();
        // Ждём, пока видео не закончится
        yield return new WaitForSeconds(70f);
        cutScene.gameObject.SetActive(false);
        StartCoroutine(BakeCameraAnimation());
    }
    public void BakeCameraStart()
    {
        StartCoroutine(BakeCameraAnimation());
    }
    private IEnumerator BakeCameraAnimation()
    {
        InkQuestManager.Instance.SetQuestUIVisible(false);
        if (masterMixer != null)
        {
            masterMixer.SetFloat(environmentVolumeParam, -60f);
        }
        lights[1].SetActive(true);
        lights[0].SetActive(false);
        isNight = true;

        PauseManager.Instance.SetShouldChangeEnvironmentSounds(false);

        cameraBehaviour.SwitchCamera(3);
        Camera camera = cameraBehaviour.GetCurrentCamera();
        Animation bakeCameraAnime = camera.GetComponent<Animation>();
        bakeCameraAnime.Play("BakeCamera");
        yield return new WaitUntil(() => !bakeCameraAnime.isPlaying);
        yield return null;

        bakeCameraAnime.Stop();
        
        PlayerController.Instance.SetNewMovementSpeeds(3.8f, 3.8f, 4.5f);
        bakeCameraAnime.Stop("BakeCamera");
        bakeCameraAnime.Rewind("BakeCamera");
        ResetSceneAfterBakeAnimation();
        brotherStoled = true;
        InkQuestManager.Instance.NextMainTask(2);
        InkQuestManager.Instance.SetQuestUIVisible(true);
    }
    private void ResetSceneAfterBakeAnimation()
    {
        // РАСКОММИТИТЬ ДЛЯ ПОИСКА ЛАМПЫ
        InteractionManager.Instance.SetInputLocked(false);
        PlayerAnimatorController.Instance.SetHandAnimate(false);
        PlayerController.Instance.SetActiveObjectInHands(false);
        cameraBehaviour.SwitchCamera(0);
        interactionController.ResetInteraction();
        DialogueManager.Instance.HideAllPanels();
        interactionController.SetCanInteractOfTriggerByIndex(0, true);
        interactionController.SetCanInteractOfTriggerByIndex(1, true);
        DialogueManager.Instance.PlayPartOfPlotWithDelay("girl_thoughts", 2f);
    }
    public void HandleNightWindow(int number)
    {
        if (number == 1)
        {
            if (goose == null)
            {
                Debug.LogWarning("[HandleNightWindow] Объект 'goose' не найден в сцене!");
                return;
            }
            // Находим дочерний объект "Chawk" у гуся
            Transform chawkTransform = goose.transform.Find("Chawk");
            if (chawkTransform == null)
            {
                Debug.LogWarning("[HandleNightWindow] У 'goose' не найден дочерний объект 'Chawk'!");
                return;
            }

            // Получаем компонент AudioSource
            gooseChawk = chawkTransform.GetComponent<AudioSource>();
            if (gooseChawk == null)
            {
                Debug.LogWarning("[HandleNightWindow] У объекта 'Chawk' отсутствует AudioSource!");
                return;
            }

            // Запускаем воспроизведение звука
            gooseChawk.Play();
            //Debug.Log("[HandleNightWindow] Проигрывается звук чавканья гуся");

            DialogueManager.Instance.PlayPartOfPlotWithDelay("girl_thoughts_1", 2f);
            horrorSounds.SetActive(false);
        }
        if (number == 2)
        {
            stopAllGooseActions = true;
            InteractionManager.Instance.SetInputLocked(true);
            if (volumeRoutine != null)
            {
                StopCoroutine(volumeRoutine);
                volumeRoutine = null;
            }
            gooseChawk.Stop();
            DialogueManager.Instance.StopDialogue();
            Animation gooseAnime = goose.GetComponent<Animation>();
            gooseAnime.Play();
            StartCoroutine(WaitForGooseAnimationEnd(gooseAnime));
        }
    }
    public void ScreamGoose()
    {
        Animation gooseAnime = goose.GetComponent<Animation>();
        gooseAnime.Play();
    }
    private IEnumerator WaitForGooseAnimationEnd(Animation gooseAnime)
    {
        while (gooseAnime.isPlaying)
        {
            yield return null;
        }

        InteractionManager.Instance.SetInputLocked(false);
        LoadForestScene();
    }

    public void HighVolume()
    {
        volumeRoutine = StartCoroutine(SmoothIncreaseVolume());
    }
    private IEnumerator SmoothIncreaseVolume()
    {
        float duration = 15f;
        float startVolume = gooseChawk.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (stopAllGooseActions) yield break;

            elapsed += Time.deltaTime;
            gooseChawk.volume = Mathf.Lerp(startVolume, 1f, elapsed / duration);
            yield return null;
        }

        gooseChawk.volume = 1f;

        float t = 0f;
        while (t < 4f)
        {
            if (stopAllGooseActions) yield break;
            t += Time.deltaTime;
            yield return null;
        }

        gooseChawk.Stop();

        t = 0f;
        while (t < 3f)
        {
            if (stopAllGooseActions) yield break;
            t += Time.deltaTime;
            yield return null;
        }

        if (!stopAllGooseActions && !hasPlayedGirlThoughts2)
        {
            hasPlayedGirlThoughts2 = true;
            DialogueManager.Instance.PlayPartOfPlot("girl_thoughts_2");
        }


        volumeRoutine = null;
    }

    public void SetGooseScreaming()
    {
        Transform gooseAnimTransform = goose.transform.Find("goose_anim");
        if (gooseAnimTransform == null)
        {
            Debug.LogWarning("[SetGooseScreaming] У 'goose' не найден дочерний объект 'goose_anim'!");
            return;
        }

        // Получаем Animator
        Animator gooseAnimator = gooseAnimTransform.GetComponent<Animator>();
        if (gooseAnimator == null)
        {
            Debug.LogWarning("[SetGooseScreaming] У объекта 'goose_anim' отсутствует компонент Animator!");
            return;
        }

        // Устанавливаем булевый параметр "screaming" в true
        gooseAnimator.SetBool("screaming", true);
        Debug.Log("[SetGooseScreaming] Гусь теперь кричит");
    }

    public IEnumerator BrotherPotionCoroutine()
    {
        InteractionManager.Instance.SetInputLocked(true);
        DialogueManager.Instance.PlayPartOfPlot("brother_potion");
        while (DialogueManager.Instance.IsDialoguePlaying)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        StartCutScene();
        yield return null;
    }

    public void LoadForestScene()
    {
        StartCoroutine(LoadForestSceneCoroutine());
    }
    private IEnumerator LoadForestSceneCoroutine()
    {
        Camera camera = cameraBehaviour.GetCurrentCamera();
        Animation anime = camera.GetComponent<Animation>();
        anime.Stop();
        Volume volume = camera.GetComponent<Volume>();
        volume.enabled = true;
        if (volume != null)
        {
            float duration = 1f;
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
    }
}
