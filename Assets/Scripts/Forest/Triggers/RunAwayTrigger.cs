using System;
using UnityEngine;

public class RunAwayTrigger : Trigger
{
    public static event Action<RunAwayTrigger> OnRunAwayTriggerEnter;
    public static event Action<RunAwayTrigger> OnMonsterTriggerExit;

    protected override void OnTriggerEnter(Collider other)
    {
        //base.OnTriggerEnter(other);

        if (other.CompareTag("Player") && canInteract)
        {
            OnRunAwayTriggerEnter?.Invoke(this);
            Debug.Log("¿¿¿¿¿¿¿! —Ô‡ÒËÚÂ! √”—»»»»!");
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
       // base.OnTriggerExit(other);

        if (other.CompareTag("Player") && canInteract)
        {
            OnMonsterTriggerExit?.Invoke(this);
        }
    }
}
