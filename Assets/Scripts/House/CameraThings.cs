using UnityEngine;

public class CameraThings : MonoBehaviour
{
    public void DisplayGirlLine()
    {
        DialogueManager.Instance.PlayPartOfPlot("girl_line_around_bake");
    }
}
