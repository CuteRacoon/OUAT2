using UnityEngine;

public class TestScript : MonoBehaviour
{
    [Tooltip("Объект, который будет перемещаться. Если оставить пустым — будет перемещён сам объект со скриптом.")]
    public Transform character;

    [Tooltip("Целевая точка, куда переместить.")]
    public Transform target;

    void Reset()
    {
        if (character == null) character = transform;
    }

    // Этот метод можно вызвать с кнопки в инспекторе, UI или где угодно
    public void Teleport()
    {
        if (character != null && target != null)
        {
            character.position = target.position;
            character.rotation = target.rotation; // если нужно ещё и повернуть
        }
    }
}
