using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractPrompt : MonoBehaviour
{
    [SerializeField] private Image promptImage;
    [SerializeField] private TextMeshProUGUI promptText;
    
    private void OnEnable()
    {
        // StopAllCoroutines();
        // StartCoroutine(FadeIn());
    }

    private void OnDisable()
    {
        // StartCoroutine(FadeOut());
        // StopAllCoroutines();
    }

    private void SetPromptAlpha(float value)
    {
        Color promptColor = promptImage.color;
        Color textColor = promptText.color;

        promptColor.a = value;
        textColor.a = value;

        promptImage.color = promptColor;
        promptText.color = textColor;
    }
}
