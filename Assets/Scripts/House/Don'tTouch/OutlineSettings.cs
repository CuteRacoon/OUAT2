using UnityEngine;

public class OutlineSettings : MonoBehaviour
{
    [Tooltip("Цвет обводки, используемый для выделения интерактивных объектов.")]
    public Color basicOutlineColor = Color.yellow;  // Задайте цвет по умолчанию.
    public Color tipOutlineColor = Color.blue;
    public float outlineWidth = 1f;
}