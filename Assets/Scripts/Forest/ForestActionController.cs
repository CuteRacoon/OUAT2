using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ForestActionController : MonoBehaviour
{
    private DialogueManager dialogueManager;
    private LampController lampController;
    private PlayerController playerController;

    private bool lampState;
    public static ForestActionController Instance { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueManager = FindAnyObjectByType<DialogueManager>();
        lampController = FindAnyObjectByType<LampController>();
        playerController = FindAnyObjectByType<PlayerController>();

        StartCoroutine(DialogueCoroutine());
    }
    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private IEnumerator DialogueCoroutine()
    {
        /*GameEvents.RaiseCannotDisplayLampBar();
        yield return new WaitForSeconds(4f);
        dialogueManager.PlayPartOfPlot("girl_thoughts");
        while (dialogueManager.IsDialoguePlaying)
        {
            yield return null;
        }
        StartCoroutine(StartLampLearning());*/

        // при билде убрать, раскомментить обучение
        GameEvents.RaiseCannotDisplayLampBar();
        yield return new WaitForSeconds(0.5f);
        lampController.StateCanUseLamp(true);
        lampController.LearningCompleted();

    }
    public void HidePerson()
    {
        lampState = lampController.IsLampOn;

        playerController.SetPlayerControl(false);
        playerController.SetMovement(false);
        lampController.DisableLampBar();
        lampController.StateCanUseLamp(false);
        lampController.SetLampState(false); // выключить, если включена
    }
    public void ShowPerson()
    {
        ForestCameraManager.Instance.SwitchToPlayerCamera();
        lampController.StateCanUseLamp(true);
        playerController.SetPlayerControl(true);
        playerController.SetMovement(true);
        lampController.SetLampState(lampState); //вернуть лампу в изначальное положение
    }
    private IEnumerator StartLampLearning()
    {
        playerController.SetPlayerControl(false);

        dialogueManager.LearningPanelText("Для того, чтобы зажечь лампу, нажмите F");
        lampController.StateCanUseLamp(true);
        yield return null;

        while (!lampController.IsLampOn)
        {
            yield return null;
        }
        lampController.StateCanUseLamp(false);

        // Подождать ещё 0.5 секунды перед скрытием
        yield return new WaitForSeconds(0.5f);
        dialogueManager.HideAllPanels();

        yield return new WaitForSeconds(1f);
        dialogueManager.LearningPanelText("Однако топливо в лампе не бесконечно. Шкала топлива подскажет");
        GameEvents.RaiseCanDisplayLampBar();

        yield return new WaitForSeconds(0.1f);

        lampController.StopLampBar();

        yield return new WaitForSeconds(5f);
        dialogueManager.HideAllPanels();

        lampController.StateCanUseLamp(true);

        lampController.ResumeLampBar();
        lampController.LearningCompleted();

        playerController.SetPlayerControl(true);
    }
}
