using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using System.Collections;

public class InkQuestManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI mainTaskText;
    public TextMeshProUGUI additionalTaskText;

    [Header("Ink")]
    public TextAsset inkJSONAsset; // скомпилированный story.json


    private Story story;
    private List<string> additionalTasks = new List<string>();
    private string currentMainTask = "";
    private int currentAdditionalIndex = 0;

    void Start()
    {
        if (inkJSONAsset == null)
        {
            Debug.LogError("InkQuestManager: назначь story.json (компилированный Ink) в поле inkJSONAsset.");
            return;
        }
        story = new Story(inkJSONAsset.text);

        // «агружаем первый knot
        ChooseKnot("main_task_1");
    }
    public void ChooseKnot(string knotName)
    {
        /*if (!story.KnotContainerExists(knotName))
        {
            Debug.LogWarning($"QuestManagerInk: knot '{knotName}' не найден!");
            return;
        }*/

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

            if (string.IsNullOrEmpty(line)) continue;

            // ѕровер€ем теги текущей строки
            if (story.currentTags != null)
            {
                foreach (string tag in story.currentTags)
                {
                    if (tag == "main")
                    {
                        currentMainTask = line;
                        Debug.Log($"[QuestManager] Main task: {line}");
                    }
                    else if (tag == "sub")
                    {
                        additionalTasks.Add(line);
                        Debug.Log($"[QuestManager] Sub task: {line}");
                    }
                }
            }

            yield return null; // можно убрать, если не надо пошагово
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        mainTaskText.text = currentMainTask;

        if (additionalTasks.Count > 0)
            additionalTaskText.text = additionalTasks[currentAdditionalIndex];
        else
            additionalTaskText.text = "";
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
}
