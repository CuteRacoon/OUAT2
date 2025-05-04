using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    public GameObject[] hintPanels;
    public GameObject[] lights;
    public Transform[] stayingGirls;

    private CameraBehaviour cameraBehaviour;
    private PlayerController playerController;

    private bool playerInside = false;
    private bool somethingChanging = false;

    private int activeHintPanelIndex = -1;
    private int cameraAndTriggerIndex = -1;

    private Vector3[] initialPanelPositions; // Массив для хранения изначальных позиций панелей
    private bool inProcess = false; // Флаг, показывающий, что процесс перемещения панелей активен

    void Start()
    {
        foreach (GameObject light in lights)
        {
            light.SetActive(true);
        }
        initialPanelPositions = new Vector3[hintPanels.Length];
        for (int i = 0; i < hintPanels.Length; i++)
        {
            initialPanelPositions[i] = hintPanels[i].GetComponent<RectTransform>().anchoredPosition; // Сохраняем позицию RectTransform
            hintPanels[i].SetActive(false);
        }
        cameraBehaviour = FindAnyObjectByType<CameraBehaviour>();
        playerController = FindAnyObjectByType<PlayerController>();
    }
    public void SetActiveTrigger(int index)
    {
        activeHintPanelIndex = index - 1;
        cameraAndTriggerIndex = index;
        somethingChanging = true;
       // playerInside = playerController.GetPlayerInside();
    }
    void Update()
    {
        if (activeHintPanelIndex >= 0) //если есть пересечение с триггером
        {
            playerInside = playerController.GetPlayerInside();
            if (somethingChanging) //пересечение произошло
            {
                hintPanels[activeHintPanelIndex].SetActive(playerInside);
                //lights[activeHintPanelIndex].SetActive(playerInside);
                somethingChanging = false;
            }
            // при выходе из триггерной области, обнуляем пересечение
            if (!playerInside)
            {
                hintPanels[activeHintPanelIndex].SetActive(playerInside);
                //lights[activeHintPanelIndex].SetActive(playerInside);
                SetActiveTrigger(-1);
                return;
            }
            // нажата клавиша взаимодействия
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!inProcess) //камера изначальная?
                {
                    MoveHintPanel(); // переключить камеру, передвинуть подсказку
                    playerController.SetTransform(stayingGirls[activeHintPanelIndex]);
                }
                else // нет
                {
                    ResetInteraction(); // вернуть всё на свои места
                }
            }
        }
    }
    private void MoveHintPanel()
    {
        cameraBehaviour.SwitchCamera(cameraAndTriggerIndex);
        if (activeHintPanelIndex == 1)
        {
            hintPanels[activeHintPanelIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(-700, -230);
        }
        else if (activeHintPanelIndex == 0)
        {
            hintPanels[activeHintPanelIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(850, 230);
        }
        inProcess = true;
        playerController.SetMovement(false);
    }
    private void ResetInteraction()
    {
        playerController.SetMovement(true);
        cameraBehaviour.SwitchCamera(0);
        inProcess = false;
        // Сбрасываем только если activeHintPanelIndex допустимый
        if (activeHintPanelIndex >= 0)
        {
            hintPanels[activeHintPanelIndex].GetComponent<RectTransform>().anchoredPosition = initialPanelPositions[activeHintPanelIndex];
        }
    }
}
