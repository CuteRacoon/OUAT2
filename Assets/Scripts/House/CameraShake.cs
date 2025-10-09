using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraShake : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Максимальное смещение в метрах при интенсивности = 1")]
    public float positionMultiplier = 0.5f;
    [Tooltip("Максимальный угол поворота в градусах при интенсивности = 1")]
    public float rotationMultiplier = 1.5f;
    [Tooltip("Частота изменения шума (чем выше — тем резче)")]
    public float noiseFrequency = 20f;
    [Tooltip("Гладкость возвращения в исходное положение")]
    [Range(0f, 1f)]
    public float returnSmooth = 0.9f;

    // внутреннее состояние
    Vector3 initialLocalPos;
    Quaternion initialLocalRot;
    Coroutine shakeCoroutine;

    void Awake()
    {
        initialLocalPos = transform.localPosition;
        initialLocalRot = transform.localRotation;
    }

    /// <summary>
    /// Запустить тряску.
    /// intensity: 0 — рекомендуемые 0.1..3
    /// duration: секунды
    /// </summary>
    public void Shake(float intensity, float duration)
    {
        // если уже трясётся — прервём и запустим заново (можно поменять логику)
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float elapsed = 0f;

        // используем псевдо-рандом и Perlin для плавности
        float seedX = Random.Range(-1000f, 1000f);
        float seedY = Random.Range(-1000f, 1000f);
        float seedZ = Random.Range(-1000f, 1000f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // нормированное прогрессирование 0..1
            float t = Mathf.Clamp01(elapsed / duration);

            // затухание эффекта (можно заменить кривой)
            float attenuation = 1f - t; // простое линейное затухание
            // более мягкое:
            attenuation = Mathf.SmoothStep(attenuation, 0f, 0f);

            float amp = intensity * attenuation;

            // PerlinNoise возвращает 0..1, переводим в -1..1
            float nx = (Mathf.PerlinNoise(seedX, Time.time * noiseFrequency) - 0.5f) * 2f;
            float ny = (Mathf.PerlinNoise(seedY, Time.time * noiseFrequency) - 0.5f) * 2f;
            float nz = (Mathf.PerlinNoise(seedZ, Time.time * noiseFrequency) - 0.5f) * 2f;

            Vector3 posOffset = new Vector3(nx, ny, nz) * positionMultiplier * amp;
            // Для вращения используем меньшую амплитуду; вращение вокруг локальных осей
            Vector3 rotOffsetEuler = new Vector3(ny, nx, nz) * rotationMultiplier * amp;

            // Применяем локально
            transform.localPosition = initialLocalPos + posOffset;
            transform.localRotation = Quaternion.Euler(initialLocalRot.eulerAngles + rotOffsetEuler);

            yield return null;
        }

        // Возврат в изначальное состояние (плавно)
        float smoothT = 0f;
        float smoothDur = 0.12f + (1f - returnSmooth) * 0.4f; // короткая плавность, можно настраивать
        Vector3 fromPos = transform.localPosition;
        Quaternion fromRot = transform.localRotation;

        while (smoothT < 1f)
        {
            smoothT += Time.deltaTime / smoothDur;
            float s = Mathf.SmoothStep(0f, 1f, smoothT);
            transform.localPosition = Vector3.Lerp(fromPos, initialLocalPos, s);
            transform.localRotation = Quaternion.Slerp(fromRot, initialLocalRot, s);
            yield return null;
        }

        transform.localPosition = initialLocalPos;
        transform.localRotation = initialLocalRot;
        shakeCoroutine = null;
    }
}
