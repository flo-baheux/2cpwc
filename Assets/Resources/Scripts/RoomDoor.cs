using UnityEngine;

public class RoomDoor : MonoBehaviour, Interactable
{
  public int id;
  public Transform exitPosition;

  public void Interact()
  {
    GameObject.Find("GameplayManager").GetComponent<GameplayManager>().RoomTransitionFrom(id);
  }
}
