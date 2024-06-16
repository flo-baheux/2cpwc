using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxDamage : MonoBehaviour
{
    private int _damageDealt = 1;

    public int GetDamageValue()
    {
        return _damageDealt;
    }

    public void SetDamageValue(int damageValue)
    {
        _damageDealt = damageValue;
    }
}
