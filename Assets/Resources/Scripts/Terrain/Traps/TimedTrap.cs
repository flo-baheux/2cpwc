using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedTrap : Trap
{
    // Values changed won't be applied during runtime, will have to stop the play mode to make changes
    [Space(10f)]
    [SerializeField] private float startTime = 2.0f;
    [SerializeField] private float hitBoxDuration = 0.5f;
    [SerializeField] private float intervalLength = 2.0f;

    // Toggles the hit box on and off in an interval
    private void Start()
    {
        InvokeRepeating(nameof(ActivateHitBox), startTime, intervalLength);
        InvokeRepeating(nameof(DeactivateHitBox), startTime + hitBoxDuration, intervalLength);
    }

    private void ActivateHitBox()
    {
        hitBox.SetActive(true);
    }

    private void DeactivateHitBox()
    {
        hitBox.SetActive(false);
    }
}
