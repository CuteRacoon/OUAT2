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
        StartCoroutine(startDialogue());
    }
    private IEnumerator startDialogue()
    {
        cameraBehaviour.SwitchCamera(3);
        interactionController.SetPlayerPosition(2);
        yield return new WaitForSeconds(1f);
        
        dialogueController.PlayPartOfPlot("beginning");

        while (dialogueController.IsDialoguePlaying)
        {
            yield return null;
        }

        interactionController.ResetInteraction();
        interactionController.DisableTriggerByIndex(2);
    }
}
