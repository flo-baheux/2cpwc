using UnityEngine;
using UnityEngine.Playables;

public class Shrine : MonoBehaviour, Interactable
{
  [SerializeField] private PlayableDirector playableDirector;
  private GameplayManager gameManager;

  void Start()
  {
    gameManager = GameObject.Find("GameManager").GetComponent<GameplayManager>();
  }

  public void Interact(Player player)
  {
    gameManager.StartFinalCutscene();
    playableDirector.Play();
  }

  public void OnSacrifice() => gameManager.SacrificeSimba();
}
