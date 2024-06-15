using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
  [SerializeField] private List<Transform> checkpoints;
  [SerializeField] private Player Player1;
  [SerializeField] private Player Player2;

  void OnEnable()
  {
    Player1.playerDeadState.OnEnter += HandlePlayerDeath;
    Player2.playerDeadState.OnEnter += HandlePlayerDeath;
  }

  void Update()
  {

  }

  public void HandlePlayerDeath(Player player)
  {

    // Find closest checkpoint logic
    // Could wait until player death animation is over, or a fixed timer, using a coroutine
    player.RespawnToPosition(findClosestCheckpoint(player.transform.position));
  }

  void OnDisable()
  {
    Player1.playerDeadState.OnEnter -= HandlePlayerDeath;
  }

  // FIXME: Checkpoint could be an entity that saves if the player already visited it or not
  // to avoid warping after an obstacle / through a wall.
  Vector2 findClosestCheckpoint(Vector2 position)
  {
    Transform closestCheckpoint = checkpoints[0];
    float shortestDistance = Mathf.Infinity;
    foreach (Transform checkpoint in checkpoints)
    {
      float distance = Vector2.Distance(position, checkpoint.position);
      if (distance < shortestDistance)
      {
        shortestDistance = distance;
        closestCheckpoint = checkpoint;
      }
    }
    return closestCheckpoint.position;
  }
}
