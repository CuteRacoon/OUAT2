using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AchievementDataManager : MonoBehaviour
{
    public static AchievementDataManager Instance { get; private set; }

    [Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public bool unlocked;
        public string dateUnlocked;

        public Achievement(string id, string title, string description)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            unlocked = false;
            dateUnlocked = "";
        }
    }

    private Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();
    private List<Achievement> achievementList = new List<Achievement>();

    public event Action<Achievement> OnAchievementUnlocked;
    private string savePath;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "achievements.json");

        InitAchievements();
        LoadAchievements();
    }

    private void InitAchievements()
    {
        AddAchievement("another_way", "Другой способ", "Узнайте о другом способе выбраться из леса");
        AddAchievement("did_you_hear", "Слыхали?", "Узнайте все деревенские сплетни");
        AddAchievement("to_home_all", "Домой", "Спасите брата и сестру");
        AddAchievement("to_home_brother", "Лишь один", "Спасите только брата");
        AddAchievement("to_home_none", "Шаги смолкли", "Оставьте обоих детей в лесу");
        AddAchievement("truth_disease", "Не болезнь", "Узнайте правду о болезни, поразившей брата");
        AddAchievement("friend_of_oven", "Тёплое общение", "Понравьтесь Печке");
        AddAchievement("master_hand", "Рука мастера", "Сварите зелье с первой попытки");
    }

    private void AddAchievement(string id, string title, string description)
    {
        if (!achievements.ContainsKey(id))
        {
            var ach = new Achievement(id, title, description);
            achievements.Add(id, ach);
            achievementList.Add(ach);
        }
    }
    public void TestFunc()
    {
        Unlock("did_you_hear");
    }

    public void Unlock(string id)
    {
        if (achievements.TryGetValue(id, out Achievement ach))
        {
            if (!ach.unlocked)
            {
                ach.unlocked = true;
                ach.dateUnlocked = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                SaveAchievements();
                Debug.Log($"Достижение получено: {ach.title} ({ach.dateUnlocked})");

                OnAchievementUnlocked?.Invoke(ach);
            }
        }
        else Debug.LogWarning($"Ачивка с id {id} не найдена!");
    }

    public bool IsUnlocked(string id)
    {
        return achievements.ContainsKey(id) && achievements[id].unlocked;
    }

    public List<Achievement> GetAllAchievements()
    {
        return new List<Achievement>(achievementList);
    }

    private void SaveAchievements()
    {
        string json = JsonUtility.ToJson(new AchievementSaveWrapper(achievementList), true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Achievements saved to {savePath}");
    }

    private void LoadAchievements()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            var wrapper = JsonUtility.FromJson<AchievementSaveWrapper>(json);
            foreach (var savedAch in wrapper.achievements)
            {
                if (achievements.ContainsKey(savedAch.id))
                {
                    var ach = achievements[savedAch.id];
                    ach.unlocked = savedAch.unlocked;
                    ach.dateUnlocked = savedAch.dateUnlocked;
                }
            }
        }
    }

    [Serializable]
    private class AchievementSaveWrapper
    {
        public List<Achievement> achievements;

        public AchievementSaveWrapper(List<Achievement> achievements)
        {
            this.achievements = achievements;
        }
    }
}
