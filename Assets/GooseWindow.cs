using UnityEngine;

public class GooseWindow : MonoBehaviour
{

    public void SetGooseScream()
    {
        ActionManager.Instance.SetGooseScreaming();
    }
    public void ShakeCamera()
    {
        CameraShake camShake = Camera.main.GetComponent<CameraShake>();
        if (camShake != null) camShake.Shake(0.05f, 4f);
    }
    public void CameraAnimation()
    {
        Animation anime = Camera.main.GetComponent<Animation>();
        anime.Play();
    }
}
