using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Camera[] cameras; // Массив камер для переключения
    private int currentCameraIndex = 0; // Индекс текущей активной камеры


    void Start()
    {
        // Отключаем все камеры, кроме первой
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        // Убедимся, что есть хотя бы одна камера
        if (cameras.Length == 0)
        {
            Debug.LogError("Нет камер для переключения! Добавьте камеры в массив 'Cameras'.");
            enabled = false; // Отключаем скрипт, чтобы избежать ошибок
            return;
        }

        if (cameras[0] == null)
        {
            Debug.LogError("Первая камера не назначена!");
            enabled = false;
            return;
        }

        //Включаем первую камеру, если она выключена
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