using UnityEngine;

public class CameraThings : MonoBehaviour
{
    public GameObject horrorSounds;
    public void DisplayGirlLine()
    {
        DialogueManager.Instance.PlayPartOfPlot("girl_line_around_bake");
    }
    public void HideBoy()
    {
        BoyController.Instance.HideBoy();
    }
    public void SoundsOn()
    {
        horrorSounds.SetActive(true);
    }
}
