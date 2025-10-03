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
        bool isPaused = !pauseCanvas.activeSelf; //Если до этого канвас не был запущен, значит сейчас пауза

        CurrentPauseState = isPaused ? PauseState.PAUSE : PauseState.PLAY;

        if (ActionManager.Instance && additionalCanvas && ActionManager.Instance.isNight) //если по данным игры это ночь
        {
            activeCanvas = additionalCanvas;
        }
        else activeCanvas = pauseCanvas;
        activeCanvas.SetActive(isPaused);
        UIElements.SetActive(!isPaused);

        Time.timeScale = isPaused ? 0f : 1f;

        // управляем громкостью группы Prehistory
        if (masterMixer != null)
        {
            if (isPaused)
            {
                masterMixer.SetFloat(prehistoryVolumeParam, -60f); // заглушаем
                masterMixer.SetFloat(environmentVolumeParam, -10f);
            }
            else SoundsBack();
        }

    }
    private void SoundsBack()
    {
        masterMixer.SetFloat(prehistoryVolumeParam, 0f);
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
        // при переходе в меню обязательно возвращаем время
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
        CurrentPauseState = PauseState.PLAY;
        if (masterMixer != null)
            SoundsBack();
    }

    public void ExitGame()
    {
        // на всякий случай тоже возвращаем время
        Time.timeScale = 1f;
        Application.Quit();
    }
}
