using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class NarrationWidget : MonoBehaviour
{
  [SerializeField] private Canvas canvas;
  [SerializeField] private AudioClip narrationAudio;
  public bool strongFocus = false;
  private GameplayManager gameManager;
  bool HasBeenActivated = false;

  void Start()
  {
    gameManager = GameObject.Find("GameManager").GetComponent<GameplayManager>();
  }

  public void TriggerNarration()
  {
    if (!HasBeenActivated)
    {
      HasBeenActivated = true;
      StartCoroutine(TriggerNarrationThenDestroySelfCoroutine());
    }
  }

  IEnumerator TriggerNarrationThenDestroySelfCoroutine()
  {
    yield return gameManager.TriggerNarrationCutsceneCoroutine(canvas.gameObject, narrationAudio, strongFocus);
    Destroy(gameObject);
  }
}
