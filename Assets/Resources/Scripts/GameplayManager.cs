using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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

  [SerializeField] private Player Player1;
  [SerializeField] private Player Player2;

  bool inTransition = false;
  bool gamePaused = false;

  private GameAudioController audioController;

  public void Awake()
  {
    SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);

    audioController = GetComponent<GameAudioController>();
    audioController.PlayDefaultAmbiantSounds();
  }

  public void LoadGameFromMainMenu()
  {
    SceneManager.UnloadSceneAsync("MainMenu");
    AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(sceneToLoadPlayersOnStart, LoadSceneMode.Additive);
    sceneLoading.completed += (AsyncOperation scene) =>
    {
      audioController.PlayForestBGM();
      SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoadPlayersOnStart));
      currentRoomManager = FindObjectOfType<DoorManager>();

      Player1.SetPlayerAssignment(PlayerAssignment.Player1);
      Player1.playerDeadState.OnEnter += HandlePlayerDeath;
      Player1.OnCheckpointActivated += HandleCheckpointActivated;

      Player2.SetPlayerAssignment(PlayerAssignment.Player2);
      Player2.playerDeadState.OnEnter += HandlePlayerDeath;
      Player2.OnCheckpointActivated += HandleCheckpointActivated;
      MovePlayersToSceneAtDoor(sceneToLoadPlayersOnStart, doorToLoadPlayersOnStart);
    };
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
