using UnityEngine;

public class RoomDoor : MonoBehaviour, Interactable
{
  public int id;
  public Transform player1ExitPos;
  public Transform player2ExitPos;

  public void Interact(Player player)
  {
    GameObject.Find("GameplayManager").GetComponent<GameplayManager>().RoomTransitionFrom(id);
  }
}
