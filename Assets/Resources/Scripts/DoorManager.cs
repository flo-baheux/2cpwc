using UnityEngine;

public class DoorManager : MonoBehaviour
{
  public void Start()
  {
    GameObject.Find("GameManager").GetComponent<GameplayManager>().currentRoomManager = this;
  }

  public Vector2 GetDoorExitPositionForPlayer(PlayerAssignment playerAssignment, int doorId)
  {

    foreach (Transform child in transform)
    {
      RoomDoor door = child.GetComponent<RoomDoor>();
      if (door && door.id == doorId)
        return playerAssignment == PlayerAssignment.Player1 ? door.player1ExitPos.position : door.player2ExitPos.position;
    }
    Debug.LogError("No room with id " + doorId + " found");
    return Vector2.zero;
  }
}
