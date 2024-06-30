using System.Collections;
using UnityEngine;

public class TutorialWidget : MonoBehaviour
{
  [SerializeField] private Canvas canvas;

  public void Trigger()
  {
    StartCoroutine(TriggerNarrationThenDestroySelfCoroutine());
  }

  IEnumerator TriggerNarrationThenDestroySelfCoroutine()
  {
    canvas.gameObject.SetActive(true);
    yield return new WaitForSecondsRealtime(10);
    Destroy(gameObject);
  }
}
