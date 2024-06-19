using UnityEngine;

public class NarrationWidgetTrigger : MonoBehaviour
{
  public void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
      transform.parent.GetComponent<NarrationWidget>().TriggerNarration();
  }

}
