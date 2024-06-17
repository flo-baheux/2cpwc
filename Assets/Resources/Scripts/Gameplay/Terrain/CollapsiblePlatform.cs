using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

enum TimerState
{
    Off,
    Active,
    Collapsed,
}
public class CollapsiblePlatform : MonoBehaviour
{

    [SerializeField] private float ActiveDuration = 2f;
    [SerializeField] private float CollapsedDuration = 5f;
    [SerializeField] private float FadeOutDuration = 1f;
    [SerializeField] private float FadeInDuration = 0.5f;
    private float targetTime = 0f;
    private TimerState state = TimerState.Off;
    private Collider2D collider;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    private void Awake()
    {
        state = TimerState.Off;
        targetTime = 0f;
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (state) 
        {
            case TimerState.Active:
                targetTime -= Time.deltaTime;

                if (targetTime <= 0.0f)
                {
                    collider.enabled = false;
                    StartCoroutine(LerpTransperancy(0, FadeOutDuration));

                    targetTime = CollapsedDuration;
                    state = TimerState.Collapsed;
                }
                break;
                
            case TimerState.Collapsed:
                targetTime -= Time.deltaTime;

                if (targetTime <= 0.0f)
                {
                    StartCoroutine(LerpTransperancy(1, FadeInDuration));
                    targetTime = 0;
                    state = TimerState.Off;
                    collider.enabled = true;   
                }
                break;

            case TimerState.Off:
            default:
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(state == TimerState.Off)
        {
            targetTime = ActiveDuration;
            state = TimerState.Active;
        }
    }

    private IEnumerator LerpTransperancy(float targetAlpha, float duration)
    {
        float timer = 0;
        Color startValue = spriteRenderer.color;
        Color endValue = spriteRenderer.color;
        endValue.a = targetAlpha;   

        while (timer < duration)
        {
            spriteRenderer.color = Color.Lerp(startValue, endValue, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = endValue;
    }
}
