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

    private int currentActiveChapterIndex = -1;
    private int? hoveredIndex = null;

    protected virtual void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
        DialogueManager.CanResetButtonsState += HandleResetButtons;
    }

    public void OnButtonClicked(int index)
    {
        if (index == currentActiveChapterIndex) return;

        currentActiveChapterIndex = index;
        hoveredIndex = null;
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

            if (i == currentActiveChapterIndex)
            {
                buttonImage.sprite = activeSprite;
            }
            else if (hoveredIndex != null && i == hoveredIndex)
            {
                buttonImage.sprite = hoverSprite != null ? hoverSprite : activeSprite;
            }
            else
            {
                buttonImage.sprite = inactiveSprite;
            }
        }
    }
    void HandleResetButtons()
    {
        ResetButtons();
    }
    void ResetButtons()
    {
        foreach (var button in buttons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.sprite = inactiveSprite;
        }
    }
    private void OnDestroy()
    {
        DialogueManager.CanResetButtonsState -= HandleResetButtons;
    }
}
