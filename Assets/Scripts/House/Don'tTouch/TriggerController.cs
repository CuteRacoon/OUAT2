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
        // ������� ������ � GameController
        playerController = FindFirstObjectByType<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        // ���������, ��� � ������� ����� �����
        if (other.CompareTag("Player") && canInteract)
        {
            // ������������� ����
            playerController.SetPlayerInside(true);
            OnPlayerEnterTrigger?.Invoke(this);
        }
    }
    void OnTriggerExit(Collider other)
    {
        // ���������, ��� � ������� ����� �����
        if (other.CompareTag("Player") && canInteract)
        {
            // ������������� ����
            playerController.SetPlayerInside(false);
            OnPlayerExitTrigger?.Invoke(this);
        }
    }
}