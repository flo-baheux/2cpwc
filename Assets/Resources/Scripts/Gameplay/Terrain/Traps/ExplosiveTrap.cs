using System.Collections;
using UnityEditor;
using UnityEngine;

public class ExplosiveTrap : Trap
{
  [Space(10f)]
  [SerializeField] private float explosionDelay = 3f;
  [SerializeField] private float explosionDuration = 1f;

  private SpriteRenderer _sprite;
  private Color _originalColor;

  private void Awake()
  {
    _sprite = GetComponent<SpriteRenderer>();
    _originalColor = _sprite.color;
  }

  public void OnTrigger() => StartCoroutine(Activate());

  public IEnumerator Activate()
  {
    float timer = 0f;
    while (timer <= explosionDelay)
    {
      timer += Time.deltaTime;
      _sprite.color = Color.Lerp(_originalColor, Color.red, Mathf.PingPong(Time.time * timer + 0.2f, 1));
      yield return null;
    }
    DealDamage();
  }

  private void DealDamage()
  {
    hitbox.gameObject.SetActive(true);
    Destroy(gameObject, explosionDuration);
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
      OnTrigger();
  }
}

#if UNITY_EDITOR
// Adds a button to the inspector for easier testing
[CustomEditor(typeof(ExplosiveTrap))]
public class ExplosiveTrapEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    ExplosiveTrap trapClass = (ExplosiveTrap)target;
    if (GUILayout.Button("Trigger the trap"))
      trapClass.OnTrigger();
  }
}
#endif