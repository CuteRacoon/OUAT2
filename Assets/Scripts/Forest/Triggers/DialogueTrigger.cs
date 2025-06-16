using System;
using UnityEngine;

public class DialogueTrigger : Trigger
{
    public static event Action<DialogueTrigger> OnDialogueTriggerEnter;
    public static event Action<DialogueTrigger> OnDialogueTriggerExit;
    protected override void OnTriggerEnter(Collider other)
    {
        //base.OnTriggerEnter(other);

        if (other.CompareTag("Player") && canInteract)
        {
            OnDialogueTriggerEnter?.Invoke(this);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        //base.OnTriggerExit(other);

        if (other.CompareTag("Player") && canInteract)
        {
            OnDialogueTriggerExit?.Invoke(this);
        }
    }
}
