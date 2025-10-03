using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementPopup : MonoBehaviour
{
    public CanvasGroup canvasGroup;   // для прозрачности

    private void Start()
    {
        // Подписка на событие
        AchievementDataManager.Instance.OnAchievementUnlocked += ShowPopup;
        canvasGroup.alpha = 0; // скрыто по умолчанию
    }

    private void OnDestroy()
    {
        if (AchievementDataManager.Instance != null)
            AchievementDataManager.Instance.OnAchievementUnlocked -= ShowPopup;
    }

    public void ShowPopup(AchievementDataManager.Achievement ach)
    {
        StopAllCoroutines();
        StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        // плавное появление
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f; // скорость появления
            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }

        // держим 3 секунды
        yield return new WaitForSeconds(3f);

        // плавное исчезновение
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }
    }
}
