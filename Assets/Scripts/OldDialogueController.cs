using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

public class DialogueController2 : MonoBehaviour
{
    public Text text;
    public GameObject textThing;
    public TextAsset monologueLines;

    private GameLogic gameLogic;
    private Story story;
    private bool skipRequested;
    private bool isDialoguePlaying = false; // Новый флаг

    void Start()
    {
        text.text = null;
        textThing.SetActive(false);
        gameLogic = FindAnyObjectByType<GameLogic>();
        story = new Story(monologueLines.text);
    }

    public IEnumerator EndGame(int dialogueIndex)
    {
        string knotName = $"end_{dialogueIndex}";
        yield return StartCoroutine(PlayStory(knotName));

        if (dialogueIndex != 3)
        {
            gameLogic.ResetGame();
        }
    }
    // Метод для обработки строк монолога внутри файла: если нет wait, воспроизводит с заданной задержкой
    private IEnumerator PlayPhrases()
    {
        isDialoguePlaying = true;
        textThing.SetActive(true);
        text.text = "";

        while (story.canContinue)
        {
            skipRequested = false;

            string line = story.Continue().Trim();
            float delay = 3f;

            if (story.currentTags != null)
            {
                foreach (string tag in story.currentTags)
                {
                    if (tag.StartsWith("wait:") && float.TryParse(tag.Substring(5), out float parsedDelay))
                    {
                        delay = parsedDelay;
                    }
                }
            }

            text.text = line;

            float timer = 0f;
            while (timer < delay && !skipRequested)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    skipRequested = true;
                    break;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        textThing.SetActive(false);
        isDialoguePlaying = false;
    }

    // Метод для проигрывания абсолютно любой истории в файле, даже извне
    public IEnumerator PlayStory(string knotName)
    {
        try
        {
            story.ChoosePathString(knotName);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Нет узла {knotName} в Ink-файле: {e.Message}");
            yield break;
        }
        yield return StartCoroutine(PlayPhrases());
    }

    void Update()
    {
        if (isDialoguePlaying && Input.GetMouseButtonDown(0))
        {
            skipRequested = true;
        }
    }
}
