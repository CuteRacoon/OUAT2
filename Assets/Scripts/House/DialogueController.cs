using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private GameObject textThing;
    [SerializeField] private TextAsset inkFile;
    [SerializeField] private GameObject choiceButtonsParent;

    private Story story;
    private bool skipRequested;
    private bool isDialoguePlaying;
    private GameLogic gameLogic;

    private Color girlColor;
    private Color othersColor;
    private Button[] choiceButtons;

    void Start()
    {
        girlColor = new Color32(0xCD, 0x19, 0x19, 0xFF);
        othersColor = Color.white;
        choiceButtons = choiceButtonsParent.GetComponentsInChildren<Button>();

        gameLogic = FindAnyObjectByType<GameLogic>();
        text.text = "";
        textThing.SetActive(false);

        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }

        story = new Story(inkFile.text);
        PlayKnot("main");
    }
    public void EndGame(int dialogueIndex)
    {
        string knotName = $"end_{dialogueIndex}";
        PlayKnot(knotName);

        if (dialogueIndex != 3)
        {
            gameLogic.ResetGame();
        }
    }

    public void PlayKnot(string knotName)
    {
        if (story == null)
            story = new Story(inkFile.text);

        try
        {
            story.ChoosePathString(knotName);
        }
        catch
        {
            Debug.LogWarning($"Ќет узла: {knotName}");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(PlayMonologue());
    }

    private IEnumerator PlayMonologue()
    {
        isDialoguePlaying = true;
        textThing.SetActive(true);
        text.text = "";

        while (story.canContinue || story.currentChoices.Count > 0)
        {
            // если это просто отображение реплик
            if (story.canContinue)
            {
                text.color = girlColor;
                string line = story.Continue().Trim();
                string endLine = "- " + line;
                float delay = 3f;

                // обработка тега wait:1.5
                if (story.currentTags != null)
                {
                    foreach (string tag in story.currentTags)
                    {
                        if (tag.StartsWith("wait:") && float.TryParse(tag.Substring(5), out float parsedDelay))
                            delay = parsedDelay;
                        if (tag == "othersLine")
                        {
                            text.color = othersColor;
                            endLine = line;
                        }
                    }
                }

                text.text = endLine;
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
        textThing.SetActive(false);
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
        yield return new WaitForSeconds(1.5f);
        text.color = othersColor;
        StartCoroutine(PlayMonologue());
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
