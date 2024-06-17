using UnityEngine;
using UnityEngine.UI;

public class CreditsScreen : MonoBehaviour
{
  [SerializeField] private GameObject startScreen;
  [SerializeField] private Button BackToMenuButton;

  private void OnEnable()
  {
    BackToMenuButton.Select();
  }

  private void Start()
  {
    BackToMenuButton.Select();
  }

  public void onClickCloseSettings()
  {
    startScreen.SetActive(true);
    gameObject.SetActive(false);
  }
}
