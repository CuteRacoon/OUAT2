using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ForestActionController : MonoBehaviour
{
    private DialogueController dialogueController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueController = FindAnyObjectByType<DialogueController>();
        StartCoroutine(DialogueCoroutine());
    }

    private IEnumerator DialogueCoroutine()
    {
        yield return new WaitForSeconds(1f);
        dialogueController.PlayPartOfPlot("main");
    }
}
