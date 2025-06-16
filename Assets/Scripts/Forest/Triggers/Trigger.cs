using System;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    private PlayerController playerController;
    public int index;
    public bool canInteract = true;

    public static event Action<Trigger> OnPlayerEnterTrigger;
    public static event Action<Trigger> OnPlayerExitTrigger;

    void Start()
    {
        // Находим объект с GameController
        playerController = FindFirstObjectByType<PlayerController>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        // Проверяем, что в триггер вошел игрок
        if (other.CompareTag("Player") && canInteract)
        {
            // Устанавливаем флаг
            playerController.SetPlayerInside(true);
            OnPlayerEnterTrigger?.Invoke(this);
        }
    }
    protected virtual void OnTriggerExit(Collider other)
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