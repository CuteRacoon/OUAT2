using UnityEngine;
using UnityEngine.UI;

public class CandleFlickerUI : MonoBehaviour
{
    [SerializeField] private Image overlayImage;
    [SerializeField] private float minAlpha = 0.1f;
    [SerializeField] private float maxAlpha = 0.3f;
    [SerializeField] private float speed = 5f;

    private void Update()
    {
        if (overlayImage != null)
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PerlinNoise(Time.unscaledTime * speed, 0f));
            Color c = overlayImage.color;
            c.a = alpha;
            overlayImage.color = c;
        }
    }
}
