using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera[] cameras; // ������ ����� ��� ������������
    private int currentCameraIndex = 0; // ������ ������� �������� ������

    private Camera currentCamera;
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
        currentCamera = cameras[0];
    }

    public void SwitchCamera(int index)
    {
        cameras[currentCameraIndex].gameObject.SetActive(false);
        cameras[index].gameObject.SetActive(true);
        currentCameraIndex = index;
        currentCamera = cameras[currentCameraIndex];
    }
    public Camera GetCameraByIndex(int index)
    {
        return cameras[index];
    }
    public Camera GetCurrentCamera()
    {
        return currentCamera;
    }
}