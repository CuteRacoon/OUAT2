using UnityEngine;
using System.Collections;

public class LampController : MonoBehaviour
{
    [SerializeField] private BarScript barScript;
    [SerializeField] private Material lampMaterial;
    private Coroutine emissionCoroutine;
    private Color targetEmissionColor = new Color(25f / 255f, 0f, 2f / 255f);

    private bool playerInsideTrigger = false;
    private LampScript lampScript;

    private bool previousCriticalState = false;
    private bool learningCompleted = false;
    private Canvas canvas;
    private ParticleSystem currentParticleSystem = null;
    public bool IsLampOn => lampScript.IsLampOn;

    private LampTrigger trigger;
    public static LampController Instance { get; private set; }
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
        lampScript = FindAnyObjectByType<LampScript>();
        lampMaterial.EnableKeyword("_EMISSION");
    }
    public void LearningCompleted()
    {
        learningCompleted = true;
    }
    private void OnEnable()
    {
        LampTrigger.OnLampTriggerEnter += OnPlayerEnterLampZone;
        LampTrigger.OnLampTriggerExit += OnPlayerExitLampZone;

        GameEvents.CanDisplayLampBar += EnableLampBar;
        GameEvents.CannotDisplayLampBar += DisableLampBar;
        GameEvents.LampStateChanging += HandleLampStateChanging;
        GameEvents.BarIsNull += HandleBarNull;
    }
    private void HandleBarNull() // при достижении 0 заряда - выключить лампу
    {
        SetLampState(false);
        StateCanUseLamp(false);
    }

    private void OnDisable()
    {
        LampTrigger.OnLampTriggerEnter -= OnPlayerEnterLampZone;
        LampTrigger.OnLampTriggerExit -= OnPlayerExitLampZone;
    }
    private void OnPlayerEnterLampZone(LampTrigger currentTrigger)
    {
        trigger = currentTrigger;
        playerInsideTrigger = true;

        canvas = trigger.GetComponentInChildren<Canvas>(true);
        canvas.gameObject.SetActive(true);
        Transform parent = trigger.transform.parent;
        if (parent != null)
        {
            currentParticleSystem = parent.GetComponentInChildren<ParticleSystem>();
        }
    }
    private void FadeLampEmissionToRed(float duration = 2f)
    {
        if (lampMaterial == null)
        {
            Debug.LogWarning("LampController: lampMaterial не назначен.");
            return;
        }

        if (emissionCoroutine != null)
        {
            StopCoroutine(emissionCoroutine);
        }

        emissionCoroutine = StartCoroutine(FadeEmissionCoroutine(targetEmissionColor, duration));
    }
    private IEnumerator FadeEmissionCoroutine(Color targetColor, float duration)
    {
        Color startColor = lampMaterial.GetColor("_EmissionColor");
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            Color currentColor = Color.Lerp(startColor, targetColor, t);
            lampMaterial.SetColor("_EmissionColor", currentColor);
            yield return null;
        }

        lampMaterial.SetColor("_EmissionColor", targetColor);
    }
    private void OnPlayerExitLampZone(LampTrigger currentTrigger)
    {
        if (canvas.gameObject.activeSelf)
        {
            canvas.gameObject.SetActive(false);
        }
        trigger = null;
        playerInsideTrigger = false;
    }
    private void HandleLampStateChanging()
    {
        if (learningCompleted)
        {
            if(!lampScript.IsLampOn)
            {
                DisableLampBar();
            }
            else EnableLampBar();
        }
    }
    private void Update()
    {
        if (playerInsideTrigger && Input.GetKeyDown(KeyCode.E) && trigger.canInteract)
        {
            ResetBarAndFuel();
            canvas.gameObject.SetActive(false);
            trigger.canInteract = false;
        }

        // Проверка перехода в критическое состояние
        bool isNowCritical = barScript.IsCriticalLevel;

        if (isNowCritical && !previousCriticalState)
        {
            // Критический уровень впервые достигнут
            lampScript?.OnFuelCritical();
        }
        previousCriticalState = isNowCritical;
    }
    public void SetLampState(bool state)
    {
        lampScript.SetLampState(state);
    }
    public void ResetBarAndFuel()
    {
        if (currentParticleSystem != null)
            StartCoroutine(FadeOutParticles(currentParticleSystem, 1.5f)); // 1.5 секунды на затухание
        FadeLampEmissionToRed();
        barScript.ResetBar();
        StateCanUseLamp(true);
        lampScript.ResetIntensity();

        SetLampState(true); // включить лампу
    }
    private IEnumerator FadeOutParticles(ParticleSystem ps, float duration)
    {
        var emission = ps.emission;
        float startRate = emission.rateOverTime.constant;
        float timer = 0f;

        while (timer < duration)
        {
            float t = timer / duration;
            float newRate = Mathf.Lerp(startRate, 0f, t);

            var rateOverTime = emission.rateOverTime;
            rateOverTime.constant = newRate;
            emission.rateOverTime = rateOverTime;

            timer += Time.deltaTime;
            yield return null;
        }

        // Отключить систему полностью
        var finalRate = emission.rateOverTime;
        finalRate.constant = 0f;
        emission.rateOverTime = finalRate;
        ps.Stop();
    }

    public void DisableLampBar()
    {
        barScript.DisableBar();
    }
    public void EnableLampBar()
    {
        barScript.EnableBar();
    }
    public void StopLampBar()
    {
        barScript.StopBar();
    }
    public void ResumeLampBar()
    {
        barScript.ResumeBar();
    }
    public void StateCanUseLamp(bool state)
    {
        lampScript.CanUseLamp = state;
    }
}
