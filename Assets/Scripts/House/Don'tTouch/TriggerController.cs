using UnityEngine;

public class TriggerController : MonoBehaviour
{
    private PlayerController playerController;
    private InteractionController interactionController;
    public int index;
    public bool canInteract = true;

    void Start()
    {
        // Находим объект с GameController
        playerController = FindFirstObjectByType<PlayerController>();
        interactionController = FindAnyObjectByType<InteractionController>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Проверяем, что в триггер вошел игрок
        if (other.CompareTag("Player") && canInteract)
        {
            // Устанавливаем флаг
            playerController.SetPlayerInside(true);
            if (interactionController != null) interactionController.SetActiveTrigger(index);
        }
    }
    void OnTriggerStay(Collider other)
    {
        // Проверяем, что в триггер вошел игрок
        if (other.CompareTag("Player") && canInteract)
        {
            // Устанавливаем флаг
            playerController.SetPlayerInside(true);
            if (interactionController != null) interactionController.SetActiveTrigger(index);
        }
    }
    void OnTriggerExit(Collider other)
    {
        // Проверяем, что в триггер вошел игрок
        if (other.CompareTag("Player") && canInteract)
        {
            // Устанавливаем флаг
            playerController.SetPlayerInside(false);
            if (interactionController != null && interactionController.IsCurrentTrigger(index))
            {
                interactionController.SetActiveTrigger(-1);
            }
        }
    }
}