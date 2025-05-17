using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Interactable : MonoBehaviour
{
    protected float pickupHeight = 0.5f;
    protected LayerMask tableLayer;
    public int index = -1;

    public bool highlightOnHover = true; // Переключатель для включения/выключения подсветки при наведении
    public bool manualHighlightEnabled = false; // Флаг для ручного включения подсветки

    protected Rigidbody rb;
    protected Camera tableCamera;

    protected bool isHoldingObject = false;
    protected bool isMouseOver = false;
    protected bool isReturning = false;

    protected float initialYOffset;
    protected float returnSpeed = 4f;

    protected Vector3 initialPosition;
    protected Vector3 initialScale;
    protected Vector3 initialRotation;
    protected Vector3 objectWorldPosition;

    protected Outline outlineComponent;
    protected CameraBehaviour cameraBehaviour;
    protected OutlineSettings outlineSettings;
    protected InteractionController interactionController;
    protected GameLogic gameLogic;

    protected virtual void Start()
    {
        tableLayer = LayerMask.GetMask("Table");
        rb = GetComponent<Rigidbody>();
        gameLogic = FindAnyObjectByType<GameLogic>();
        cameraBehaviour = FindAnyObjectByType<CameraBehaviour>();
        interactionController = FindAnyObjectByType<InteractionController>();

        if (rb == null)
        {
            Debug.LogError("Объект должен иметь компонент Rigidbody!");
            enabled = false;
            return;
        }
        //tableCamera = Camera.main;
        tableCamera = cameraBehaviour.GetCameraByIndex(1);
        rb.useGravity = true;
        rb.isKinematic = false;

        outlineComponent = GetComponent<Outline>();
        if (outlineComponent == null)
        {
            Debug.LogWarning("Компонент 'Outline' не найден на объекте");
        }
        else
        {
            UpdateOutlineState(); // Инициализируем состояние Outline в Start
        }

        outlineSettings = FindAnyObjectByType<OutlineSettings>();
        initialPosition = transform.position;
        initialScale = transform.localScale;
        initialRotation = transform.rotation.eulerAngles;

        if (this.index == -1) Debug.Log("Не назначен индекс объекта");
    }
    public int GetСomponentIndex()
    {
        return index;
    }
    protected virtual void Update()
    {
        // Всегда проверяем на наведение мыши, чтобы isMouseOver был актуальным
        HandleMouseInteraction();

        if (Input.GetMouseButtonDown(0) && isMouseOver)
        {
            PickupObject();
            gameLogic.SetCurrentObject(this.GameObject());
        }

        if (Input.GetMouseButtonUp(0) && isHoldingObject)
        {
            StartCoroutine(HandleObjectRelease());
        }

        if (isHoldingObject)
        {
            MoveObject();
        }

        if (isReturning && gameObject.activeSelf)
        {
            ReturnToInitialPosition();
        }
    }

    protected virtual IEnumerator HandleObjectRelease()
    {
        DropObject();
        yield return null;
    }

    protected virtual void PickupObject()
    {
        isHoldingObject = true;
        rb.useGravity = false;
        if (!rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        rb.isKinematic = true;

        Ray ray = tableCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tableLayer))
        {
            initialYOffset = transform.position.y - hit.point.y;
            objectWorldPosition = hit.point;
        }
        else
        {
            objectWorldPosition = transform.position;
            initialYOffset = 0;
            objectWorldPosition.y -= pickupHeight;
        }
    }

    protected virtual void DropObject()
    {
        isHoldingObject = false;
        rb.useGravity = true;
        rb.isKinematic = false;
        isReturning = true;
        gameLogic.NullCurrentObject();
    }

    protected virtual void MoveObject()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Vector3.Distance(transform.position, tableCamera.transform.position);
        Vector3 worldPosition = tableCamera.ScreenToWorldPoint(mousePosition);

        Ray ray = tableCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tableLayer))
        {
            objectWorldPosition = hit.point;
        }

        Vector3 targetPosition = new Vector3(objectWorldPosition.x, objectWorldPosition.y + pickupHeight + initialYOffset, objectWorldPosition.z);
        transform.position = targetPosition;
    }

    protected virtual void HandleMouseInteraction()
    {
        Ray ray = tableCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool hitThisObject = Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject;

        if (hitThisObject != isMouseOver)
        {
            isMouseOver = hitThisObject;
            UpdateOutlineState();
        }
    }

    public void OutlineOn(bool enable)
    {
        EnableOutline(enable);
        SetHighlightOnHover(!enable);
    }
    private void EnableOutline(bool enable)
    {
        manualHighlightEnabled = enable;
        UpdateOutlineState();
    }
    private void SetHighlightOnHover(bool enable)
    {
        highlightOnHover = enable;
        UpdateOutlineState();
    }
    // Метод для обновления состояния Outline (включение/выключение и настройка)
    private void UpdateOutlineState()
    {
        bool shouldBeEnabled = (isMouseOver && highlightOnHover) || manualHighlightEnabled; // Проверяем условие для включения Outline
        if (outlineComponent != null)
        {
            if (outlineSettings != null)
            {
                if (highlightOnHover)
                {
                    outlineComponent.OutlineColor = outlineSettings.basicOutlineColor;
                }
                else
                {
                    outlineComponent.OutlineColor = outlineSettings.tipOutlineColor;
                }
                outlineComponent.OutlineWidth = outlineSettings.outlineWidth;
            }
            outlineComponent.enabled = shouldBeEnabled;
        }
    }
    protected virtual void ReturnToInitialPosition()
    {
        transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * returnSpeed);

        if (Vector3.Distance(transform.position, initialPosition) < 0.01f)
        {
            ReturnToInitialPositionIfHide();

            gameLogic.CheckNumberOfObjects(); // УБРАТЬ ПОТОМ
        }
    }
    protected virtual void ReturnToInitialPositionIfHide()
    {
        transform.position = initialPosition;
        transform.localScale = initialScale;
        transform.rotation = Quaternion.Euler(initialRotation);
        isReturning = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
