using UnityEngine;
using UnityEngine.Audio;

public class PlayerPrefsApplier : MonoBehaviour
{
  [SerializeField] private AudioMixer mixer;

  public static float defaultGlobalVolume = 1;
  public static float defaultBGMVolume = 1;
  public static float defaultSFXVolume = 1;

  void Start()
  {
    if (PlayerPrefs.HasKey("MasterVolume"))
      mixer.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume")) * 20);
    else
      mixer.SetFloat("MasterVolume", Mathf.Log10(defaultGlobalVolume) * 20);

    if (PlayerPrefs.HasKey("BGMVolume"))
      mixer.SetFloat("BGMVolume", Mathf.Log10(PlayerPrefs.GetFloat("BGMVolume")) * 20);
    else
      mixer.SetFloat("BGMVolume", Mathf.Log10(defaultBGMVolume) * 20);


    if (PlayerPrefs.HasKey("SFXVolume"))
      mixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume")) * 20);
    else
      mixer.SetFloat("SFXVolume", Mathf.Log10(defaultSFXVolume) * 20);
  }
}

