using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
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
  public SceneAsset scene;
  public int doorId;
}
public class GameplayManager : MonoBehaviour
{
  [SerializeField] private Checkpoint latestCheckpoint;
  [SerializeField] private Player Player1;
  [SerializeField] private Player Player2;
  public List<SceneToSceneLink> sceneLinks;
  [SerializeField] private List<CollectibleScriptableObject> collectibles;

  public DoorManager currentRoomManager = null;
  bool inTransition = false;

  void Awake()
  {
    DontDestroyOnLoad(gameObject);
  }

  void OnEnable()
  {
    Player1.playerDeadState.OnEnter += HandlePlayerDeath;
    Player1.OnCheckpointActivated += HandleCheckpointActivated;

    Player2.playerDeadState.OnEnter += HandlePlayerDeath;
    Player2.OnCheckpointActivated += HandleCheckpointActivated;
  }

  public void RoomTransitionFrom(int doorId)
  {
    SceneDoorLink target = null;
    foreach (SceneToSceneLink sceneLink in sceneLinks)
    {
      if (sceneLink.from.scene.name == SceneManager.GetActiveScene().name && sceneLink.from.doorId == doorId)
        target = sceneLink.to;
    }

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

    AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(sceneDoorLink.scene.name, LoadSceneMode.Additive);
    while (!asyncLoadScene.isDone)
      yield return null;

    SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneDoorLink.scene.name));

    int[] playerInstanceIds = { Player1.gameObject.GetInstanceID(), Player2.gameObject.GetInstanceID() };
    SceneManager.MoveGameObjectsToScene(new NativeArray<int>(playerInstanceIds, Allocator.Temp), SceneManager.GetSceneByName(sceneDoorLink.scene.name));
    SceneManager.UnloadSceneAsync(sceneBeforeTransition);

    Vector2 exitPosition = currentRoomManager.GetRoomExitPosition(sceneDoorLink.doorId);
    Player1.transform.position = exitPosition;
    Player2.transform.position = exitPosition;
    inTransition = false;
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

  void OnDisable()
  {
    Player1.playerDeadState.OnEnter -= HandlePlayerDeath;
    Player1.OnCheckpointActivated -= HandleCheckpointActivated;

    Player2.playerDeadState.OnEnter -= HandlePlayerDeath;
    Player2.OnCheckpointActivated -= HandleCheckpointActivated;
  }
}
