using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Animator[] bookmarkAnimators = new Animator[3]; // Массив аниматоров закладок
    public SkinnedMeshRenderer bookMesh;
    public Material[] materials = new Material[4];
    public AudioClip[] sounds = new AudioClip[2];
    public GameObject RightPage;
    
    private AudioSource audioSource;
    private GameObject canvas;
    private GameObject[] pagesUI = new GameObject[3];
    private TMP_Text titleText;
    private TMP_Text inDevText;
    private GameObject actionChaptersButtons;

    private bool chaptersSelected = false;
    private int initialActiveBookmarkIndex = 0;
    private int currentActiveBookmarkIndex = -1; // Индекс текущей активной закладки. Инициализируем -1.

    void Start()
    {
        canvas = GameObject.FindWithTag("Canvas");
        if (canvas != null)
        {
            canvas.SetActive(false);
            if (RightPage != null)
            {
                // Получаем все TMP_Text компоненты среди прямых детей
                var texts = RightPage.transform.Cast<Transform>()
                    .Select(t => t.GetComponent<TMP_Text>())
                    .Where(t => t != null)
                    .ToArray();

                if (texts.Length >= 2)
                {
                    titleText = texts[0];
                    inDevText = texts[1];
                }
                else
                {
                    Debug.LogWarning("RightPage содержит меньше двух TMP_Text компонентов.");
                }

                // Получаем первый не-текстовый объект (например, декорация или иконка)
                actionChaptersButtons = RightPage.transform.Cast<Transform>()
                    .Select(t => t.gameObject)
                    .FirstOrDefault(go => go.GetComponent<TMP_Text>() == null);
            }
        }
        audioSource = GetComponent<AudioSource>();
        // Проверка на корректность данных
        if (bookmarkAnimators == null || bookmarkAnimators.Length == 0)
        {
            Debug.LogError("Bookmark Animators array is empty!  Assign animators in the inspector.");
            return;
        }
        GameObject[] foundPages = System.Array.FindAll(canvas.GetComponentsInChildren<Transform>(true),
        t => t.CompareTag("PagesUI")).Select(t => t.gameObject).ToArray();

        if (foundPages.Length != 3)
        {
            Debug.LogWarning("Ожидается ровно 3 объекта с тегом 'PagesUI'. Найдено: " + foundPages.Length);
        }
        // Назначаем в массив по порядку (предполагаем, что порядок неважен)
        for (int i = 0; i < Mathf.Min(3, foundPages.Length); i++)
        {
            pagesUI[i] = foundPages[i];
            pagesUI[i].SetActive(false); // выключаем все в начале
        }
        pagesUI[0].SetActive(true);

        if (initialActiveBookmarkIndex < 0 || initialActiveBookmarkIndex >= bookmarkAnimators.Length)
        {
            Debug.LogError("Invalid initial active bookmark index.  Setting to 0.");
            initialActiveBookmarkIndex = 0;
        }

        currentActiveBookmarkIndex = initialActiveBookmarkIndex;
        StartCoroutine(ResetMeshes(initialActiveBookmarkIndex, false));
    }

    public void OnBookmarkButtonClicked(int bookmarkIndex)
    {
        //bookmarkAnimators[bookmarkIndex].SetTrigger("NeedToHide");

        if (bookmarkIndex < 0 || bookmarkIndex >= bookmarkAnimators.Length)
        {
            Debug.LogError("Invalid bookmark index: " + bookmarkIndex);
            return;
        }

        // Если пытаемся показать уже активную закладку, ничего не делаем
        if (bookmarkIndex == currentActiveBookmarkIndex) { return; }

        // Скрываем текущую активную закладку, если она есть
        if (currentActiveBookmarkIndex >= 0 && currentActiveBookmarkIndex < bookmarkAnimators.Length)
        {
            HideBookmark(currentActiveBookmarkIndex);
        }
        ShowBookmark(bookmarkIndex);

        currentActiveBookmarkIndex = bookmarkIndex;
    }
    // метод для обновления материалов на страницах
    private IEnumerator ResetMeshes(int index, bool needSound)
    {
        if (needSound)
        {
            audioSource.volume = 0.15f;
            audioSource.PlayOneShot(sounds[0]);
        }
        yield return new WaitForSeconds(0.2f);
        if (chaptersSelected && index == 0) {
            bookMesh.material = materials[3];
        }
        else bookMesh.material = materials[index];
        // Активация нужной страницы UI
        for (int i = 0; i < pagesUI.Length; i++)
        {
            if (pagesUI[i] != null)
            {
                if (i == index)
                {
                    StartCoroutine(FadeInCanvas(pagesUI[i], 0.2f));
                }
                else
                {
                    pagesUI[i].SetActive(false);
                }
            }
        }
    }
    public void SetRightPageInfo(int index, string text)
    {
        if (index == 0)
        {
            bookMesh.material = materials[3];
            actionChaptersButtons.SetActive(true);
            inDevText.gameObject.SetActive(false);
            chaptersSelected = true;
        }
        else
        {
            chaptersSelected = false;
            bookMesh.material = materials[0];
            actionChaptersButtons.SetActive(false);
            inDevText.gameObject.SetActive(true);
        }
        if (!titleText.gameObject.activeSelf) titleText.gameObject.SetActive(true);
        titleText.text = text;
    }
    private void ShowBookmark(int index)
    {
        bookmarkAnimators[index].SetTrigger("NeedToShow");
        StartCoroutine(ResetMeshes(index, true));
    }

    private void HideBookmark(int index)
    {
        bookmarkAnimators[index].SetTrigger("NeedToHide");
        StartCoroutine(ResetMeshes(index, true));
    }
    public void ActivateCanvas()
    {
        StartCoroutine(FadeInCanvas(canvas, 0.5f));
        titleText.gameObject.SetActive(false);
        inDevText.gameObject.SetActive(false);
        actionChaptersButtons.SetActive(false);
    }
    private IEnumerator FadeInCanvas(GameObject canvas, float duration)
    {
        if (canvas == null) yield break;

        canvas.SetActive(true);

        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = canvas.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("House");
        //Debug.Log("Меняю сцену на игровую");
    }
}

