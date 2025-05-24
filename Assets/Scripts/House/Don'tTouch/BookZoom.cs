using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BookZoom : MonoBehaviour
{
    private Camera mainCamera;
    public RectTransform bookRectTransform;
    public float zoomStep = 8f;
    public float minFOV = 35f;
    // Для ограничения скорости
    public float scrollCooldown = 0.04f;
    public float ratio = 6f;

    private CameraManager cameraBehaviour;
    private float initialFOV;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float targetFOV;
    private Vector3 targetPosition;

    private bool isZoomed = false;


    private float zoomOffsetStrength = 0.015f;
    private float lastScrollTime = 0f;

    void Start()
    {
        cameraBehaviour = FindAnyObjectByType<CameraManager>();
        if (mainCamera == null)
            mainCamera = cameraBehaviour.GetCameraByIndex(2);

        initialFOV = mainCamera.fieldOfView;
        initialPosition = mainCamera.transform.position;
        initialRotation = mainCamera.transform.rotation;

        targetFOV = initialFOV;
        targetPosition = initialPosition;
    }

    void Update()
    {
        if (!mainCamera.isActiveAndEnabled) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float currentTime = Time.time;

        if (IsMouseOverUIElement(bookRectTransform) && Mathf.Abs(scroll) > 0.01f && currentTime - lastScrollTime > scrollCooldown)
        {
            lastScrollTime = currentTime;

            // === ПРИБЛИЖЕНИЕ ===
            if (scroll > 0f && targetFOV > minFOV)
            {
                if (!isZoomed)
                {
                    isZoomed = true;
                }

                targetFOV = Mathf.Max(minFOV, targetFOV - zoomStep);

                // Смещение к курсору
                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                Vector2 mousePosition = Input.mousePosition;
                Vector2 offsetFromCenter = (mousePosition - screenCenter) * zoomOffsetStrength;

                float depth = 10f;
                Vector3 screenPointCenter = new Vector3(screenCenter.x, screenCenter.y, depth);
                Vector3 screenPointOffset = new Vector3(screenCenter.x + offsetFromCenter.x, screenCenter.y + offsetFromCenter.y, depth);

                Vector3 worldOffset = mainCamera.ScreenToWorldPoint(screenPointOffset) - mainCamera.ScreenToWorldPoint(screenPointCenter);
                targetPosition += worldOffset;
            }

            // === ОТДАЛЕНИЕ ===
            else if (scroll < 0f)
            {
                targetFOV = Mathf.Min(initialFOV, targetFOV + zoomStep);

                if (targetFOV >= initialFOV)
                {
                    targetPosition = initialPosition;
                    mainCamera.transform.rotation = initialRotation;
                    isZoomed = false;
                }
            }
        }

        // === ПЛАВНОЕ ПРИБЛИЖЕНИЕ/ОТДАЛЕНИЕ ===
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * ratio);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * ratio);
    }

    bool IsMouseOverUIElement(RectTransform rectTransform)
    {
        Vector2 localMousePosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            Input.mousePosition,
            null, // null для Screen Space Overlay
            out localMousePosition);

        return rectTransform.rect.Contains(localMousePosition);
    }
}
