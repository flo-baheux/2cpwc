using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
  [SerializeField] private VerticalLayoutGroup layoutGroup;

  [SerializeField] private TextMeshProUGUI player1Health;
  [SerializeField] private TextMeshProUGUI player2Health;

  [SerializeField] private Image player1InteractPrompt;
  [SerializeField] private Image player2InteractPrompt;

  [SerializeField] private Image player1ClimbPrompt;
  [SerializeField] private Image player2ClimbPrompt;

  [SerializeField] private TextMeshProUGUI displayText;

  [SerializeField] private Vector3 promptOffset = new Vector3(0f, 2.5f, 0f);
  private Transform _player1;
  private Transform _player2;

  private Camera _cam;

  private void Start()
  {
    _cam = Camera.main;

    //StartCoroutine(DisplayInRow());
    Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);

    foreach (Player player in players)
    {
      if (player.playerAssignment == PlayerAssignment.Player1)
      {
        player.health.PlayerHealthChanged += ChangePlayer1Health;
        _player1 = player.transform;
      }

      if (player.playerAssignment == PlayerAssignment.Player2)
      {
        player.health.PlayerHealthChanged += ChangePlayer2Health;
        _player2 = player.transform;
      }

      player.OnInteractionDetected += DisplayInteractPrompt;
      player.OnClimbingDetected += DisplayClimbPrompt;
    }
  }

  private void Update()
  {
    Vector3 player1Pos = _player1.position;
    Vector3 player2Pos = _player2.position;

    player1InteractPrompt.rectTransform.position = _cam.WorldToScreenPoint(player1Pos + promptOffset);
    player1ClimbPrompt.rectTransform.position = _cam.WorldToScreenPoint(player1Pos + promptOffset);

    player2InteractPrompt.rectTransform.position = _cam.WorldToScreenPoint(player2Pos + promptOffset);
    player2ClimbPrompt.rectTransform.position = _cam.WorldToScreenPoint(player2Pos + promptOffset);
  }

  private void ChangePlayer1Health(int previousHealth, int currentHealth) =>
      player1Health.text = $"{currentHealth}x";

  private void ChangePlayer2Health(int previousHealth, int currentHealth) =>
      player2Health.text = $"{currentHealth}x";

  public void DisplayInteractPrompt(PlayerAssignment player, bool turnOn)
  {
    Image prompt = player1InteractPrompt;

    if (player == PlayerAssignment.Player2)
      prompt = player2InteractPrompt;

    prompt.gameObject.SetActive(turnOn);
  }

  public void DisplayClimbPrompt(PlayerAssignment player, bool turnOn)
  {
    // Image prompt = player1ClimbPrompt;

    // if (player == PlayerAssignment.Player2)
    //   prompt = player2ClimbPrompt;

    // prompt.gameObject.SetActive(turnOn);
  }

  public void DisplayMessage(string message)
  {
    TextMeshProUGUI text = Instantiate(displayText, transform.position, Quaternion.identity);
    text.rectTransform.SetParent(layoutGroup.transform, false);
    text.text = message;
  }

  // For testing
  // IEnumerator DisplayInRow()
  // {
  //     for (int i = 0; i < 4; i++)
  //     {
  //         DisplayMessage("Yes");
  //
  //         yield return new WaitForSeconds(0.5f);
  //     }
  // }
}
