using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController
{
  private bool InTransition = false;

  public void LoadMainMenu(Action<AsyncOperation> callback)
  {
    SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive).completed += callback;
  }

  public void LoadGameFromMainMenu(string landingSceneName, Action<AsyncOperation> callback)
  {
    SceneManager.UnloadSceneAsync("MainMenu");

    AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(landingSceneName, LoadSceneMode.Additive);
    sceneLoad.completed += callback;
    sceneLoad.completed +=
    (AsyncOperation sceneLoad) =>
    {
      SceneManager.LoadScene("IngameHUD", LoadSceneMode.Additive);
      SceneManager.SetActiveScene(SceneManager.GetSceneByName(landingSceneName));
    };
  }

  public void TransitionToSceneDoor(SceneDoorLink sceneDoorLink, Action<AsyncOperation> postNewSceneLoadedCallback = null, Action<AsyncOperation> postOldSceneUnloadedCallback = null)
  {
    if (InTransition)
      return;
    InTransition = true;
    Scene sceneBeforeTransition = SceneManager.GetActiveScene();

    AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneDoorLink.sceneName, LoadSceneMode.Additive);
    sceneLoad.completed += postNewSceneLoadedCallback;
    sceneLoad.completed += (AsyncOperation sceneLoad) =>
    {
      SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneDoorLink.sceneName));
      SceneManager.UnloadSceneAsync(sceneBeforeTransition).completed += postOldSceneUnloadedCallback;
    };

    InTransition = false;
  }


  public void LoadPauseScene(Action<AsyncOperation> callback = null)
    => SceneManager.LoadSceneAsync("IngameMenu", LoadSceneMode.Additive).completed += callback;

  public void UnloadPauseScene(Action<AsyncOperation> callback = null)
    => SceneManager.UnloadSceneAsync("IngameMenu").completed += callback;

  public void UnloadIngameHUD(Action<AsyncOperation> callback = null)
    => SceneManager.UnloadSceneAsync("IngameHUD").completed += callback;
}
