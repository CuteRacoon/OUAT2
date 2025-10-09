
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using System.Collections;

public class InkQuestManager : MonoBehaviour
{
    public static InkQuestManager Instance { get; private set; }

    [Header("UI (Day)")]
    public TextMeshProUGUI mainTaskText;
    public TextMeshProUGUI additionalTaskText;

    [Header("UI (Night)")]
    public TextMeshProUGUI mainTaskTextNight;
    public TextMeshProUGUI additionalTaskTextNight;

    [Header("Ink")]
    public TextAsset inkJSONAsset; // скомпилированный story.json

    private Story story;
    private List<string> additionalTasks = new List<string>();
    private string currentMainTask = "";
    private int currentAdditionalIndex = 0;

    private void Awake()
    {
        // Синглтон
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        if (inkJSONAsset == null)
        {
            Debug.LogError("InkQuestManager: назначь story.json (скомпилированный Ink) в поле inkJSONAsset.");
            return;
        }

        story = new Story(inkJSONAsset.text);
        ChooseKnot("main_task_1");
    }

    public void ChooseKnot(string knotName)
    {
        story.ChoosePathString(knotName);
        StartCoroutine(ReadQuestBlock());
    }

    private IEnumerator ReadQuestBlock()
    {
        currentMainTask = "";
        additionalTasks.Clear();
        currentAdditionalIndex = 0;

        while (story.canContinue)
        {
            string line = story.Continue().Trim();

            if (string.IsNullOrEmpty(line))
                continue;

            if (story.currentTags != null)
            {
                foreach (string tag in story.currentTags)
                {
                    if (tag == "main")
                        currentMainTask = line;
                    else if (tag == "sub")
                        additionalTasks.Add(line);
                }
            }

            yield return null;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        // Определяем, ночь сейчас или день
        bool isNight = ActionManager.Instance != null && ActionManager.Instance.isNight;

        // Выбираем нужные TextMeshPro поля
        TextMeshProUGUI mainUI = isNight ? mainTaskTextNight : mainTaskText;
        TextMeshProUGUI addUI = isNight ? additionalTaskTextNight : additionalTaskText;

        if (mainUI != null)
            mainUI.text = currentMainTask;

        if (addUI != null)
        {
            if (additionalTasks.Count > 0)
                addUI.text = additionalTasks[currentAdditionalIndex];
            else
                addUI.text = "";
        }

        // Скрываем/показываем ненужный UI
        if (mainTaskText != null) mainTaskText.gameObject.SetActive(!isNight);
        if (additionalTaskText != null) additionalTaskText.gameObject.SetActive(!isNight);

        if (mainTaskTextNight != null) mainTaskTextNight.gameObject.SetActive(isNight);
        if (additionalTaskTextNight != null) additionalTaskTextNight.gameObject.SetActive(isNight);
    }

    public void NextAdditionalTask()
    {
        if (additionalTasks.Count == 0) return;
        currentAdditionalIndex = (currentAdditionalIndex + 1) % additionalTasks.Count;
        UpdateUI();
    }

    public void NextMainTask(int index)
    {
        string knotName = $"main_task_{index}";
        ChooseKnot(knotName);
    }

    public void SetQuestUIVisible(bool visible)
    {
        bool isNight = ActionManager.Instance != null && ActionManager.Instance.isNight;

        if (!isNight)
        {
            if (mainTaskText != null)
                mainTaskText.gameObject.SetActive(visible);
            if (additionalTaskText != null)
                additionalTaskText.gameObject.SetActive(visible);
        }
        else
        {
            if (mainTaskTextNight != null)
                mainTaskTextNight.gameObject.SetActive(visible);
            if (additionalTaskTextNight != null)
                additionalTaskTextNight.gameObject.SetActive(visible);
        }
    }

    public void SetAdditionalTaskByIndex(int index)
    {
        if (additionalTasks.Count == 0)
        {
            Debug.LogWarning("[InkQuestManager] Нет дополнительных задач для переключения!");
            return;
        }

        if (index < 1 || index > additionalTasks.Count)
        {
            Debug.LogWarning($"[InkQuestManager] Индекс {index} вне диапазона (1 - {additionalTasks.Count})!");
            return;
        }

        currentAdditionalIndex = index - 1;
        UpdateUI();
    }
}
