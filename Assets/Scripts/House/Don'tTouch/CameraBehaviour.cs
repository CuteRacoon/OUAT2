using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Camera[] cameras; // ������ ����� ��� ������������
    private int currentCameraIndex = 0; // ������ ������� �������� ������


    void Start()
    {
        // ��������� ��� ������, ����� ������
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        // ��������, ��� ���� ���� �� ���� ������
        if (cameras.Length == 0)
        {
            Debug.LogError("��� ����� ��� ������������! �������� ������ � ������ 'Cameras'.");
            enabled = false; // ��������� ������, ����� �������� ������
            return;
        }

        if (cameras[0] == null)
        {
            Debug.LogError("������ ������ �� ���������!");
            enabled = false;
            return;
        }

        //�������� ������ ������, ���� ��� ���������
        cameras[0].gameObject.SetActive(true);
    }

    public void SwitchCamera(int index)
    {
        cameras[currentCameraIndex].gameObject.SetActive(false);
        cameras[index].gameObject.SetActive(true);
        currentCameraIndex = index;
    }
    public Camera GetCamera()
    {
        return cameras[1];
    }
    public Camera GetMonsterCamera()
    {
        return cameras[1];
    }
}