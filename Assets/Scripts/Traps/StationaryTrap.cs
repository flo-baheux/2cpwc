using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryTrap : Trap
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Stationary Trap Touches Player");
    }
}
