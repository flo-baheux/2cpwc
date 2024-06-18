using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
  [SerializeField] private Button ResumeButton;
  [SerializeField] private GameObject settingsScreen;

  private void OnEnable()
  {
    ResumeButton.Select();
  }

  private void Start()
  {
    ResumeButton.Select();
  }

  public void OnClickSettingsScreen()
  {
    settingsScreen.SetActive(true);
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
