using UnityEngine;

public class CandleLightFlicker : MonoBehaviour
{
    // Настройки в инспекторе
    [Header("Настройки свечи")]
    public float averageIntensity = 1f; // Средняя интенсивность света
    public float flickerSpeed = 0.1f; // Скорость мерцания
    public float intensityRandomness = 0.2f; // Рандомность интенсивности
    public Light candleLight;
    
    private float timer;

    void Start()
    {
        // Получаем компонент Light
        candleLight = GetComponent<Light>();
        if (candleLight == null || candleLight.type != LightType.Point)
        {
            Debug.LogError("Убедитесь, что у объекта есть компонент Light типа Point Light.");
            this.enabled = false; // Отключаем скрипт, если нет компонента Light
        }
    }

    void Update()
    {
        // Обновляем таймер
        timer += Time.deltaTime * flickerSpeed;

        // Генерируем случайное значение для интенсивности
        float flickerIntensity = averageIntensity + Random.Range(-intensityRandomness, intensityRandomness);

        // Устанавливаем новую интенсивность света
        candleLight.intensity = Mathf.Clamp(flickerIntensity, 0, averageIntensity * 2); // Ограничиваем интенсивность
    }
}