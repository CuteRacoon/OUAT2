using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunAwayController : MonoBehaviour
{
    [SerializeField] private BarScript barScript;
    private bool lastShiftHeld = false;
    private Camera followCamera;

    private void OnEnable()
    {
        RunAwayTrigger.OnRunAwayTriggerEnter += HandleFailure;
        GooseTrigger.OnGooseTriggerEnter += HandleStartChase;
    }
    void HandleFailure(RunAwayTrigger trigger)
    {
        Animation anime = followCamera.GetComponent<Animation>();
        anime.Play("RunAwayCameraAnimation");
        barScript.DisableBar();
        LampController.Instance.DisableLampBar();
        StartCoroutine(BackToMenuCoroutine());
       // StartCoroutine(FailureCoroutine());
        //PlayerController.Instance.SetPlayerControl(false);
    }
    /*private IEnumerator FailureCoroutine()
    {
        MonsterSceneController.Instance.StartRunning();
        yield return new WaitForSeconds(1.5f);
        MonsterSceneController.Instance.StopRunning();
    }*/
    IEnumerator BackToMenuCoroutine()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Menu");
    }
    void HandleStartChase(GooseTrigger trigger)
    {
        StartChase();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        barScript.DisableBar();
    }
    void Update()
    {
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift);

        // Вызываем SetShiftHeld только при изменении состояния Shift
        if (shiftPressed != lastShiftHeld)
        {
            if (barScript.CanUseShift || !shiftPressed) // Разрешить отпускание даже если shift заблокирован
            {
                barScript.SetShiftHeld(shiftPressed);
            }

            lastShiftHeld = shiftPressed;
        }
    }
    private void StartChase()
    {
        ForestCameraManager.Instance.SwitchToRunAwayCamera();
        followCamera = ForestCameraManager.Instance.GetCurrentCamera();
        followCamera.GetComponent<RunAwayCameraFollow>().SetChaseMode(true);
        GameEvents.RaiseStartChasing();
        // followCamera.GetComponent<RunAwayCameraFollow>().SetChaseSpeed(7f);
        PlayerController.Instance.SetNewMovementSpeeds(4f, 4f, 5.5f);
        barScript.SetShiftBlocked(false);
        barScript.EnableBar();
    }
}
