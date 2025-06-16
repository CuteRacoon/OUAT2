using System;
using UnityEngine;

public class GooseTrigger: Trigger
{
    public static event Action<GooseTrigger> OnGooseTriggerEnter;
    public static event Action<GooseTrigger> OnGooseTriggerExit;

    protected override void OnTriggerEnter(Collider other)
    {
        //base.OnTriggerEnter(other);

        if (other.CompareTag("Player") && canInteract)
        {
            OnGooseTriggerEnter?.Invoke(this);
            Debug.Log("¿¿¿¿¿¿¿! —Ô‡ÒËÚÂ! √”—»»»»!");
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        // base.OnTriggerExit(other);

        if (other.CompareTag("Player") && canInteract)
        {
            OnGooseTriggerExit?.Invoke(this);
        }
    }
}
