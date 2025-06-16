using UnityEngine;
using System.Collections;
using System;

public class LampScript : MonoBehaviour
{
    [Header("��������� �����")]
    public bool CanUseLamp = true;              // ����� �� ��������� ������
    public GameObject lampObject;               // ���� ���������� ������ � ������ � ����������
    private Light lampLight;                   // ��� ��������� Light
    private Material lampMaterial;

    public float fadeDuration = 3f;           // ����� ���������/���������
    public float maxIntensity = 1f;           // ������������ �������
    private Color initialEmissionColor;

    private bool isLampOn = false;
    public bool IsLampOn => isLampOn;
    private Coroutine fadeCoroutine;
    void Start()
    {
        if (lampObject != null)
        {
            lampLight = lampObject.GetComponentInChildren<Light>();

            Renderer rend = lampObject.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                lampMaterial = rend.material;
                lampMaterial.EnableKeyword("_EMISSION");

                // ��������� �������� ����
                initialEmissionColor = lampMaterial.GetColor("_EmissionColor");

                // ��������� ��������� � ����� ���������
                lampMaterial.SetColor("_EmissionColor", Color.black);
            }

            if (lampLight != null)
            {
                lampLight.intensity = 0f;
                lampObject.SetActive(false);
            }
        }
        CanUseLamp = false;
    }
    public void ResetIntensity()
    {
        lampLight.intensity = maxIntensity;
    }
    public void OnFuelCritical()
    {
        // ������� �� ����������� ������� �������
        if (lampLight != null)
        {
            lampLight.intensity = maxIntensity * 0.3f;
        }
    }

    void Update()
    {
        if (CanUseLamp && Input.GetKeyDown(KeyCode.F))
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            StartCoroutine(ToggleLamp());
        }
    }
    public void SetLampState(bool state)
    {
        if (isLampOn == state)
            return;
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        StartCoroutine(ToggleLamp());
    }

    IEnumerator ToggleLamp()
    {
        if (lampObject == null || lampLight == null || lampMaterial == null)
        {
            Debug.LogWarning("LampController: �� ��� ���������� ���������.");
            yield break;
        }
        isLampOn = !isLampOn;
        if (isLampOn)
        {
            GameEvents.RaiseLampStateChanging();
        }

        float startIntensity = lampLight.intensity;
        float endIntensity = isLampOn ? maxIntensity : 0f;

        Color startEmission = lampMaterial.GetColor("_EmissionColor");
        Color endEmission = isLampOn ? initialEmissionColor : Color.black;

        float timer = 0f;

        if (isLampOn)
        {
            lampObject.SetActive(true); // �������� �����
        }

        // ������� ��������� �������
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);
            lampLight.intensity = Mathf.Lerp(startIntensity, endIntensity, t);
            lampMaterial.SetColor("_EmissionColor", Color.Lerp(startEmission, endEmission, t));
            yield return null;
        }

        lampLight.intensity = endIntensity;
        lampMaterial.SetColor("_EmissionColor", endEmission);

        // ���� ��������� � �������� � ����������� �������
        if (!isLampOn)
        {
            lampObject.SetActive(false);
            GameEvents.RaiseLampStateChanging();
        }
    }
}
