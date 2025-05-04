using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

public class DialogueController : MonoBehaviour
{
    
    [SerializeField] private GameObject textPanel;
    [SerializeField] private GameObject learningPanel;
    [SerializeField] private TextAsset inkFile;
    [SerializeField] private GameObject choiceButtonsParent;

    HashSet<string> noDashTags = new HashSet<string> { "othersLine", "learningPhrase" };

    private Story story;
    private Text text;
    private Text learningText;
    private bool skipRequested;
    private bool isDialoguePlaying;
    public bool IsDialoguePlaying => isDialoguePlaying;
    private GameLogic gameLogic;

    public Color girlColor = new Color32(0xCD, 0x19, 0x19, 0xFF);
    public Color othersColor = Color.white;
    private Button[] choiceButtons;
    private float delay;
    private Coroutine dialogueCoroutine;

    void Start()
    {
        choiceButtons = choiceButtonsParent.GetComponentsInChildren<Button>();
        gameLogic = FindAnyObjectByType<GameLogic>();

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
    public void HideAllPanels()
    {
        textPanel.SetActive(false);
        learningPanel.SetActive(false);
    }
    public IEnumerator EndGame(int dialogueIndex)
    {
        string knotName = $"end_{dialogueIndex}";
        dialogueCoroutine = StartCoroutine(PlayKnot(knotName, false));
        yield return dialogueCoroutine;

        while (isDialoguePlaying)
            yield return null;

        if (dialogueIndex != 3)
        {
            gameLogic.ResetGame();
        }
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
            Debug.Log("ќстанавливаю корутину");
        }
        else Debug.Log(" орутина и так нулева€");

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
            Debug.LogWarning($"Ќет узла: {knotName}");
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
        learningText.text = "";

        while (story.canContinue || story.currentChoices.Count > 0)
        {
            // если это просто отображение реплик
            if (story.canContinue)
            {
                text.color = girlColor;
                string line = story.Continue().Trim();
                float delay = 3f;
                bool isLearning = false;
                bool shouldAddDash = true;

                if (story.currentTags != null)
                {
                    foreach (string tag in story.currentTags)
                    {
                        if (tag.StartsWith("wait:") && float.TryParse(tag.Substring(5), out float parsedDelay))
                            delay = parsedDelay;
                        else if (tag == "othersLine")
                            text.color = othersColor;
                        else if (tag == "learningPhrase")
                            isLearning = true;

                        if (noDashTags.Contains(tag))
                            shouldAddDash = false;
                    }
                }
                string endLine = shouldAddDash ? "- " + line : line;

                if (isLearning)
                {
                    learningPanel.SetActive(true);
                    learningText.text = endLine;
                }
                else
                {
                    textPanel.SetActive(true);
                    text.text = endLine;
                }
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
        learningPanel.SetActive(false);
        isDialoguePlaying = false;
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

                    // ѕоказ выбора игрока на экране
                    text.color = girlColor;
                    text.text = "- " + choices[choiceIndex].text.Trim();

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
            if (Input.GetMouseButtonDown(0))
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
        if (isDialoguePlaying && Input.GetMouseButtonDown(0))
        {
            skipRequested = true;
        }
    }
}
