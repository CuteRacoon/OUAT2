using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using System;

public class DialogueManager : MonoBehaviour
{
    
    [SerializeField] private GameObject textPanel;
    [SerializeField] private GameObject learningPanel;
    [SerializeField] private TextAsset inkFile;
    [SerializeField] private GameObject choiceButtonsParent;
    [SerializeField] private Text personNameText;

    private Story story;
    private Text text;
    private Text learningText;
    private bool skipRequested;
    private bool canSkip = true;

    private bool isDialoguePlaying;
    public bool IsDialoguePlaying => isDialoguePlaying;

    public Color girlColor = new Color32(0xCD, 0x19, 0x19, 0xFF);
    public Color othersColor = Color.white;

    private Button[] choiceButtons;
    private float delay;
    private Coroutine dialogueCoroutine;

    private Dictionary<int, string> othersNames = new Dictionary<int, string>
    {
        { 1, "Всемил" },
        { 2, "Агафья"},
        { 3, "Марфа" },
        { 4, "Печка"}
    };
    public static DialogueManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        choiceButtons = choiceButtonsParent.GetComponentsInChildren<Button>();

        text = textPanel.GetComponentInChildren<Text>();
        textPanel.SetActive(false);

        learningText = learningPanel.GetComponentInChildren<Text>();
        learningPanel.SetActive(false);

        text.text = learningText.text = "";

        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }

        story = new Story(inkFile.text);
    }
    public void LearningPanelText(string text)
    {
        learningText.text = text;
        learningPanel.SetActive(true);
    }
    public void BlockSkippingForOneKnot()
    {
        canSkip = false;
    }
    public void HideAllPanels()
    {
        textPanel.SetActive(false);
        learningPanel.SetActive(false);
    }

    public void PlayPartOfPlot(string knotName)
    {
        dialogueCoroutine = StartCoroutine(PlayKnot(knotName, false));
    }
    public void StopDialogue()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
            skipRequested = true;
            Debug.Log("Останавливаю корутину");
        }
        else Debug.Log("Корутина и так нулевая");

        isDialoguePlaying = false;
        HideAllPanels();
    }
    private IEnumerator PlayKnot(string knotName, bool isDelayNeeded)
    {
        if (isDelayNeeded)
        {
            yield return new WaitForSeconds(delay);
        }
        isDialoguePlaying = true;
        if (story == null)
            story = new Story(inkFile.text);

        try
        {
            story.ChoosePathString(knotName);
        }
        catch
        {
            Debug.LogWarning($"Нет узла: {knotName}");
            yield break;
        }

        //StopAllCoroutines();
        dialogueCoroutine = StartCoroutine(PlayMonologue());
    }
    public void PlayPartOfPlotWithDelay(string knotName, float neededDelay)
    {
        delay = neededDelay;
        dialogueCoroutine = StartCoroutine(PlayKnot(knotName, true));
    }

    private IEnumerator PlayMonologue()
    {
        isDialoguePlaying = true;
        text.text = "";

        while (story.canContinue || story.currentChoices.Count > 0)
        {
            // если это просто отображение реплик
            if (story.canContinue)
            {
                text.color = girlColor;
                string line = story.Continue().Trim();
                float delay = 3f;
                personNameText.text = "";

                if (story.currentTags != null)
                {
                    foreach (string tag in story.currentTags)
                    {
                        if (tag.StartsWith("wait:") && float.TryParse(tag.Substring(5), out float parsedDelay))
                        {
                            delay = parsedDelay;
                        }
                        else if (tag.StartsWith("othersLine_"))
                        {
                            text.color = othersColor;

                            if (int.TryParse(tag.Substring("othersLine_".Length), out int index))
                            {
                                if (othersNames.TryGetValue(index, out string name))
                                    personNameText.text = name;
                                else
                                    personNameText.text = "???";
                            }
                        }
                    }
                }

                textPanel.SetActive(true);
                text.text = line;
                yield return WaitOrSkip(delay);
            } 

            // если это диалог с выбором
            if (story.currentChoices.Count > 0)
            {
                DisplayChoices();
                yield break;
            }
        }

        yield return new WaitForSeconds(0.5f);
        textPanel.SetActive(false);
        isDialoguePlaying = false;
        canSkip = true;
    }

    private void DisplayChoices()
    {
        List<Choice> choices = story.currentChoices;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<Text>().text = choices[i].text.Trim();
                int choiceIndex = i;

                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() =>
                {
                    foreach (var btn in choiceButtons)
                        btn.gameObject.SetActive(false);

                    // Показ выбора игрока на экране
                    text.color = girlColor;
                    personNameText.text = "";
                    text.text = choices[choiceIndex].text.Trim();

                    story.ChooseChoiceIndex(choiceIndex);
                    StartCoroutine(ShowChoiceThenContinue());
                });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator ShowChoiceThenContinue()
    {
        personNameText.text = "";
        skipRequested = false;
        yield return WaitOrSkip(3f); // <- теперь можно кликнуть и прервать ожидание

        text.color = othersColor;
        dialogueCoroutine = StartCoroutine(PlayMonologue());
    }

    private IEnumerator WaitOrSkip(float duration)
    {
        float timer = 0f;
        skipRequested = false;

        while (timer < duration && !skipRequested)
        {
            if (Input.GetMouseButtonDown(0) && canSkip)
            {
                skipRequested = true;
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    void Update()
    {
        if (isDialoguePlaying && Input.GetMouseButtonDown(0) && canSkip)
        {
            skipRequested = true;
        }
    }
}
