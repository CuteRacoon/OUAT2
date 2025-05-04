using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Animator[] bookmarkAnimators = new Animator[3]; // Массив аниматоров закладок
    public SkinnedMeshRenderer bookMesh;
    public Material[] materials = new Material[3];
    public AudioClip[] sounds = new AudioClip[2];
    public GameObject chaptersButtons;
    
    private AudioSource audioSource;

    private int initialActiveBookmarkIndex = 0;
    private int currentActiveBookmarkIndex = -1; // Индекс текущей активной закладки. Инициализируем -1.

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Проверка на корректность данных
        if (bookmarkAnimators == null || bookmarkAnimators.Length == 0)
        {
            Debug.LogError("Bookmark Animators array is empty!  Assign animators in the inspector.");
            return;
        }

        if (initialActiveBookmarkIndex < 0 || initialActiveBookmarkIndex >= bookmarkAnimators.Length)
        {
            Debug.LogError("Invalid initial active bookmark index.  Setting to 0.");
            initialActiveBookmarkIndex = 0;
        }

        currentActiveBookmarkIndex = initialActiveBookmarkIndex;
        StartCoroutine(ResetMeshes(initialActiveBookmarkIndex, false));
        bookMesh.material = materials[0];
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
        //StartCoroutine(ResetTriggers());
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
        bookMesh.material = materials[index];
        if (index == 0)
        {
            chaptersButtons.SetActive(true);
        }
        else
        {
            chaptersButtons.SetActive(false);
        }
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

    public void PlayGame()
    {
        SceneManager.LoadScene("House");
        //Debug.Log("Меняю сцену на игровую");
    }
}

