using UnityEngine;

public class TriggerController : MonoBehaviour
{
    private PlayerController playerController;
    private InteractionController interactionController;
    public int index;

    void Start()
    {
        // ������� ������ � GameController
        playerController = FindFirstObjectByType<PlayerController>();
        interactionController = FindAnyObjectByType<InteractionController>();
    }

    void OnTriggerEnter(Collider other)
    {
        // ���������, ��� � ������� ����� �����
        if (other.CompareTag("Player"))
        {
            // ������������� ����
            playerController.SetPlayerInside(true);
            if (interactionController != null) interactionController.SetActiveTrigger(index);
        }
    }
    void OnTriggerExit(Collider other)
    {
        // ���������, ��� � ������� ����� �����
        if (other.CompareTag("Player"))
        {
            // ������������� ����
            playerController.SetPlayerInside(false);
        }
    }
}