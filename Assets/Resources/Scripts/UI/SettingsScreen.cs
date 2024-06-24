using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
  [SerializeField] private GameObject startScreen;

  [SerializeField] private AudioMixer mixer;

  [SerializeField] private Slider GlobalVolumeSlider;
  [SerializeField] private Slider BGMVolumeSlider;
  [SerializeField] private Slider SFXVolumeSlider;
  [SerializeField] private Slider NarrationVolumeSlider;

  [SerializeField] private Button BackToMenuButton;

  private void OnEnable()
  {
    BackToMenuButton.Select();
  }

  void Start()
  {
    GlobalVolumeSlider.value = PlayerPrefs.HasKey("GlobalVolume") ? PlayerPrefs.GetFloat("GlobalVolume") : PlayerPrefsApplier.defaultGlobalVolume;
    BGMVolumeSlider.value = PlayerPrefs.HasKey("BGMVolume") ? PlayerPrefs.GetFloat("BGMVolume") : PlayerPrefsApplier.defaultBGMVolume;
    SFXVolumeSlider.value = PlayerPrefs.HasKey("SFXVolume") ? PlayerPrefs.GetFloat("SFXVolume") : PlayerPrefsApplier.defaultSFXVolume;
    NarrationVolumeSlider.value = PlayerPrefs.HasKey("NarrationVolume") ? PlayerPrefs.GetFloat("NarrationVolume") : PlayerPrefsApplier.defaultSFXVolume;
  }


  public void OnGlobalVolumeChanged()
  {
    PlayerPrefs.SetFloat("GlobalVolume", GlobalVolumeSlider.value);
    mixer.SetFloat("MasterVolume", Mathf.Log10(GlobalVolumeSlider.value) * 20);
  }

  public void OnBGMVolumeChanged()
  {
    PlayerPrefs.SetFloat("BGMVolume", BGMVolumeSlider.value);
    mixer.SetFloat("BGMVolume", Mathf.Log10(BGMVolumeSlider.value) * 20);
  }

  public void OnSFXVolumeChanged()
  {
    PlayerPrefs.SetFloat("SFXVolume", SFXVolumeSlider.value);
    mixer.SetFloat("SFXVolume", Mathf.Log10(SFXVolumeSlider.value) * 20);
  }

  public void OnNarrationVolumeChanged()
  {
    PlayerPrefs.SetFloat("NarrationVolume", NarrationVolumeSlider.value);
    mixer.SetFloat("NarrationVolume", Mathf.Log10(NarrationVolumeSlider.value) * 20);
  }

  public void onClickCloseSettings()
  {
    startScreen.SetActive(true);
    gameObject.SetActive(false);
  }
}
