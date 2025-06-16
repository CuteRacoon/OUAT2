using UnityEditor;
using UnityEngine;
using System.Collections;

public class HideController : MonoBehaviour
{
    private Canvas canvas;
    private HideTrigger trigger;
    private ForestActionController action;

    private bool isHidden = false;
    private int currentTriggerIndex = -1;

    private bool playerInsideTrigger = false;

    private void OnEnable()
    {
        HideTrigger.OnHideTriggerEnter += OnPlayerEnterHideZone;
        HideTrigger.OnHideTriggerExit += OnPlayerExitHideZone;
    }
    private void Start()
    {
        action = FindAnyObjectByType<ForestActionController>();
    }
    private void OnPlayerEnterHideZone(HideTrigger currentTrigger)
    {
        trigger = currentTrigger;
        currentTriggerIndex = trigger.index;
        playerInsideTrigger = true;

        canvas = trigger.GetComponentInChildren<Canvas>(true);
        canvas.gameObject.SetActive(true);
    }
    private void Update()
    {
        if (playerInsideTrigger && Input.GetKeyDown(KeyCode.E) && trigger != null)
        {
            if (!isHidden)
            {
                isHidden = true;
                action.HidePerson();
                canvas.gameObject.SetActive(false);
                StartCoroutine(StartHidePerson());
            }
            else
            {
                isHidden = false;
                action.ShowPerson();
                canvas.gameObject.SetActive(true);
            }
        }
    }
    private IEnumerator StartHidePerson()
    {
        int actualIndex = currentTriggerIndex * 2;
        ForestCameraManager.Instance.SwitchToHidingCamera(actualIndex);
        Animation anime = ForestCameraManager.Instance.GetCurrentCamera().GetComponent<Animation>();
        anime.enabled = true; // запускаем анимацию
        anime.Play("HideAnimation");

        // ∆дЄм, пока длительность клипа не пройдЄт
        yield return new WaitForSeconds(4.1f);
        actualIndex = currentTriggerIndex * 2 + 1;
        ForestCameraManager.Instance.SwitchToHidingCamera(actualIndex);
    }
    private void OnPlayerExitHideZone(HideTrigger currentTrigger)
    {
        if (canvas.gameObject.activeSelf)
        {
            canvas.gameObject.SetActive(false);
        }
        trigger = null;
        playerInsideTrigger = false;
        currentTriggerIndex = -1;
    }
}
