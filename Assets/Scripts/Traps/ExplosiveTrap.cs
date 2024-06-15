using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExplosiveTrap : Trap
{    
    [Space(10f)]
    [SerializeField] private float explosionDelay = 3f;
    [SerializeField] private float explosionDuration = 1f;
    [SerializeField] private float flashInterval = 0.2f;

    private SpriteRenderer _sprite;
    private Color _originalColor;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _originalColor = _sprite.color;
    }
    
    public void DetonateExplosive()
    {
        Invoke(nameof(Explode), explosionDelay);
        StartCoroutine(FlashColors());
    }

    private void Explode()
    {
        hitBox.SetActive(true);
        Destroy(gameObject, 1f);
    }

    IEnumerator FlashColors()
    {
        float timer = explosionDelay;
        bool isRed = false;

        while (timer >= 0.0f)
        {
            timer -= flashInterval;

            isRed = !isRed;
            _sprite.color = isRed ? Color.red : _originalColor;
            
            yield return new WaitForSeconds(flashInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DetonateExplosive();
        }
    }
}

// Adds a button to the inspector for easier testing
[CustomEditor(typeof(ExplosiveTrap))]
public class ExplosiveTrapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ExplosiveTrap trapClass = (ExplosiveTrap)target;
        if(GUILayout.Button("Detonate Explosive"))
        {
            trapClass.DetonateExplosive();
        }
    }
}
