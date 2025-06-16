using UnityEngine;
using System.Collections;

public class ForestDialogueController : MonoBehaviour
{
    private DialogueManager dialogueManager;
    private DialogueTrigger trigger;

    private Canvas canvas;
    private GameObject[] lightObjects;

    private bool playerInsideTrigger = false;
    private int currentTriggerIndex = -1;
    private bool dialogueInProcess = false;

    [SerializeField] private GameObject[] dialogueTriggers;
    private GameObject[] dialogueObjects;
    private Light[][] pointLights; // добавлено
    private float[][] originalIntensities; // добавлено
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueManager = FindAnyObjectByType<DialogueManager>();
        DialogueTrigger.OnDialogueTriggerEnter += OnPlayerEnterDialogueZone;
        DialogueTrigger.OnDialogueTriggerExit += OnPlayerExitDialogueZone;

        int triggerCount = dialogueTriggers.Length;
        lightObjects = new GameObject[triggerCount];
        pointLights = new Light[triggerCount][];
        originalIntensities = new float[triggerCount][];
        dialogueObjects = new GameObject[triggerCount];

        for (int i = 0; i < triggerCount; i++)
        {
            if (dialogueTriggers[i].transform.childCount > 0)
            {
                // Первый дочерний объект — предполагаемый контейнер света
                lightObjects[i] = dialogueTriggers[i].transform.GetChild(0).gameObject;
                dialogueObjects[i] = dialogueTriggers[i].transform.GetChild(1).gameObject;
                dialogueObjects[i].SetActive(false);

                // Получаем все Light внутри него
                Light[] lights = lightObjects[i].GetComponentsInChildren<Light>(true);
                pointLights[i] = lights;
                originalIntensities[i] = new float[lights.Length];

                // Сохраняем исходные интенсивности и сбрасываем в 0
                for (int j = 0; j < lights.Length; j++)
                {
                    originalIntensities[i][j] = lights[j].intensity;
                    lights[j].intensity = 0f;
                }
            }
            else
            {
                lightObjects[i] = null;
                pointLights[i] = new Light[0];
                originalIntensities[i] = new float[0];
                Debug.LogWarning($"DialogueTrigger {dialogueTriggers[i].name} не имеет дочерних объектов.");
            }
        }
    }
    private void OnPlayerEnterDialogueZone(DialogueTrigger currentTrigger)
    {
        trigger = currentTrigger;
        playerInsideTrigger = true;
        currentTriggerIndex = trigger.index;

        canvas = trigger.gameObject.GetComponentInChildren<Canvas>(true);
        canvas.gameObject.SetActive(true);

        StartCoroutine(FadeLightsForTrigger(currentTriggerIndex, 2f, true)); // Плавно включаем
    }
    private void OnPlayerExitDialogueZone(DialogueTrigger currentTrigger)
    {
        trigger = null;
        playerInsideTrigger = false;

        if (canvas.gameObject.activeSelf)
        {
            canvas.gameObject.SetActive(false);
        }

        dialogueInProcess = false;
        StartCoroutine(FadeLightsForTrigger(currentTriggerIndex, 2f, false));
        currentTriggerIndex = -1;
    }
    // Update is called once per frame
    void Update()
    {
        if (playerInsideTrigger && Input.GetKeyDown(KeyCode.E) && trigger.canInteract && !dialogueInProcess)
        {
            // запуск диалога с печкой и переключение объектов
            ForestCameraManager.Instance.SwitchToDialogueCamera(currentTriggerIndex);
            canvas.gameObject.SetActive(false);
            dialogueManager.PlayPartOfPlot("bake_dialogue");
            dialogueInProcess = true;
            ForestActionController.Instance.HidePerson();
            dialogueObjects[currentTriggerIndex].SetActive(true);
        }
        else if (playerInsideTrigger && Input.GetKeyDown(KeyCode.E) && trigger.canInteract && dialogueInProcess)
        {
            dialogueManager.StopDialogue();
            ForestCameraManager.Instance.SwitchToPlayerCamera();
            ForestActionController.Instance.ShowPerson();
            dialogueObjects[currentTriggerIndex].SetActive(false);
            canvas.gameObject.SetActive(true);
            dialogueInProcess = false;
        }
    }
    private IEnumerator FadeLightsForTrigger(int triggerIndex, float duration, bool turnOn)
    {
        float timer = 0f;
        int lightCount = pointLights[triggerIndex].Length;

        float[] startIntensities = new float[lightCount];
        float[] targetIntensities = new float[lightCount];

        for (int i = 0; i < lightCount; i++)
        {
            var light = pointLights[triggerIndex][i];
            startIntensities[i] = light.intensity;

            if (light.type == LightType.Point)
            {
                targetIntensities[i] = turnOn ? originalIntensities[triggerIndex][i] : 0f;
            }
            else
            {
                targetIntensities[i] = light.intensity; // не трогаем
            }
        }

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            for (int i = 0; i < lightCount; i++)
            {
                if (pointLights[triggerIndex][i].type == LightType.Point)
                {
                    pointLights[triggerIndex][i].intensity = Mathf.Lerp(startIntensities[i], targetIntensities[i], t);
                }
            }

            yield return null;
        }

        // Гарантируем финальное значение
        for (int i = 0; i < lightCount; i++)
        {
            if (pointLights[triggerIndex][i].type == LightType.Point)
            {
                pointLights[triggerIndex][i].intensity = targetIntensities[i];
            }
        }
    }


}
