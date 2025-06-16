using System;
using UnityEngine;

public class LampTrigger : Trigger
{
    public static event Action<LampTrigger> OnLampTriggerEnter;
    public static event Action<LampTrigger> OnLampTriggerExit;

    protected override void OnTriggerEnter(Collider other)
    {
        //base.OnTriggerEnter(other);

        if (other.CompareTag("Player") && canInteract)
        {
            OnLampTriggerEnter?.Invoke(this);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        //base.OnTriggerExit(other);

        if (other.CompareTag("Player") && canInteract)
        {
            OnLampTriggerExit?.Invoke(this);
        }
    }
}
