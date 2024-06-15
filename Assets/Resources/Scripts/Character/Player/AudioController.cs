using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
  private Player player;
  public AudioClip jump;
  public AudioClip land;

  void Awake()
  {
    player = GetComponent<Player>();
  }

  void OnEnable()
  {
    player.playerJumpingState.OnEnter += OnJump;
  }

  void OnJump()
  {
    Debug.Log("-- AUDIO FOR JUMP --");
  }

  void OnDisable()
  {
    player.playerJumpingState.OnEnter -= OnJump;
  }
}
