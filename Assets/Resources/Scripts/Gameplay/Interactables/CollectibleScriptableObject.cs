using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Collectible", menuName = "ScriptableObjects/CollectibleScriptableObject", order = 1)]
public class CollectibleScriptableObject : ScriptableObject
{
    public Sprite sprite;
    public bool collected;
}
