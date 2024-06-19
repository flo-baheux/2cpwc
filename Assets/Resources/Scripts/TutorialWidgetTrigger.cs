using UnityEngine;

public class TutorialWidgetTrigger : MonoBehaviour
{
  public void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
      transform.parent.GetComponent<TutorialWidget>().Trigger();
  }
}
