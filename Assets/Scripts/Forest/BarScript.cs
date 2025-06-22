using UnityEngine;
using UnityEngine.UI;
using System;
public enum BarMode
{
    Manual,
    ShiftControlled
}
public class BarScript : MonoBehaviour
{
    [Header("UI Settings")]
    private Image barImage;

    [Header("Timing Settings")]
    public float duration = 5f; // Время в секундах, за которое шкала должна исчезнуть
    public float regenerationDuration = 3f; // Время полного восстановления

    private float startFillAmount = 0.6f;
    private bool isFirstManualRun = true;


    public float criticalLevelAmount = 0.4f;
    [Header("Режим работы")]
    public BarMode mode = BarMode.Manual;


    private float timer = 0f;
    private float regenTimer = 0f;

    private bool isRunning = true;
    private bool isCriticalLevel = false;
    private bool isFailure = false;
    private bool shiftHeld = false;
    private bool isRegenerating = false;
    private bool shiftBlocked = false;
   /* public bool IsFailure => isFailure;*/
    public bool IsCriticalLevel => isCriticalLevel;
    public bool CanUseShift => !shiftBlocked;
    private GameObject parentObj;

    private Color normalColor;
    public Color criticalColor = new Color(0xF2 / 255f, 0xC9 / 255f, 0x89 / 255f); // F2C989

    
    private void Awake()
    {
        Image[] allImages = GetComponentsInChildren<Image>(includeInactive: true);
        foreach (var img in allImages)
        {
            if (img.gameObject != this.gameObject)
            {
                barImage = img;
                break;
            }
        }

        if (barImage == null)
        {
            Debug.LogError("BarScript не нашел компонент Image внутри дочерних объектов!");
        }
        normalColor = barImage.color;
        parentObj = transform.parent != null ? transform.parent.gameObject : null;
        shiftBlocked = true;
    }
    public void SetShiftBlocked(bool state)
    {
        shiftBlocked = state;
    }
    public void SetShiftHeld(bool state)
    {
        if (mode != BarMode.ShiftControlled) return;

        shiftHeld = state;

        if (shiftHeld)
        {
            isRunning = true;
            isRegenerating = false;
            timer = (1f - barImage.fillAmount) * duration;
        }
        else if (!shiftHeld && barImage.fillAmount < 1f)
        {
            StartRegeneration();
        }
    }
    public void EnableBar()
    {
        if (parentObj != null)
            parentObj.SetActive(true);
        ResumeBar();
    }
    public void DisableBar()
    {
        if (parentObj != null)
            parentObj.SetActive(false);
        StopBar();
    }

    private void Update()
    {
        if (barImage == null || duration <= 0f) return;

        switch (mode)
        {
            case BarMode.Manual:
                ManualModeUpdate();
                break;

            case BarMode.ShiftControlled:
                ShiftModeUpdate();
                break;
        }
    }
    private void ManualModeUpdate()
    {
        if (!isRunning) return;

        timer += Time.deltaTime;
        float fill = Mathf.Clamp01(startFillAmount - (timer / duration));
        barImage.fillAmount = fill;

        if (fill <= criticalLevelAmount && !isCriticalLevel)
        {
            barImage.color = criticalColor;
            isCriticalLevel = true;
        }

        if (fill <= 0f)
        {
            barImage.fillAmount = 0f;
            isFailure = true;
            DisableBar();
            GameEvents.RaiseBarIsNull();
        }
    }
    private void ShiftModeUpdate()
    {
        if (shiftHeld && !shiftBlocked && isRunning)
        {
            timer += Time.deltaTime;
            float fill = Mathf.Clamp01(1f - (timer / duration));
            barImage.fillAmount = fill;

            if (fill <= criticalLevelAmount && !isCriticalLevel)
            {
                barImage.color = criticalColor;
                isCriticalLevel = true;

            }

            if (fill <= 0.02f)
            {
                barImage.fillAmount = 0f;
                shiftBlocked = true;
                isRunning = false;
                isFailure = true;
                StartRegeneration();
                GameEvents.RaiseNeedToStopSprint();
            }
        }
        else if (isRegenerating)
        {
            regenTimer += Time.deltaTime;
            float fill = Mathf.Clamp01(regenTimer / regenerationDuration);
            barImage.fillAmount = fill;

            if (fill >= 1f)
            {
                CompleteRegeneration();
            }
            else if (fill > criticalLevelAmount && shiftBlocked)
            {
                shiftBlocked = false;
                GameEvents.RaiseNeedToStartSprint();
            }

            if (fill > criticalLevelAmount)
            {
                barImage.color = normalColor;
                isCriticalLevel = false;
            }
        }
    }
    private void StartRegeneration()
    {
        regenTimer = barImage.fillAmount * regenerationDuration;
        isRegenerating = true;
        isRunning = false;
        isFailure = false;
    }

    private void CompleteRegeneration()
    {
        regenTimer = 0f;
        isRegenerating = false;
        isRunning = false;
        barImage.fillAmount = 1f;
        barImage.color = normalColor;
        isCriticalLevel = false;
        shiftBlocked = false;
        timer = 0f;
    }


    public void ResetBar()
    {
        EnableBar();
        startFillAmount = 1f;
        timer = 0f;
        regenTimer = 0f;
        isFailure = false;
        isRunning = true;
        isRegenerating = false;
        shiftBlocked = false;

        if (barImage != null)
        {
            barImage.fillAmount = 1f;
            barImage.color = normalColor;
        }
        isCriticalLevel = false;
    }
    public void StopBar()
    {
        isRunning = false;
    }
    public void ResumeBar()
    {
        isRunning = true;
    }
}

