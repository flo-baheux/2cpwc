using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
  [SerializeField] private Button PlayButton;
  [SerializeField] private GameObject settingsScreen;
  [SerializeField] private GameObject creditsScreen;

  private void OnEnable()
  {
    PlayButton.Select();
  }

  private void Start()
  {
    PlayButton.Select();
  }

  public void OnClickPlay()
  {
    GameplayManager gameplayManager = GameObject.Find("GameManager").GetComponent<GameplayManager>();
    if (!gameplayManager)
      return;
    gameplayManager.LoadGameFromMainMenu();
  }

  public void OnClickSettingsScreen()
  {
    settingsScreen.SetActive(true);
    gameObject.SetActive(false);
  }

  public void OnClickCreditsScreen()
  {
    creditsScreen.SetActive(true);
    gameObject.SetActive(false);
  }

  public void OnClickResume()
  {
    GameplayManager gameplayManager = GameObject.Find("GameManager").GetComponent<GameplayManager>();
    if (!gameplayManager)
      return;
    gameplayManager.PauseResumeGame();
  }

  public void OnClickQuitGame()
  {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif
    Application.Quit();
  }
}
