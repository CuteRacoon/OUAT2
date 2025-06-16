using System;
using UnityEngine;

public class MonsterTrigger : Trigger
{
    public static event Action<MonsterTrigger> OnMonsterTriggerEnter;
    public static event Action<MonsterTrigger> OnMonsterTriggerExit;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Player") && canInteract)
        {
            OnMonsterTriggerEnter?.Invoke(this);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (other.CompareTag("Player") && canInteract)
        {
            OnMonsterTriggerExit?.Invoke(this);
        }
    }
}
