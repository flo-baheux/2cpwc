using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractPrompt : MonoBehaviour
{
    [SerializeField] private Image promptImage;
    [SerializeField] private TextMeshProUGUI promptText;
    
    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    private void OnDisable()
    {
        StartCoroutine(FadeOut());
        StopAllCoroutines();
    }

    private IEnumerator FadeIn()
    {
        
        yield return null;
    }

    private IEnumerator FadeOut()
    {
        yield return null;
    }
}
