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
        // ������� ������ � GameController
        playerController = FindFirstObjectByType<PlayerController>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        // ���������, ��� � ������� ����� �����
        if (other.CompareTag("Player") && canInteract)
        {
            // ������������� ����
            playerController.SetPlayerInside(true);
            OnPlayerEnterTrigger?.Invoke(this);
        }
    }
    protected virtual void OnTriggerExit(Collider other)
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