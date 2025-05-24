using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsManager : MonoBehaviour
{
    [SerializeField] GameObject berriesParent;
    [SerializeField] GameObject rootsParent;
    [SerializeField] GameObject herbsParent;
    [SerializeField] GameObject berriesDustParent;
    [SerializeField] GameObject herbsDustParent;
    [SerializeField] GameObject rootsWaterParent;
    [SerializeField] GameObject endPotionParent;

    public bool isFull = false;

    public Animation mortarAnime;
    private MiniGameLogicManager gameLogic;

    public Collider[] bowlColliders;

    public static AnimationsManager Instance { get; private set; }

    private void Awake()
    {
        gameLogic = FindAnyObjectByType<MiniGameLogicManager>();
        // Синглтон, для надёжности
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        if (bowlColliders == null || bowlColliders.Length != 3)
        {
            Debug.LogError("Необходимо назначить 3 коллайдера мисок в массиве bowlColliders в инспекторе!");
        }
    }
    public void ResetWaters()
    {
        ObjectsOn(-1, 3);
        ObjectsOn(-1, 4);
        GameObject[] objects = GetChildren(endPotionParent);
        objects[0].SetActive(true);
        objects = GetChildren(rootsWaterParent);
        objects[0].SetActive(true);
    }
    private GameObject[] GetChildren(GameObject parent)
    {
        if (parent == null)
        {
            Debug.LogError("Parent GameObject is null!");
            return new GameObject[0]; // Возвращаем пустой массив
        }

        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parent.transform)
        {
            children.Add(child.gameObject);
        }
        return children.ToArray();
    }
    public bool IsNearCorrectBowl(GameObject obj)
    {
        int correctBowlIndex = GetCorrectBowlIndex(obj);

        if (correctBowlIndex == -1)
        {
            return false; // Неизвестный тег объекта
        }

        // Проверяем, пересекает ли объект коллайдер правильной миски
        return IsOverlappingCollider(obj, correctBowlIndex);
    }

    private bool IsOverlappingCollider(GameObject obj, int bowlIndex)
    {
        if (bowlColliders == null || bowlIndex < 0 || bowlIndex >= bowlColliders.Length || bowlColliders[bowlIndex] == null)
        {
            Debug.LogError("Неверный индекс коллайдера или коллайдер не назначен!");
            return false;
        }

        // Используем перекрытие Bounds для более надежной проверки
        return bowlColliders[bowlIndex].bounds.Intersects(obj.GetComponent<Collider>().bounds);
    }
    public int GetCorrectBowlIndex(GameObject obj)
    {
        if (obj.CompareTag("roots"))
        {
            return 0; // Для roots правильная миска с индексом 0
        }
        else if (obj.CompareTag("bowl"))
        {
            return 1; // Для berries правильная миска с индексом 1
        }
        else if (obj.CompareTag("herbs") || obj.CompareTag("berries"))
        {
            return 2; // Для herbs правильная миска с индексом 2
        }
        else
        {
            Debug.LogWarning("Неизвестный тег объекта: " + obj.tag);
            return -1; // Если тег не распознан, возвращаем -1
        }
    }
    public IEnumerator PlayMortarAnimation(int index, int objectIndicator)
    {
        if (mortarAnime != null)
        {
            gameLogic.AccessHerbsAndBerriesInteraction(false);
            Debug.Log("Запускаю анимацию ступки");

            mortarAnime.Play("MortarAnimation");
            yield return new WaitForSeconds(mortarAnime["MortarAnimation"].length);

            gameLogic.AccessBowls(3, true);
            ObjectsDustOn(index, objectIndicator);
        }
        else Debug.Log("Animation нет на ступке"); ;
    }
    public void ObjectsDustOn(int index, int objectIndicator)
    {
        ObjectsOn(-1, 1);
        ObjectsOn(-1, 2);
        // Получаем массив из parent-объекта
        GameObject[] objects = null;
        switch (objectIndicator)
        {
            case 1:
                objects = GetChildren(berriesDustParent);
                break;
            case 2:
                objects = GetChildren(herbsDustParent);
                break;
        }
        if (objects != null)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(false);
            }
            if (index > 0)
            {
                objects[index - 1].SetActive(true);
                isFull = true;
            }
        }
    }
    public void CleanDust()
    {
        ObjectsDustOn(-1, 1);
        ObjectsDustOn(-1, 2);
        isFull = false;
    }

    public void ObjectsOn(int index, int objectIndicator)
    {
        GameObject[] objects = null;
        switch (objectIndicator)
        {
            case 0:
                objects = GetChildren(rootsParent);
                break;
            case 1:
                objects = GetChildren(berriesParent);
                break;
            case 2:
                objects = GetChildren(herbsParent);
                break;
            case 3:
                objects = GetChildren(rootsWaterParent);
                objects[0].SetActive(false);
                Debug.Log("Изменяется вода в миске");
                break;
            case 4:
                objects = GetChildren(endPotionParent);
                objects[0].SetActive(false);
                break;
        }
        if (objects != null)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(false);
            }
            if (index > 0)
            {
                objects[index - 1].SetActive(true);
            }
        }
    }
}
