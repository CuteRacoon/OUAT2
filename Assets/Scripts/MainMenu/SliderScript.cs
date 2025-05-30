using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderPercentScript : MonoBehaviour
{
    private Slider slider;         // Слайдер
    [SerializeField] private TMP_Text numberText;      // TextMeshPro Text (если используется)

    private void Start()
    {
        slider = GetComponent<Slider>();
        if (slider != null)
        {
            slider.onValueChanged.AddListener(UpdateDisplay);
            UpdateDisplay(slider.value);
        }
    }

    private void UpdateDisplay(float value)
    {
        int percent = Mathf.RoundToInt(value * 100f);
        if (numberText != null)
            numberText.text = percent.ToString();
    }
}
