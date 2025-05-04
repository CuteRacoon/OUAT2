using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public GameObject[] objectsToOutline;

    private static GameObject currentObject;
    private AnimationsControl animationsControl;
    private int rootIndex;
    private int neededBowlIndex = -1;
    public bool isGameOver = false;
    private DialogueController dialogueController;

    // Словари для хранения объектов по тегам и индексам
    public Dictionary<int, GameObject> Roots { get; private set; } = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> Berries { get; private set; } = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> Bowls { get; private set; } = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> Herbs { get; private set; } = new Dictionary<int, GameObject>();

    public List<KeyValuePair<int, int>> CollectedObjects { get; private set; } = new List<KeyValuePair<int, int>>();

    private void Start()
    {
        dialogueController = FindAnyObjectByType<DialogueController>();
        animationsControl = FindAnyObjectByType<AnimationsControl>();
        currentObject = null;
        PopulateResources();

        AccessHerbsAndBerriesInteraction(false);
        AccessBowls(2, false);
        AccessBowls(1, false);
        AccessBowls(3, false);
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
            new KeyValuePair<int, int>(1, 1),
            new KeyValuePair<int, int>(2, 1),
            new KeyValuePair<int, int>(2, 3)
        };

        SortList(CollectedObjects);
        SortList(expectedObjects);

        // Считаем количество несовпадений
        int differences = CountDifferences(CollectedObjects, expectedObjects);
        if (differences > 0 && differences <= 2)
        {
            dialogueController.EndGame(1);
            animationsControl.ObjectsOn(2, 4);
        }
        else if (differences > 2)
        {
            dialogueController.EndGame(2);
            animationsControl.ObjectsOn(3, 4);
        }
        else if (differences == 0 && CollectedObjects.Count == expectedObjects.Count)
        {
            dialogueController.EndGame(3);
            animationsControl.ObjectsOn(4, 4);
        }
    }
    public void SortList(List<KeyValuePair<int, int>> list)
    {
        list.Sort((x, y) => x.Key.CompareTo(y.Key));
    }
    private int CountDifferences(List<KeyValuePair<int, int>> list1, List<KeyValuePair<int, int>> list2)
    {
        int differences = 0;
        if (list1.Count != list2.Count)
        {
            differences = Mathf.Abs(list1.Count - list2.Count); //если количество элементов разное

            //return differences;
        }
        //сравниваем количество элементов, равное минимальному из двух списков
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
