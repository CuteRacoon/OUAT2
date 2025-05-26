using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameLogicManager : MonoBehaviour
{
    public GameObject[] objectsToOutline;
    // тоже сделать сигнал какой-нибудь
    public bool isGameOver = false;

    private static GameObject currentObject;
    private int rootIndex;
    private int neededBowlIndex = -1;

    private AnimationsManager animationsControl;

    // Словари для хранения объектов по тегам и индексам
    private Dictionary<int, GameObject> Roots = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> Berries = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> Bowls = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> Herbs = new Dictionary<int, GameObject>();

    private List<KeyValuePair<int, int>> CollectedObjects = new List<KeyValuePair<int, int>>();
    public static MiniGameLogicManager Instance { get; private set; }
    public static event Action<int> OnGameEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        DialogueManager.CanResetGame += HandleResetGame;
        animationsControl = FindAnyObjectByType<AnimationsManager>();
        currentObject = null;
        PopulateResources();

        AccessHerbsAndBerriesInteraction(false);
        AccessBowls(2, false);
        AccessBowls(1, false);
        AccessBowls(3, false);
    }
    void HandleResetGame()
    {
        ResetGame();
    }
    public void EndGame()
    {
        Debug.Log("Игра закончена");
        CheckIngredients();
    }
    public void ResetGame()
    {
        isGameOver = false;

        SetActiveAllIngredients();
        animationsControl.ResetWaters();

        AccessHerbsAndBerriesInteraction(false);
        AccessBowls(2, false);
        AccessBowls(1, false);
        AccessBowls(3, false);
        Berries[1].GetComponent<Berries>().ResetBerries();
        Berries[2].GetComponent<Berries>().ResetBerries();
        Berries[3].GetComponent<Berries>().ResetBerries();
        CollectedObjects.Clear();
    }
    public void SetActiveAllIngredients()
    {
        foreach (var pair in Roots)
        {
            GameObject obj = pair.Value;
            obj.layer = 0;
            obj.SetActive(true);
        }
        foreach (var pair in Berries)
        {
            GameObject obj = pair.Value;
            obj.layer = 0;
            obj.SetActive(true);
        }
        foreach (var pair in Herbs)
        {
            GameObject obj = pair.Value;
            obj.layer = 0;
            obj.SetActive(true);
        }
        foreach (var pair in Bowls)
        {
            GameObject obj = pair.Value;
            obj.layer = 0;
            obj.SetActive(true);
        }
    }
    public void CheckIngredients()
    {
        animationsControl.ObjectsOn(-1, 3);
        // Ожидаемый список
        List<KeyValuePair<int, int>> expectedObjects = new List<KeyValuePair<int, int>>()
        {
            new KeyValuePair<int, int>(0, 1),
            new KeyValuePair<int, int>(1, 3),
            new KeyValuePair<int, int>(2, 1),
            new KeyValuePair<int, int>(2, 2)
        };

        SortList(CollectedObjects);
        SortList(expectedObjects);

        // Считаем количество несовпадений
        int differences = CountDifferences(CollectedObjects, expectedObjects);
        if (differences > 0 && differences <= 2)
        {
            OnGameEnded?.Invoke(1);
            Debug.Log("Количество несовпадений: " + differences);
            animationsControl.ObjectsOn(2, 4);
        }
        else if (differences > 2)
        {
            OnGameEnded?.Invoke(2);
            Debug.Log("Количество несовпадений: " + differences);
            animationsControl.ObjectsOn(3, 4);
        }
        else if (differences == 0 && CollectedObjects.Count == expectedObjects.Count)
        {
            OnGameEnded?.Invoke(3);
            Debug.Log("Количество несовпадений: " + differences);
            animationsControl.ObjectsOn(4, 4);
        }
    }
    public void SortList(List<KeyValuePair<int, int>> list)
    {
        list.Sort((x, y) =>
        {
            int keyComparison = x.Key.CompareTo(y.Key);
            return keyComparison != 0 ? keyComparison : x.Value.CompareTo(y.Value);
        });
    }
    private int CountDifferences(List<KeyValuePair<int, int>> list1, List<KeyValuePair<int, int>> list2)
    {
        int differences = Mathf.Abs(list1.Count - list2.Count);
        int count = Mathf.Min(list1.Count, list2.Count);

        for (int i = 0; i < count; i++)
        {
            if (list1[i].Key != list2[i].Key || list1[i].Value != list2[i].Value)
            {
                differences++;
            }
        }

        return differences;
    }
    public void AddToObjectsList(int index, int objectIndicator)
    {
        CollectedObjects.Add(new KeyValuePair<int, int>(objectIndicator, index));
        Debug.Log("К списку ингредиентов добавлен " + index + "-й объект группы " + objectIndicator);
        if (objectIndicator == 0) rootIndex = index;
    }
    public bool CheckNumberOfObjects()
    {
        if (isGameOver)
        {
            AccessHerbsAndBerriesInteraction(false);

            AccessBowls(1, true);
            AccessBowls(3, false);
            animationsControl.ObjectsOn(rootIndex + 1, 3); // запускаем правильную воду
            animationsControl.ObjectsOn(-1, 0); // убираем корень из миски
            isGameOver = false;
            return false;
        }
        if (CollectedObjects.Count > 3)
        {
            Debug.Log("Ингредиентов 3 штуки");
            return true;
        }
        return false;
    }
    public void AccessHerbsAndBerriesInteraction(bool scriptOn)
    {
        DisableScripts(Berries, scriptOn);
        DisableScripts(Herbs, scriptOn);
    }
    public void AccessRoots(bool scriptOn)
    {
        DisableScripts(Roots, scriptOn);
    }
    public void AccessBowls(int index, bool scriptOn)
    {
        GameObject obj = Bowls[index];
        if (!scriptOn)
        {
            obj.layer = 2;
        }
        else obj.layer = 0;
    }

    // Вспомогательный метод для отключения скриптов у всех объектов в словаре
    private void DisableScripts(Dictionary<int, GameObject> dictionary, bool scriptOn)
    {
        foreach (var pair in dictionary)
        {
            GameObject obj = pair.Value;

            if (!scriptOn)
            {
                obj.layer = 2;
            }
            else obj.layer = 0;
        }
    }
    public void SetCurrentObject(GameObject obj)
    {
        currentObject = obj;
        Debug.Log("CurrentObject is" + currentObject.name);
        neededBowlIndex = animationsControl.GetCorrectBowlIndex(currentObject);
        OutlineObject(neededBowlIndex, true);
    }

    public void NullCurrentObject()
    {
        currentObject = null;
        Debug.Log("CurrentObject is null");
        OutlineObject(neededBowlIndex, false);
    }
    public void OutlineObject(int index, bool enable)
    {
        Interactable interactable = objectsToOutline[index].GetComponent<Interactable>();
        if (interactable != null)
        {
            interactable.OutlineOn(enable);
        }
        else
        {
            Debug.Log("Interactable is null in ControlOneOutline");
        }
    }

    void PopulateResources()
    {
        // Поиск объектов по тегу и добавление их в соответствующие словари
        FindAndAddResources("roots", Roots);
        FindAndAddResources("berries", Berries);
        FindAndAddResources("bowl", Bowls);
        FindAndAddResources("herbs", Herbs);
    }

    void FindAndAddResources(string tag, Dictionary<int, GameObject> dictionary)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in objects)
        {
            int index = obj.GetComponent<Interactable>().GetСomponentIndex();
            if (!dictionary.ContainsKey(index))
            {
                dictionary.Add(index, obj);
            }
            else
            {
                Debug.LogWarning($"Duplicated index {index} for tag {tag}. Object {obj.name} will be ignored.");
            }
        }
    }
}
