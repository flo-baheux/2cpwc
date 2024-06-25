using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
  [SerializeField] private AudioSource SFXAudioSource;
  [SerializeField] private AudioSource SubtleSFXAudioSource;

  private Player player;

  public AudioClip jump;
  public AudioClip fall;
  public AudioClip land;
  public List<AudioClip> takeDamage;
  public List<AudioClip> vocalize;
  public AudioClip death;

  void Awake()
  {
    player = GetComponent<Player>();
  }

  // As the component only listen to events related to its own gameobject
  // it's fine to have no unsubscribe method and sub during Start()
  void Start()
  {
    player.state.jumpingState.OnEnter += OnJump;
    player.state.groundedState.OnEnter += OnLand;
    player.state.deadState.OnEnter += OnDeath;
    player.health.PlayerHealthChanged += OnPlayerHealthChanged;
    player.OnInteract += OnInteract;
  }

  void OnJump(Player player) => SubtleSFXAudioSource.PlayOneShot(jump);
  void OnLand(Player player) => SubtleSFXAudioSource.PlayOneShot(land);
  void OnDeath(Player player) => SFXAudioSource.PlayOneShot(death);

  void OnPlayerHealthChanged(int hpBefore, int hpAfter)
  {
    if (hpAfter < hpBefore)
      SFXAudioSource.PlayOneShot(takeDamage.OrderBy(n => Guid.NewGuid()).ToArray()[0]);
  }

  void OnInteract(Player player)
  {
    if (!player.canInteractWithSomething())
      SFXAudioSource.PlayOneShot(vocalize.OrderBy(n => Guid.NewGuid()).ToArray()[0]);
  }
}
