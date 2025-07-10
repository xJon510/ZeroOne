using System.Collections.Generic;
using UnityEngine;

public class SFXSliderVolume : MonoBehaviour
{
    [Header("SFX Managers")]
    public List<GameObject> sfxManagers = new List<GameObject>();

    private List<AudioSource> sfxAudioSources = new List<AudioSource>();

    private MusicManager musicManager;

    void Start()
    {
        // Find the one-and-only MusicManager
        musicManager = FindObjectOfType<MusicManager>();

        if (musicManager == null)
        {
            Debug.LogError("[SFXSliderVolume] No MusicManager found!");
            return;
        }

        // Collect AudioSources from each manually assigned manager
        foreach (var manager in sfxManagers)
        {
            if (manager != null)
            {
                AudioSource source = manager.GetComponent<AudioSource>();
                if (source != null)
                {
                    sfxAudioSources.Add(source);
                }
                else
                {
                    Debug.LogWarning($"[SFXSliderVolume] No AudioSource found on {manager.name}!");
                }
            }
        }

        // Initial sync:
        UpdateSFXVolumes(musicManager.savedSFXVolume);

        // Register to slider
        if (musicManager.sfxVolumeSlider != null)
        {
            musicManager.sfxVolumeSlider.onValueChanged.AddListener(UpdateSFXVolumes);
        }
    }

    void UpdateSFXVolumes(float newVolume)
    {
        foreach (var sfxSource in sfxAudioSources)
        {
            if (sfxSource != null)
            {
                sfxSource.volume = newVolume;
            }
        }

        Debug.Log($"[SFXSliderVolume] Updated all SFX volumes to {newVolume}");
    }
}
