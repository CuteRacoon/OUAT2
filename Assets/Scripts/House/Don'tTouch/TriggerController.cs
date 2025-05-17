using UnityEngine;

public class TriggerController : MonoBehaviour
{
    private PlayerController playerController;
    private InteractionController interactionController;
    public int index;
    public bool canInteract = true;

    void Start()
    {
        // ������� ������ � GameController
        playerController = FindFirstObjectByType<PlayerController>();
        interactionController = FindAnyObjectByType<InteractionController>();
    }

    void OnTriggerEnter(Collider other)
    {
        // ���������, ��� � ������� ����� �����
        if (other.CompareTag("Player") && canInteract)
        {
            // ������������� ����
            playerController.SetPlayerInside(true);
            if (interactionController != null) interactionController.SetActiveTrigger(index);
        }
    }
    void OnTriggerStay(Collider other)
    {
        // ���������, ��� � ������� ����� �����
        if (other.CompareTag("Player") && canInteract)
        {
            // ������������� ����
            playerController.SetPlayerInside(true);
            if (interactionController != null) interactionController.SetActiveTrigger(index);
        }
    }
    void OnTriggerExit(Collider other)
    {
        // ���������, ��� � ������� ����� �����
        if (other.CompareTag("Player") && canInteract)
        {
            // ������������� ����
            playerController.SetPlayerInside(false);
            if (interactionController != null && interactionController.IsCurrentTrigger(index))
            {
                interactionController.SetActiveTrigger(-1);
            }
        }
    }
}