using UnityEngine;

public class DoorManager : MonoBehaviour
{
  public void Start()
  {
    GameObject.Find("GameplayManager").GetComponent<GameplayManager>().currentRoomManager = this;
  }

  public Vector2 GetDoorExitPosition(int doorId)
  {

    foreach (Transform child in transform)
    {
      RoomDoor door = child.GetComponent<RoomDoor>();
      if (door && door.id == doorId)
        return door.exitPosition.position;
    }
    Debug.LogError("No room with id " + doorId + " found");
    return Vector2.zero;
  }
}
