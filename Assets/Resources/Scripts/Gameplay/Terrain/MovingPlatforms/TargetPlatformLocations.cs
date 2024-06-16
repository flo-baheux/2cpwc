using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPlatformLocations : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;

    private void Awake()
    {
        // Makes the sprite invisible
        sprite.color = new Color(1, 0, 0, 0);
    }
}
