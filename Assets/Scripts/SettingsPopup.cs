using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPopup : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider SFXSlider;

    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 0.5f);
        SFXSlider.value = PlayerPrefs.GetFloat("sfx", 0.5f);
        AudioListener.volume = volumeSlider.value;
        AudioListener.volume = SFXSlider.value;
    }

    public void OnVolumeValue(float vol)
    {
        Debug.Log($"Volume: {vol}");
        PlayerPrefs.SetFloat("volume", vol);
        AudioListener.volume = vol;
    }

    public void OnSFXValue(float sfx)
    {
        Debug.Log($"Volume: {sfx}");
        PlayerPrefs.SetFloat("sfx", sfx);
        AudioListener.volume = sfx;
    }
}