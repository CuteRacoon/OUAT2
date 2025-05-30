using System;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    private PlayerController playerController;
    public int index;
    public bool canInteract = true;

    public static event Action<TriggerController> OnPlayerEnterTrigger;
    public static event Action<TriggerController> OnPlayerExitTrigger;

    void Start()
    {
        // Находим объект с GameController
        playerController = FindFirstObjectByType<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Проверяем, что в триггер вошел игрок
        if (other.CompareTag("Player") && canInteract)
        {
            // Устанавливаем флаг
            playerController.SetPlayerInside(true);
            OnPlayerEnterTrigger?.Invoke(this);
        }
    }
    void OnTriggerExit(Collider other)
    {
        // Проверяем, что в триггер вошел игрок
        if (other.CompareTag("Player") && canInteract)
        {
            // Устанавливаем флаг
            playerController.SetPlayerInside(false);
            OnPlayerExitTrigger?.Invoke(this);
        }
    }
}