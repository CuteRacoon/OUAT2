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

    private int currentTriggerIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueManager = FindAnyObjectByType<DialogueManager>();
        lampController = FindAnyObjectByType<LampController>();
        playerController = FindAnyObjectByType<PlayerController>();

        StartCoroutine(DialogueCoroutine());
        MonsterTrigger.OnMonsterTriggerEnter += HandlePlayerEnterTrigger;
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
    private void HandlePlayerEnterTrigger(MonsterTrigger trigger)
    {
        currentTriggerIndex = trigger.index;
        StartCoroutine(GirlThoughts3());
    }
    IEnumerator GirlThoughts3()
    {
        if (currentTriggerIndex == 1)
        {
            yield return new WaitForSeconds(5f);
            dialogueManager.PlayPartOfPlot("girl_thoughts_4");
        }
        if (currentTriggerIndex == 0)
        {
            yield return new WaitForSeconds(3f);
            dialogueManager.PlayPartOfPlot("girl_thoughts_3");
        }
    }
    private IEnumerator DialogueCoroutine()
    {
        GameEvents.RaiseCannotDisplayLampBar();
        yield return new WaitForSeconds(2f);
        dialogueManager.PlayPartOfPlot("girl_thoughts_1");
        while (dialogueManager.IsDialoguePlaying)
        {
            yield return null;
        }
        StartCoroutine(StartLampLearning());

        // при билде убрать, раскомментить обучение
        /*GameEvents.RaiseCannotDisplayLampBar();
        yield return new WaitForSeconds(0.5f);
        lampController.StateCanUseLamp(true);
        lampController.LearningCompleted();*/

    }
    public void HidePerson()
    {
        lampState = lampController.IsLampOn;

        playerController.SetPlayerControl(false, true);
        playerController.SetMovement(false);
        lampController.DisableLampBar();
        lampController.StateCanUseLamp(false);
        lampController.SetLampState(false); // выключить, если включена
    }
    public void ShowPerson()
    {
        ForestCameraManager.Instance.SwitchToPlayerCamera();
        lampController.StateCanUseLamp(true);
        playerController.SetPlayerControl(true, false);
        playerController.SetMovement(true);
        lampController.SetLampState(lampState); //вернуть лампу в изначальное положение
    }
    private IEnumerator StartLampLearning()
    {
        playerController.SetPlayerControl(false, true);

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

        playerController.SetPlayerControl(true, false);
        yield return new WaitForSeconds(3f);
        dialogueManager.PlayPartOfPlot("girl_thoughts_2");
    }
}
