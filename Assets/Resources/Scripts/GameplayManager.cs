using UnityEngine;

public class GameplayManager : MonoBehaviour
{
  [SerializeField] private Checkpoint latestCheckpoint;
  [SerializeField] private Player Player1;
  [SerializeField] private Player Player2;

  void OnEnable()
  {
    Player1.playerDeadState.OnEnter += HandlePlayerDeath;
    Player1.OnCheckpointActivated += HandleCheckpointActivated;

    Player2.playerDeadState.OnEnter += HandlePlayerDeath;
    Player2.OnCheckpointActivated += HandleCheckpointActivated;
  }

  void Update()
  {

  }

  void HandlePlayerDeath(Player player)
  {
    player.RespawnToPosition(latestCheckpoint.transform.position);
  }

  void OnDisable()
  {
    Player1.playerDeadState.OnEnter -= HandlePlayerDeath;
  }

  void HandleCheckpointActivated(Checkpoint checkpoint) =>
    latestCheckpoint = checkpoint;
}
