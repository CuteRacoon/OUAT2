using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject[] pages; // страницы UI

    private List<AchievementDataManager.Achievement> achievements;

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        achievements = AchievementDataManager.Instance.GetAllAchievements();

        int index = 0;

        foreach (var page in pages)
        {
            foreach (Transform row in page.transform)
            {
                // собираем детей (например, 3 в ряду)
                List<Transform> achObjs = new List<Transform>();
                foreach (Transform achObj in row)
                {
                    achObjs.Add(achObj);
                }

                // идём по тройкам, но в обратном порядке
                for (int i = 0; i < achObjs.Count; i += 3)
                {
                    int count = Mathf.Min(3, achObjs.Count - i);

                    for (int j = 0; j < count; j++)
                    {
                        // берём объект «с конца» тройки
                        Transform achObj = achObjs[i + (count - 1 - j)];

                        var nameText = achObj.Find("Name")?.GetComponent<TextMeshProUGUI>();
                        var dateText = achObj.Find("Date")?.GetComponent<TextMeshProUGUI>();
                        var icon = achObj.Find("Image")?.GetComponent<Image>();
                        var descObj = achObj.Find("Popup");
                        var descText = descObj != null ? descObj.GetComponentInChildren<TextMeshProUGUI>(true) : null;

                        if (index < achievements.Count)
                        {
                            var ach = achievements[index];

                            if (nameText != null) nameText.text = ach.title;
                            if (descText != null) descText.text = ach.description;
                            if (ach.unlocked)
                            {
                                dateText.text = ach.dateUnlocked;
                                icon.gameObject.SetActive(true);
                            }
                            else
                            {
                                dateText.text = "---";
                                icon.gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            if (nameText != null) nameText.text = "---";
                            if (descText != null) descText.text = "---";
                            if (dateText != null) dateText.text = "---";
                            if (icon != null) icon.gameObject.SetActive(false);
                        }

                        index++;
                    }
                }
            }
        }
    }
}
