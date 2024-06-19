using System.Collections;
using UnityEngine;
using TMPro;


public class DisplayText : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float displaySpeed = 0.5f;
    [SerializeField] private TextMeshProUGUI text; 
    
    private void Start()
    {
        StartCoroutine(FadeText());
    }
    
    IEnumerator FadeText()
    {
        float timer = 0f;
        Color textColor = text.color;
        
        while (timer < 1.0f)
        {
            timer += 0.01f * displaySpeed;
            
            textColor.a = curve.Evaluate(timer);
            text.color = textColor;
            
            yield return new WaitForSeconds(0.01f);
        }
    }
}
