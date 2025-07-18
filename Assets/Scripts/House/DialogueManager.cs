using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using System;
using System.Globalization;
using TMPro;

public class DialogueManager : MonoBehaviour
{

    public GameObject textPanel;
    public GameObject learningPanel;
    [SerializeField] private TextAsset inkFile;
    [SerializeField] private GameObject choiceButtonsParent;
    [SerializeField] private GameObject personNameObj;

    private Story story;
    private TextMeshProUGUI text;
    private TextMeshProUGUI learningText;
    private TextMeshProUGUI personNameText;
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
        { 1, "������" },
        { 2, "������"},
        { 3, "�����" },
        { 4, "�����"},
        { 5, "�������"},
        { 6, "������"},
        { 7, "�������"}
    };
    public static DialogueManager Instance { get; private set; }
    public static event Action CanResetButtonsState;
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

        text = textPanel.GetComponentInChildren<TextMeshProUGUI>();
        textPanel.SetActive(false);
        personNameText = personNameObj.GetComponentInChildren<TextMeshProUGUI>();

        learningText = learningPanel.GetComponentInChildren<TextMeshProUGUI>();
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
    public void ShowAllPanels()
    {
        textPanel.SetActive(true);
        learningPanel.SetActive(true);
    }

    public void PlayPartOfPlot(string knotName)
    {
        story = new Story(inkFile.text);
        dialogueCoroutine = StartCoroutine(PlayKnot(knotName, false));

    }
    public void StopDialogue()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
            skipRequested = true;
            Debug.Log("������������ ��������");
        }
        else Debug.Log("�������� � ��� �������");

        isDialoguePlaying = false;
        choiceButtonsParent.SetActive(false);
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
            Debug.LogWarning($"��� ����: {knotName}");
            yield break;
        }

        //StopAllCoroutines();
        dialogueCoroutine = StartCoroutine(PlayMonologue());
    }
    public void PlayPartOfPlotWithDelay(string knotName, float neededDelay)
    {
        delay = neededDelay;
        story = new Story(inkFile.text);
        dialogueCoroutine = StartCoroutine(PlayKnot(knotName, true));
    }

    private IEnumerator PlayMonologue()
    {
        isDialoguePlaying = true;
        text.text = "";

        while (story.canContinue || story.currentChoices.Count > 0)
        {
            // ���� ��� ������ ����������� ������
            if (story.canContinue)
            {
                text.color = girlColor;
                string line = story.Continue().Trim();
                float delay = 3f;
                personNameText.text = "";
                personNameObj.SetActive(false);

                if (story.currentTags != null)
                {
                    foreach (string tag in story.currentTags)
                    {
                        if (tag.StartsWith("wait:") &&
                            float.TryParse(tag.Substring(5), NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedDelay))
                        {
                            delay = parsedDelay;
                            Debug.Log($"[Monologue] Wait tag found, delay = {parsedDelay} seconds");
                        }
                        else if (tag.StartsWith("othersLine_"))
                        {
                            text.color = othersColor;

                            if (int.TryParse(tag.Substring("othersLine_".Length), out int index))
                            {
                                if (othersNames.TryGetValue(index, out string name))
                                {
                                    personNameText.text = name;
                                    personNameObj.SetActive(true);
                                }
                                else
                                {
                                    personNameText.text = "???";
                                }
                            }
                        }
                    }
                }


                textPanel.SetActive(true);

                text.text = line;
                yield return WaitOrSkip(delay);
            }

            // ���� ��� ������ � �������
            if (story.currentChoices.Count > 0)
            {
                DisplayChoices();
                yield break;
            }
        }
        if (!story.canContinue && story.currentChoices.Count == 0)
        {
            isDialoguePlaying = false;
        }

        yield return new WaitForSeconds(0.5f);
        textPanel.SetActive(false);
        isDialoguePlaying = false;
        personNameObj.SetActive(false);
        canSkip = true;
    }

    private void DisplayChoices()
    {
        List<Choice> choices = story.currentChoices;
        CanResetButtonsState?.Invoke();
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

                    // ����� ������ ������ �� ������
                    text.color = girlColor;
                    personNameText.text = "";
                    personNameObj.SetActive(false);
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
        yield return WaitOrSkip(3f); // <- ������ ����� �������� � �������� ��������

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
