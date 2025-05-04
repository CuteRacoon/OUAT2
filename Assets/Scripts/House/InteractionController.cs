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

    private Vector3[] initialPanelPositions; // ������ ��� �������� ����������� ������� �������
    private bool inProcess = false; // ����, ������������, ��� ������� ����������� ������� �������

    void Start()
    {
        foreach (GameObject light in lights)
        {
            light.SetActive(true);
        }
        initialPanelPositions = new Vector3[hintPanels.Length];
        for (int i = 0; i < hintPanels.Length; i++)
        {
            initialPanelPositions[i] = hintPanels[i].GetComponent<RectTransform>().anchoredPosition; // ��������� ������� RectTransform
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
        if (activeHintPanelIndex >= 0) //���� ���� ����������� � ���������
        {
            playerInside = playerController.GetPlayerInside();
            if (somethingChanging) //����������� ���������
            {
                hintPanels[activeHintPanelIndex].SetActive(playerInside);
                //lights[activeHintPanelIndex].SetActive(playerInside);
                somethingChanging = false;
            }
            // ��� ������ �� ���������� �������, �������� �����������
            if (!playerInside)
            {
                hintPanels[activeHintPanelIndex].SetActive(playerInside);
                //lights[activeHintPanelIndex].SetActive(playerInside);
                SetActiveTrigger(-1);
                return;
            }
            // ������ ������� ��������������
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!inProcess) //������ �����������?
                {
                    MoveHintPanel(); // ����������� ������, ����������� ���������
                    playerController.SetTransform(stayingGirls[activeHintPanelIndex]);
                }
                else // ���
                {
                    ResetInteraction(); // ������� �� �� ���� �����
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
        // ���������� ������ ���� activeHintPanelIndex ����������
        if (activeHintPanelIndex >= 0)
        {
            hintPanels[activeHintPanelIndex].GetComponent<RectTransform>().anchoredPosition = initialPanelPositions[activeHintPanelIndex];
        }
    }
}
