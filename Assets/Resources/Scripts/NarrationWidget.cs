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
    Debug.Log("Triggering narration");
    StartCoroutine(gameManager.TriggerNarrationCutsceneCoroutine(canvas.gameObject, narrationAudio));
  }
}
