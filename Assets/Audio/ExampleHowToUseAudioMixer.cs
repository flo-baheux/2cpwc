using UnityEngine;
using UnityEngine.Audio;

public class ExampleHowToUseAudioMixer : MonoBehaviour
{
  // You can retrieve an audiomixer this way (assigned in the editor)
  [SerializeField] private AudioMixer mixer;

  /*
  ** In AudioMixer, you can access to "MasterVolume", "BGMVolume" and "SFXVolume"
  ** Those 3 volume values have been exposed to scripts.
  ** Inside Unity Editor if you create an Audio Source, you can set "Output" to one of those 3
  */

  void Start()
  {
    /* Mixers are using log10 values - this is because it uses decibels units.
    ** You need to convert dB to float!` It's confusing but eh :shrug:
    ** It's not linear, so if you hook the value to a slider, set minValue to 0.0001 and max to 1
    ** If you want to set a volume via script, you can do it this way:
    */
    mixer.SetFloat("MasterVolume", Mathf.Log10(1f /* 0 to 1 as float */) * 20);
  }
}
