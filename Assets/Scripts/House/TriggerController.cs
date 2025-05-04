using UnityEngine;

public class TriggerController : MonoBehaviour
{
    private PlayerController playerController;
    private InteractionController interactionController;
    public int index;

    void Start()
    {
        // Находим объект с GameController
        playerController = FindFirstObjectByType<PlayerController>();
        interactionController = FindAnyObjectByType<InteractionController>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Проверяем, что в триггер вошел игрок
        if (other.CompareTag("Player"))
        {
            // Устанавливаем флаг
            playerController.SetPlayerInside(true);
            if (interactionController != null) interactionController.SetActiveTrigger(index);
        }
    }
    void OnTriggerExit(Collider other)
    {
        // Проверяем, что в триггер вошел игрок
        if (other.CompareTag("Player"))
        {
            // Устанавливаем флаг
            playerController.SetPlayerInside(false);
        }
    }
}