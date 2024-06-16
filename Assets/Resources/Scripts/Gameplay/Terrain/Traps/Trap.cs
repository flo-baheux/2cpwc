using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] protected GameObject hitBox;
    [SerializeField] private int damageValue = 1;

    private void Awake()
    {
        hitBox.GetComponent<HitBoxDamage>().SetDamageValue(damageValue);
    }
}
