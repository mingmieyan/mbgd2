using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SoundVolSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider SFXSlider;
    public float currentMusicVol;
    public float currentSFXVol;
    public Toggle muteToggle;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            loadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    void Update()
    {
        currentMusicVol = MusicSlider.value;
        currentSFXVol = SFXSlider.value;

        if (muteToggle.isOn == true)
        {
            myMixer.SetFloat("Music", Mathf.Log10(0.0000001f) * 20);
            myMixer.SetFloat("SFX", Mathf.Log10(0.0000001f) * 20);
        }
        else
        {
            myMixer.SetFloat("Music", Mathf.Log10(currentMusicVol) * 20);
            myMixer.SetFloat("SFX", Mathf.Log10(currentSFXVol) * 20);
        }
    }

    public void SetMusicVolume()    //change volume of music
    {
        float volume = MusicSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume()    //change volume of sfx
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void loadVolume()   //back to game, the sound volume maintain before leaving game at last time
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        SetMusicVolume();
        SetSFXVolume();
    }
}
