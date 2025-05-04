// using UnityEngine;
using System.Collections;
using UnityEngine;

public class DelayedAnimatorStart : MonoBehaviour
{
    public Animator coverAnimator;
    public Animator pagesAnimator;
    public string animationTrigger = "Start"; // Название триггера в Animator


    private void Start()
    {
        Invoke(nameof(StartAnimation), 3f);
    }

    void StartAnimation()
    {
        if (coverAnimator != null)
        {
            coverAnimator.SetTrigger(animationTrigger);
        }
        else
        {
            Debug.LogWarning("coverAnimator не назначен в инспекторе!");
        }

        if (pagesAnimator != null)
        {
            pagesAnimator.SetTrigger(animationTrigger);
        }
        else
        {
            Debug.LogWarning("pagesAnimator не назначен в инспекторе!");
        }
    }
}