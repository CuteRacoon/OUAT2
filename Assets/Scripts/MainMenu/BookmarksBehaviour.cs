using UnityEngine;

public class BookmarksBehaviour : MonoBehaviour
{
    public AudioClip tableSound;
    public AudioClip bookMarkSound;

    private AudioSource audioSource;
    private GameObject canvas;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        canvas = GameObject.FindWithTag("Canvas");
        if (canvas != null)
        {
            canvas.SetActive(false);
        }
    }
    public void PlayTableSound(int state)
    {
        switch (state)
        {
            case 0:
                audioSource.volume = 0.4f;
                audioSource.PlayOneShot(tableSound);
                break;
            case 1:
                audioSource.volume = 0.2f;
                audioSource.PlayOneShot(tableSound);
                break;
        }
    }
    public void PlayBookmarkSound()
    {
        audioSource.volume = 0.4f;
        audioSource.PlayOneShot(bookMarkSound);
    }
    public void ActiveBookButtons()
    {
        canvas.SetActive(true);
    }
}
