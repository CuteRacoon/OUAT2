using UnityEngine;
using System.Collections;

public class ForestDialogueController : MonoBehaviour
{
    private DialogueManager dialogueManager;
    private DialogueTrigger trigger;

    private Canvas canvas;
    private GameObject[] lightObjects;

    public Material bakeEyesMaterial;
    private Coroutine emissionCoroutine;

    private bool playerInsideTrigger = false;
    private int currentTriggerIndex = -1;
    private bool dialogueInProcess = false;

    [SerializeField] private GameObject[] dialogueTriggers;
    private GameObject[] dialogueObjects;
    private Light[][] pointLights; // ���������
    private float[][] originalIntensities; // ���������
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ������������� ��������� ��������� ��������� (�������� �����)
        if (bakeEyesMaterial != null)
        {
            Color baseColor = new Color(186f / 255f, 186f / 255f, 186f / 255f); // RGB (186,186,186)
            //bakeEyesMaterial.color = baseColor;

            bakeEyesMaterial.EnableKeyword("_EMISSION");
            bakeEyesMaterial.SetColor("_EmissionColor", baseColor); // 0 ������� = �������� ���������
        }

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
                // ������ �������� ������ � �������������� ��������� �����
                lightObjects[i] = dialogueTriggers[i].transform.GetChild(0).gameObject;
                dialogueObjects[i] = dialogueTriggers[i].transform.GetChild(1).gameObject;
                dialogueObjects[i].SetActive(false);

                // �������� ��� Light ������ ����
                Light[] lights = lightObjects[i].GetComponentsInChildren<Light>(true);
                pointLights[i] = lights;
                originalIntensities[i] = new float[lights.Length];

                // ��������� �������� ������������� � ���������� � 0
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
                Debug.LogWarning($"DialogueTrigger {dialogueTriggers[i].name} �� ����� �������� ��������.");
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

        StartCoroutine(FadeLightsForTrigger(currentTriggerIndex, 2f, true)); // ������ ��������

        // ������� ���������� Emission �� ������ �� ����-��������
        if (emissionCoroutine != null) StopCoroutine(emissionCoroutine);
        emissionCoroutine = StartCoroutine(FadeEmissionColor(bakeEyesMaterial, Color.white, Color.red * 5f, 2f));
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

        // ������� �������� Emission �� ����-�������� � ������
        if (emissionCoroutine != null) StopCoroutine(emissionCoroutine);
        emissionCoroutine = StartCoroutine(FadeEmissionColor(bakeEyesMaterial, Color.red * 5f, Color.white, 2f));
    }
    private IEnumerator FadeEmissionColor(Material mat, Color fromColor, Color toColor, float duration)
    {
        float timer = 0f;

        mat.EnableKeyword("_EMISSION");

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            Color currentColor = Color.Lerp(fromColor, toColor, t);
            mat.SetColor("_EmissionColor", currentColor);
            yield return null;
        }

        mat.SetColor("_EmissionColor", toColor); // ����������� �����
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInsideTrigger && Input.GetKeyDown(KeyCode.E) && trigger.canInteract && !dialogueInProcess)
        {
            // ������ ������� � ������ � ������������ ��������
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
                targetIntensities[i] = light.intensity; // �� �������
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

        // ����������� ��������� ��������
        for (int i = 0; i < lightCount; i++)
        {
            if (pointLights[triggerIndex][i].type == LightType.Point)
            {
                pointLights[triggerIndex][i].intensity = targetIntensities[i];
            }
        }
    }


}
