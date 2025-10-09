using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour
{
    public enum PauseState
    {
        PAUSE,
        PLAY
    }

    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject additionalCanvas;
    [SerializeField] GameObject UIElements;

    [SerializeField] private AudioMixer masterMixer;
    private string prehistoryVolumeParam = "PrehistoryVolume";
    private string environmentVolumeParam = "EnvironmentVolume";

    private GameObject activeCanvas;
    public static PauseManager Instance { get; private set; }
    public PauseState CurrentPauseState { get; private set; } = PauseState.PLAY;

    // Новый флаг — нужно ли менять environment-звуки
    private bool shouldChangeEnvironmentSounds = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && pauseCanvas)
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        // Проверяем активный канвас, а не только pauseCanvas
        bool isPaused = activeCanvas == null || !activeCanvas.activeSelf;

        CurrentPauseState = isPaused ? PauseState.PAUSE : PauseState.PLAY;

        // Определяем, какой канвас сейчас должен использоваться
        if (ActionManager.Instance && additionalCanvas && ActionManager.Instance.isNight)
        {
            activeCanvas = additionalCanvas;
        }
        else
        {
            activeCanvas = pauseCanvas;
        }

        // Включаем или выключаем канвас
        activeCanvas.SetActive(isPaused);
        UIElements.SetActive(!isPaused);

        Time.timeScale = isPaused ? 0f : 1f;

        if (masterMixer != null)
        {
            if (isPaused)
            {
                masterMixer.SetFloat(prehistoryVolumeParam, -60f);
                if (shouldChangeEnvironmentSounds)
                    masterMixer.SetFloat(environmentVolumeParam, -10f);
            }
            else
                SoundsBack();
        }
    }


    private void SoundsBack()
    {
        masterMixer.SetFloat(prehistoryVolumeParam, 0f);
        if (shouldChangeEnvironmentSounds)
            masterMixer.SetFloat(environmentVolumeParam, 0f);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        activeCanvas.SetActive(false);
        UIElements.SetActive(true);

        CurrentPauseState = PauseState.PLAY;

        if (masterMixer != null)
            SoundsBack();
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
        CurrentPauseState = PauseState.PLAY;

        if (masterMixer != null)
            SoundsBack();
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    // Новый метод: выставить флаг управления environment звуками
    public void SetShouldChangeEnvironmentSounds(bool shouldChange)
    {
        shouldChangeEnvironmentSounds = shouldChange;
    }
}
