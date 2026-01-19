using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;

    // Liste aller AudioSources, die vom Master-Slider gesteuert werden
    public List<AudioSource> masterAudioSources = new List<AudioSource>();

    public AudioSource musicAudio;  // Musik-Spezialquelle

    void Start()
    {
        // Initiale Slider-Werte setzen
        if(masterSlider != null && masterAudioSources.Count > 0)
        {
            masterSlider.value = masterAudioSources[0].volume; // irgendeiner als Startwert
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if(musicSlider != null && musicAudio != null)
        {
            musicSlider.value = musicAudio.volume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
    }

    // Master-Slider steuert alle AudioSources in der Liste
    public void SetMasterVolume(float value)
    {
        foreach (AudioSource source in masterAudioSources)
        {
            if(source != null) source.volume = value;
        }
    }

    // Musik-Slider steuert nur Musik
    public void SetMusicVolume(float value)
    {
        if(musicAudio != null) musicAudio.volume = value;
    }
}