using UnityEngine;

public class CharacterScriptPlaceholder : MonoBehaviour
{
  public CharacterHealthComponent health = new();

  void OnEnable()
  {
    health.CharacterHealthChanged += OnCharacterHealthChanged;
  }

  // Update is called once per frame
  void Update()
  {
    // Example how to use it inside Update without observer pattern
    if (health.currentHealth == 0)
    {
      GetComponent<SpriteRenderer>().color = Color.red;
    }
  }

  // Example how to use it with observer pattern
  void OnCharacterHealthChanged(int currentHealth)
  {
    Debug.Log("Event received - Character health changed! New value = " + currentHealth);
  }

  void OnDisable()
  {
    health.CharacterHealthChanged -= OnCharacterHealthChanged;
  }
}
