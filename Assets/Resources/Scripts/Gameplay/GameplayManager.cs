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
  // SCENES SETUP
  [SerializeField] private string sceneToLoadPlayersOnStart;
  [SerializeField] private int doorToLoadPlayersOnStart;
  [SerializeField] private List<SceneToSceneLink> sceneLinks;
  [SerializeField] private CinemachineTargetGroup cameraTargetGroup;

  // CHARACTERS
  [SerializeField] private Player Player1;
  [SerializeField] private Player Player2;
  [SerializeField] private Player SimbaLevel4;
  [SerializeField] private Player BastetLevel4;

  // GAMEPLAY
  public DoorManager currentRoomManager;
  private Checkpoint latestCheckpoint;

  // CONTROLLERS
  private GameAudioController audioController;
  [SerializeField] private SceneController sceneController;

  private List<CollectibleScriptableObject> collectibles;

  bool gamePaused = false;

  public void Awake()
  {
    sceneController = new SceneController();
    sceneController.LoadMainMenu((AsyncOperation sceneLoad) =>
    {
      audioController = GetComponent<GameAudioController>();
      audioController.PlayDefaultAmbiantSounds();
    });
  }

  public void StartGame()
  {
    sceneController.LoadGameFromMainMenu(sceneToLoadPlayersOnStart, (AsyncOperation asyncOperation) =>
    {
      audioController.PlayForestBGM();
      currentRoomManager = FindObjectOfType<DoorManager>();

      SetupPlayer1();
      SetupPlayer2();

      MovePlayersToScene(sceneToLoadPlayersOnStart);
      MovePlayersToDoor(doorToLoadPlayersOnStart);
    });
  }

  public void SetupPlayer1()
  {
    Player1.gameObject.SetActive(true);
    Player1.SetPlayerAssignment(PlayerAssignment.Player1);
    Player1.state.deadState.OnEnter += HandlePlayerDeath;
    Player1.OnCheckpointActivated += HandleCheckpointActivated;
    cameraTargetGroup.AddMember(Player1.transform, 5, 2);
  }

  public void SetupPlayer2()
  {
    Player2.gameObject.SetActive(true);
    Player2.SetPlayerAssignment(PlayerAssignment.Player2);
    Player2.state.deadState.OnEnter += HandlePlayerDeath;
    Player2.OnCheckpointActivated += HandleCheckpointActivated;
    cameraTargetGroup.AddMember(Player2.transform, 5, 2);
  }

  public void UnsetupPlayer1()
  {
    Player1.state.deadState.OnEnter -= HandlePlayerDeath;
    Player1.OnCheckpointActivated -= HandleCheckpointActivated;
    cameraTargetGroup.RemoveMember(Player1.transform);
    Destroy(Player1.gameObject);
  }

  public void UnsetupPlayer2()
  {
    Player2.state.deadState.OnEnter -= HandlePlayerDeath;
    Player2.OnCheckpointActivated -= HandleCheckpointActivated;
    cameraTargetGroup.RemoveMember(Player2.transform);
    Destroy(Player2.gameObject);
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

    // Specifics for level 4
    if (target.sceneName == "Level4")
    {
      audioController.PlayAncientCityBGM();
      UnsetupPlayer1();
      UnsetupPlayer2();
      Player1 = SimbaLevel4;
      Player2 = BastetLevel4;
      SetupPlayer1();
      SetupPlayer2();
    }

    sceneController.TransitionToSceneDoor(
      target,
      (AsyncOperation asyncOperation) => MovePlayersToScene(target.sceneName),
      (AsyncOperation asyncOperation) => MovePlayersToDoor(target.doorId)
    );
  }

  void MovePlayersToScene(string sceneName)
  {
    SceneManager.MoveGameObjectToScene(Player1.gameObject, SceneManager.GetSceneByName(sceneName));
    SceneManager.MoveGameObjectToScene(Player2.gameObject, SceneManager.GetSceneByName(sceneName));
  }

  void MovePlayersToDoor(int doorId)
  {
    Vector2 P1ExitPosition = currentRoomManager.GetDoorExitPositionForPlayer(Player1.playerAssignment, doorId);
    Player1.transform.position = new Vector3(P1ExitPosition.x, P1ExitPosition.y, Player1.transform.position.z);

    Vector2 P2ExitPosition = currentRoomManager.GetDoorExitPositionForPlayer(Player2.playerAssignment, doorId);
    Player2.transform.position = new Vector3(P2ExitPosition.x, P2ExitPosition.y, Player2.transform.position.z);
  }

  void HandlePlayerDeath(Player player) =>
    StartCoroutine(RespawnAfter3Secs());

  IEnumerator RespawnAfter3Secs()
  {
    yield return new WaitForSeconds(3);
    Player1.RespawnToPosition(latestCheckpoint.transform.position);
    Player2.RespawnToPosition(latestCheckpoint.transform.position);
  }

  void HandleCheckpointActivated(Checkpoint checkpoint)
    => latestCheckpoint = checkpoint;

  public bool CompareCollectible(CollectibleScriptableObject collectible)
    => collectibles.Contains(collectible);

  public void PauseResumeGame()
  {
    if (!gamePaused)
    {
      Time.timeScale = 0;
      Player1.controlsEnabled = false;
      Player2.controlsEnabled = false;
      sceneController.LoadPauseScene();
    }
    else
    {
      sceneController.UnloadPauseScene((AsyncOperation _) =>
      {
        Time.timeScale = 1;
        Player1.controlsEnabled = true;
        Player2.controlsEnabled = true;
      });
    }
    gamePaused = !gamePaused;
  }

  public IEnumerator TriggerNarrationCutsceneCoroutine(GameObject narrationTarget, AudioClip audioNarration, bool strongFocus)
  {
    audioController.NarrationSource.PlayOneShot(audioNarration);

    narrationTarget.SetActive(true);
    if (strongFocus)
      cameraTargetGroup.AddMember(narrationTarget.transform, 2, 0);
    audioController.NarrationSource.PlayOneShot(audioNarration);
    yield return new WaitForSecondsRealtime(audioNarration.length + 1);
    if (strongFocus)
      cameraTargetGroup.RemoveMember(narrationTarget.transform);

    narrationTarget.SetActive(false);
  }

  public void StartFinalCutscene()
  {
    Player1.controlsEnabled = false;
    Player1.state.deadState.OnEnter -= HandlePlayerDeath;

    Player2.controlsEnabled = false;
    Player2.state.deadState.OnEnter -= HandlePlayerDeath;

    audioController.AdjustAudioForFinalCutscene();
    sceneController.UnloadIngameHUD();
  }

  public void SacrificeSimba()
    => Player1.state.TransitionToState(State.DEAD);
}
