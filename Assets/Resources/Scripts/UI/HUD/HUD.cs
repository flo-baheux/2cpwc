using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup layoutGroup;
    
    [SerializeField] private TextMeshProUGUI player1Health;
    [SerializeField] private TextMeshProUGUI player2Health;

    [SerializeField] private GameObject player1InteractPrompt;
    [SerializeField] private GameObject player2InteractPrompt;
    
    [SerializeField] private TextMeshProUGUI displayText;

    private Transform player1Pos;
    private Transform player2Pos;
    
    private void Start()
    {
        //StartCoroutine(DisplayInRow());
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);

        foreach (Player player in players)
        {
            if (player.playerAssignment == PlayerAssignment.Player1)
                player.health.PlayerHealthChanged += ChangePlayer1Health;

            if (player.playerAssignment == PlayerAssignment.Player2)
                player.health.PlayerHealthChanged += ChangePlayer2Health;
            
            player.OnInteractionDetected += DisplayInteractPrompt;
        }
    }

    public void ChangePlayer1Health(int currentHealth) =>
        player1Health.text = $"{currentHealth}x";
    
    public void ChangePlayer2Health(int currentHealth) =>
        player2Health.text = $"{currentHealth}x";
    
    public void DisplayInteractPrompt(PlayerAssignment player, bool turnOn)
    {
        GameObject prompt = player1InteractPrompt;
        
        if (player == PlayerAssignment.Player2)
            prompt = player2InteractPrompt;

        prompt.SetActive(turnOn);
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
