using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuButtonsManager : MonoBehaviour
{
    protected Button[] buttons;         // Кнопки переключения глав
    public Sprite activeSprite;             // Спрайт активной кнопки
    public Sprite inactiveSprite;           // Спрайт неактивной кнопки
    public Sprite hoverSprite;

    public bool changeState = false;
    public bool isSwitcher = false;

    private int currentActiveChapterIndex = -1;
    private int? hoveredIndex = null;
    private bool[] buttonStates; // Для хранения состояния каждой кнопки

    protected virtual void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
        buttonStates = new bool[buttons.Length];
        DialogueManager.CanResetButtonsState += HandleResetButtons;
    }

    public void OnButtonClicked(int index)
    {
        // Обработка переключателя
        if (isSwitcher)
        {
            // Переключаем состояние кнопки
            buttonStates[index] = !buttonStates[index];

            // Обновляем активный индекс (для совместимости с остальной логикой)
            currentActiveChapterIndex = buttonStates[index] ? index : -1;

            hoveredIndex = null;
            UpdateButtonStates();
            return;
        }
        if (index == currentActiveChapterIndex && !changeState)
        {
            return;
        }

        currentActiveChapterIndex = index;
        hoveredIndex = null;
        UpdateButtonStates();

        if (changeState)
        {
            StartCoroutine(ResetActiveStateAfterDelay(0.1f));
        }
    }
    private IEnumerator ResetActiveStateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentActiveChapterIndex = -1;
        if (buttonStates != null && currentActiveChapterIndex >= 0 && currentActiveChapterIndex < buttonStates.Length)
        {
            buttonStates[currentActiveChapterIndex] = false;
        }
        UpdateButtonStates();
    }

    public void OnPointerEnter(int index)
    {
        hoveredIndex = index;
        UpdateButtonStates();
    }
    public void OnPointerExit()
    {
        hoveredIndex = null;
        UpdateButtonStates();
    }

    void UpdateButtonStates()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            Image buttonImage = buttons[i].GetComponent<Image>();
            if (buttonImage == null) continue;
            bool isActive = isSwitcher ? buttonStates[i] : (i == currentActiveChapterIndex);

            if (hoveredIndex != null && i == hoveredIndex)
            {
                buttonImage.sprite = hoverSprite != null ? hoverSprite : activeSprite;
            }
            else
            {
                buttonImage.sprite = isActive ? activeSprite : inactiveSprite;
            }
        }
    }
    void HandleResetButtons()
    {
        ResetButtons();
    }
    void ResetButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            Image buttonImage = buttons[i].GetComponent<Image>();
            if (buttonImage != null)
                buttonImage.sprite = inactiveSprite;

            if (buttonStates != null && i < buttonStates.Length)
                buttonStates[i] = false;
        }
        currentActiveChapterIndex = -1;
        hoveredIndex = null;
    }
    private void OnDestroy()
    {
        DialogueManager.CanResetButtonsState -= HandleResetButtons;
    }
}
