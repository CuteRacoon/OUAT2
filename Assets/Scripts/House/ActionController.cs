using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    private DialogueController dialogueController;
    private CameraBehaviour cameraBehaviour;
    private InteractionController interactionController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueController = FindAnyObjectByType<DialogueController>();
        cameraBehaviour = FindAnyObjectByType<CameraBehaviour>();
        interactionController = FindAnyObjectByType<InteractionController>();
    }
    public void StartBeginningDialogue()
    {
        StartCoroutine(startDialogueNearBake());
    }
    private IEnumerator startDialogueNearBake()
    {
        cameraBehaviour.SwitchCamera(3);
        interactionController.SetPlayerPosition(2);
        interactionController.SetActiveTrigger(-1);
        yield return new WaitForSeconds(3f);
        
        dialogueController.PlayPartOfPlot("beginning");

        while (dialogueController.IsDialoguePlaying)
        {
            yield return null;
        }

        interactionController.ResetInteraction();
        interactionController.DisableTriggerByIndex(2);

        StartCoroutine(showFirstLearningPhrase());
    }
    private IEnumerator showFirstLearningPhrase()
    {
        yield return new WaitForSeconds(1f);
        dialogueController.LearningPanelText("��� ����������� ����������� ������� W, A, S, D ��� ���������");

        // ���, ���� ����� ����� ��������
        bool moved = false;
        while (!moved)
        {
            // ��������� ������� ������ W/A/S/D ��� �������
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                moved = true;
            yield return null;
        }
        // ��������� ��� 0.5 ������� ����� ��������
        yield return new WaitForSeconds(0.5f);
        dialogueController.HideAllPanels();
    }
}
