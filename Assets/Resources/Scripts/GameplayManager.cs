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
  [SerializeField] private CinemachineVirtualCamera narrationCamera;

  [SerializeField] private Player SimbaLevel4;
  [SerializeField] private Player BastetLevel4;

  public void Awake()
  {
    SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive).completed += (AsyncOperation sceneLoad) =>
    {
      audioController = GetComponent<GameAudioController>();
      audioController.PlayDefaultAmbiantSounds();
    };
  }

  public void LoadGameFromMainMenu()
  {
    SceneManager.UnloadSceneAsync("MainMenu");

    SceneManager.LoadSceneAsync(sceneToLoadPlayersOnStart, LoadSceneMode.Additive).completed += (AsyncOperation sceneLoad) =>
    {
      SceneManager.LoadScene("IngameHUD", LoadSceneMode.Additive);
      audioController.PlayForestBGM();
      SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoadPlayersOnStart));
      currentRoomManager = FindObjectOfType<DoorManager>();

      SetupPlayer1();
      SetupPlayer2();

      MovePlayersToSceneAtDoor(sceneToLoadPlayersOnStart, doorToLoadPlayersOnStart);
    };
  }

  public void SetupPlayer1()
  {
    Player1.gameObject.SetActive(true);
    Player1.SetPlayerAssignment(PlayerAssignment.Player1);
    Player1.playerDeadState.OnEnter += HandlePlayerDeath;
    Player1.OnCheckpointActivated += HandleCheckpointActivated;
    cameraTargetGroup.AddMember(Player1.transform, 5, 2);
  }

  public void SetupPlayer2()
  {
    Player2.gameObject.SetActive(true);
    Player2.SetPlayerAssignment(PlayerAssignment.Player2);
    Player2.playerDeadState.OnEnter += HandlePlayerDeath;
    Player2.OnCheckpointActivated += HandleCheckpointActivated;
    cameraTargetGroup.AddMember(Player2.transform, 5, 2);
  }

  public void UnsetupPlayer1()
  {
    Player1.gameObject.SetActive(false);
    Player1.playerDeadState.OnEnter -= HandlePlayerDeath;
    Player1.OnCheckpointActivated -= HandleCheckpointActivated;
    cameraTargetGroup.RemoveMember(Player1.transform);
  }

  public void UnsetupPlayer2()
  {
    Player2.gameObject.SetActive(true);
    Player2.playerDeadState.OnEnter -= HandlePlayerDeath;
    Player2.OnCheckpointActivated -= HandleCheckpointActivated;
    cameraTargetGroup.RemoveMember(Player2.transform);
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

    // Specifics for level 4
    if (sceneDoorLink.sceneName == "Level4")
    {
      audioController.PlayAncientCityBGM();
      UnsetupPlayer1();
      UnsetupPlayer2();
      Player1 = SimbaLevel4;
      Player2 = BastetLevel4;
      SetupPlayer1();
      SetupPlayer2();
    }

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
    StartCoroutine(RespawnAfter3Secs());
  }

  IEnumerator RespawnAfter3Secs()
  {
    yield return new WaitForSeconds(3);
    Player1.RespawnToPosition(latestCheckpoint.transform.position);
    Player2.RespawnToPosition(latestCheckpoint.transform.position);
  }

  void HandleCheckpointActivated(Checkpoint checkpoint)
  {
    // FindObjectOfType<HUD>().DisplayMessage("Checkpoint activated!");
    latestCheckpoint = checkpoint;
  }

  public bool CompareCollectible(CollectibleScriptableObject collectible)
  {
    return collectibles.Contains(collectible);
  }

  public void PauseResumeGame()
  {
    Debug.Log("PauseResume called - " + gamePaused);
    if (!gamePaused)
    {
      Time.timeScale = 0;
      Player1.controlsEnabled = false;
      Player2.controlsEnabled = false;
      SceneManager.LoadScene("IngameMenu", LoadSceneMode.Additive);
    }
    else
    {
      AsyncOperation op = SceneManager.UnloadSceneAsync("IngameMenu");
      Debug.Log(op);
      op.completed += (AsyncOperation _) =>
      {
        Time.timeScale = 1;
        Player1.controlsEnabled = true;
        Player2.controlsEnabled = true;
      };
    }
    gamePaused = !gamePaused;
  }

  public IEnumerator TriggerNarrationCutsceneCoroutine(GameObject narrationTarget, AudioClip audioNarration, bool strongFocus)
  {
    audioController.NarrationSource.PlayOneShot(audioNarration);

    narrationTarget.SetActive(true);
    if (strongFocus)
    {
      narrationCamera.Follow = narrationTarget.transform;
      narrationCamera.LookAt = narrationTarget.transform;

      Player1.controlsEnabled = false;
      Player2.controlsEnabled = false;

      narrationCamera.gameObject.SetActive(true);
      yield return new WaitForSecondsRealtime(audioNarration.length + 1);
      narrationCamera.gameObject.SetActive(false);

      Player1.controlsEnabled = true;
      Player2.controlsEnabled = true;
    }
    else
    {
      cameraTargetGroup.AddMember(narrationTarget.transform, 2, 0);
      audioController.NarrationSource.PlayOneShot(audioNarration);
      yield return new WaitForSecondsRealtime(audioNarration.length + 1);
      cameraTargetGroup.RemoveMember(narrationTarget.transform);
      narrationTarget.SetActive(false);
    }
  }


  public void DisablePlayersForFinalCutscene()
  {
    Player1.gameObject.SetActive(false);
    Player2.gameObject.SetActive(false);
  }
}
