using System;
using UnityEngine;

public class HideTrigger : Trigger
{
    public static event Action<HideTrigger> OnHideTriggerEnter;
    public static event Action<HideTrigger> OnHideTriggerExit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void OnTriggerEnter(Collider other)
    {
        //base.OnTriggerEnter(other);

        if (other.CompareTag("Player") && canInteract)
        {
            OnHideTriggerEnter?.Invoke(this);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        //base.OnTriggerExit(other);

        if (other.CompareTag("Player") && canInteract)
        {
            OnHideTriggerExit?.Invoke(this);
        }
    }
}
