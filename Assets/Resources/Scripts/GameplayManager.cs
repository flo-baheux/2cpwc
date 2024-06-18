using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SceneToSceneLink
{
  public SceneDoorLink from;
  public SceneDoorLink to;
}

[Serializable]
public class SceneDoorLink
{
  public string sceneName;
  public int doorId;
}
public class GameplayManager : MonoBehaviour
{
  [SerializeField] private string sceneToLoadPlayersOnStart;
  [SerializeField] private int doorToLoadPlayersOnStart;

  private Checkpoint latestCheckpoint;
  [SerializeField] private List<SceneToSceneLink> sceneLinks;
  [SerializeField] private List<CollectibleScriptableObject> collectibles;
  [SerializeField] private CinemachineTargetGroup cameraTargetGroup;

  public DoorManager currentRoomManager;

  private Player Player1;
  private Player Player2;

  bool inTransition = false;
  bool gamePaused = false;

  public void Awake()
  {
    AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(sceneToLoadPlayersOnStart, LoadSceneMode.Additive);
    sceneLoading.completed += (AsyncOperation scene) =>
    {
      SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoadPlayersOnStart));
      currentRoomManager = FindObjectOfType<DoorManager>();
    };
  }

  public void AttachPlayer(Player player)
  {
    if (player.playerAssignment == PlayerAssignment.Player1)
      Player1 = player;
    else
      Player2 = player;

    player.playerDeadState.OnEnter += HandlePlayerDeath;
    player.OnCheckpointActivated += HandleCheckpointActivated;
    MovePlayerToSceneAtDoor(player, sceneToLoadPlayersOnStart, doorToLoadPlayersOnStart);
  }

  public void RoomTransitionFrom(int doorId)
  {
    SceneDoorLink target = null;
    foreach (SceneToSceneLink sceneLink in sceneLinks)
      if (sceneLink.from.sceneName == SceneManager.GetActiveScene().name && sceneLink.from.doorId == doorId)
        target = sceneLink.to;

    if (target == null)
    {
      Debug.LogError("No scene linked to that door!");
      return;
    }
    StartCoroutine(TransitionToRoom(target));

  }

  private IEnumerator TransitionToRoom(SceneDoorLink sceneDoorLink)
  {
    if (inTransition)
      yield break;
    else
      inTransition = true;
    Scene sceneBeforeTransition = SceneManager.GetActiveScene();

    AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(sceneDoorLink.sceneName, LoadSceneMode.Additive);
    while (!asyncLoadScene.isDone)
      yield return null;
    SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneDoorLink.sceneName));
    MovePlayersToSceneAtDoor(sceneDoorLink.sceneName, sceneDoorLink.doorId);
    SceneManager.UnloadSceneAsync(sceneBeforeTransition);

    inTransition = false;
  }

  void MovePlayerToSceneAtDoor(Player player, string sceneName, int doorId)
  {
    SceneManager.MoveGameObjectToScene(player.gameObject, SceneManager.GetSceneByName(sceneName));

    Vector2 exitPosition = currentRoomManager.GetDoorExitPositionForPlayer(player.playerAssignment, doorId);
    player.transform.position = exitPosition;
  }

  void MovePlayersToSceneAtDoor(string sceneName, int doorId)
  {
    if (Player1)
      MovePlayerToSceneAtDoor(Player1, sceneName, doorId);
    if (Player2)
      MovePlayerToSceneAtDoor(Player2, sceneName, doorId);
  }

  void HandlePlayerDeath(Player player)
  {
    player.RespawnToPosition(latestCheckpoint.transform.position);
  }

  void HandleCheckpointActivated(Checkpoint checkpoint) =>
    latestCheckpoint = checkpoint;

  public bool CompareCollectible(CollectibleScriptableObject collectible)
  {
    return collectibles.Contains(collectible);
  }

  public void PauseResumeGame()
  {
    if (!gamePaused)
    {
      Time.timeScale = 0;
      Player1.controlsEnabled = false;
      Player2.controlsEnabled = false;
      SceneManager.LoadScene("IngameMenu", LoadSceneMode.Additive);
      gamePaused = true;
    }
    else
    {
      AsyncOperation op = SceneManager.UnloadSceneAsync("IngameMenu");
      op.completed += (AsyncOperation _) =>
      {
        Time.timeScale = 1;
        Player1.controlsEnabled = false;
        Player2.controlsEnabled = false;
        gamePaused = false;
      };
    }
  }
}
