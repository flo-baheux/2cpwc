using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Shrine : MonoBehaviour, Interactable
{
  [SerializeField] private PlayableDirector pd;
    private GameplayManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameplayManager>();
    }
    public void Interact(Player player)
  {
        gameManager.DisablePlayersForFinalCutscene();
        pd.Play();
  }
}
