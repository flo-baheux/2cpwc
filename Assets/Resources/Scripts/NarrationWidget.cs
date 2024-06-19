using System.Collections;
using UnityEngine;

public class NarrationWidget : MonoBehaviour
{
  [SerializeField] private Canvas canvas;
  [SerializeField] private AudioClip narrationAudio;
  private GameplayManager gameManager;

  void Start()
  {
    gameManager = GameObject.Find("GameManager").GetComponent<GameplayManager>();
  }

  public void TriggerNarration()
  {
    StartCoroutine(TriggerNarrationThenDestroySelfCoroutine());
  }

  IEnumerator TriggerNarrationThenDestroySelfCoroutine()
  {
    yield return gameManager.TriggerNarrationCutsceneCoroutine(canvas.gameObject, narrationAudio);
    Destroy(gameObject);
  }
}
