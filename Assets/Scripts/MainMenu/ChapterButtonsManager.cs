using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChapterButtonsManager : MainMenuButtonsManager
{
    private MainMenuController mainMenuController;
    private string[] chapterTexts;

    void Start()
    {
        base.Awake();
        mainMenuController = FindAnyObjectByType<MainMenuController>();

        chapterTexts = new string[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            TMP_Text tmpText = buttons[i].GetComponentInChildren<TMP_Text>();
            chapterTexts[i] = tmpText != null ? tmpText.text : "";
        }
    }

    public new void OnButtonClicked(int index)
    {
        base.OnButtonClicked(index); // визуальное переключение

        if (mainMenuController != null)
        {
            string textToSend = (chapterTexts != null && index < chapterTexts.Length) ? chapterTexts[index] : "";
            mainMenuController.SetRightPageInfo(index, textToSend);
        }
        else
        {
            Debug.LogWarning("MainMenuController не найден!");
        }
    }
}
