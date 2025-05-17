using UnityEngine;
using System.Collections;

public class BookmarksBehaviour : MonoBehaviour
{
    public AudioClip tableSound;
    public AudioClip bookMarkSound;

    private AudioSource audioSource;
    private MainMenuController mainMenuController;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mainMenuController = FindAnyObjectByType<MainMenuController>();
        
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
        mainMenuController.ActivateCanvas();
    }
}
