using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [Header("Volume Controls")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Playlist Settings")]
    public List<AudioClip> playlist = new List<AudioClip>();
    private List<AudioClip> originalPlaylist = new List<AudioClip>();
    public AudioSource audioSource; // drag in Inspector!
    public int currentSongIndex = 0;

    [Header("Playback Toggles")]
    public bool repeatSong = false;
    public bool shuffleSongs = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (originalPlaylist.Count == 0 && playlist.Count > 0)
        {
            originalPlaylist = new List<AudioClip>(playlist);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryFindHelper();
    }

    void Update()
    {
        if (audioSource != null && !audioSource.isPlaying && audioSource.clip != null)
        {
            if (repeatSong)
            {
                audioSource.Play(); // replay same song
            }
            else
            {
                PlayNextSong();
            }
        }
    }

    void PlayNextSong()
    {
        currentSongIndex++;
        if (currentSongIndex >= playlist.Count) currentSongIndex = 0;

        audioSource.clip = playlist[currentSongIndex];
        audioSource.Play();

        UpdatePlayingObjects();
    }



    public void PlayFirstSong()
    {
        if (playlist.Count > 0 && audioSource != null)
        {
            Debug.Log("Playing First Song");
            audioSource.clip = playlist[0];
            audioSource.Play();
            currentSongIndex = 0;

            UpdatePlayingObjects();
        }
    }

    public void TryFindHelper()
    {
        MusicManagerHelper helper = FindObjectOfType<MusicManagerHelper>();
        if (helper != null)
        {
            // Sliders
            musicVolumeSlider = helper.musicVolumeSlider;
            sfxVolumeSlider = helper.sfxVolumeSlider;

            // Buttons
            if (helper.playButton != null)
                helper.playButton.onClick.AddListener(Play);

            if (helper.pauseButton != null)
                helper.pauseButton.onClick.AddListener(Pause);

            if (helper.skipButton != null)
                helper.skipButton.onClick.AddListener(Skip);

            if (helper.rewindButton != null)
                helper.rewindButton.onClick.AddListener(Rewind);

            // Toggles
            if (helper.repeatButton != null)
                helper.repeatButton.onClick.AddListener(ToggleRepeat);

            if (helper.repeatButtonText != null)
                helper.repeatButtonText.text = $"Repeat Song: [{(repeatSong ? "ON" : "OFF")}]";

            if (helper.shuffleButton != null)
                helper.shuffleButton.onClick.AddListener(ToggleShuffle);

            if (helper.shuffleButtonText != null)
                helper.shuffleButtonText.text = $"Shuffle: [{(shuffleSongs ? "ON" : "OFF")}]";

            if (helper.songButtons != null && helper.songButtons.Count > 0)
            {
                for (int i = 0; i < helper.songButtons.Count; i++)
                {
                    int capturedIndex = i; // capture for closure!
                    helper.songButtons[i].onClick.AddListener(() => PlaySpecificSong(capturedIndex));
                }
            }

            Debug.Log("MusicManagerHelper found & UI connected!");
        }
        else
        {
            Debug.Log("MusicManagerHelper NOT found & UI NOT connected!");
        }
    }

    public void ToggleRepeat()
    {
        repeatSong = !repeatSong;
        Debug.Log($"Repeat toggled: {repeatSong}");

        if (FindObjectOfType<MusicManagerHelper>() is MusicManagerHelper helper && helper.repeatButtonText != null)
        {
            helper.repeatButtonText.text = $"Repeat Song: [{(repeatSong ? "ON" : "OFF")}]";
        }
    }

    public void ToggleShuffle()
    {
        shuffleSongs = !shuffleSongs;
        Debug.Log($"Shuffle toggled: {shuffleSongs}");

        if (FindObjectOfType<MusicManagerHelper>() is MusicManagerHelper helper && helper.shuffleButtonText != null)
        {
            helper.shuffleButtonText.text = $"Shuffle: [{(shuffleSongs ? "ON" : "OFF")}]";
        }

        if (shuffleSongs)
        {
            ShufflePlaylist();
        }
        else
        {
            RestoreOriginalPlaylist();
        }
    }

    private void ShufflePlaylist()
    {
        Debug.Log("Shuffling playlist!");
        for (int i = playlist.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (playlist[i], playlist[j]) = (playlist[j], playlist[i]);
        }
    }

    private void RestoreOriginalPlaylist()
    {
        Debug.Log("Restoring original playlist order!");
        playlist = new List<AudioClip>(originalPlaylist);

        // Keep the current song playing
        AudioClip currentClip = audioSource.clip;
        currentSongIndex = playlist.IndexOf(currentClip);
        if (currentSongIndex == -1) currentSongIndex = 0;
    }

    public void Play()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log("Playback resumed");
        }
    }

    public void Pause()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
            Debug.Log("Playback paused");
        }
    }

    public void Skip()
    {
        Debug.Log("Skipping to next song");
        PlayNextSong();
    }

    public void Rewind()
    {
        if (audioSource != null)
        {
            if (audioSource.time > 10f)
            {
                // If we're more than 10s in, restart the same song
                audioSource.time = 0f;
                Debug.Log("Rewinding to start of current song");
            }
            else
            {
                // If we're less than 10s in, go to the previous song
                Debug.Log("Rewinding to previous song");

                currentSongIndex--;

                if (currentSongIndex < 0)
                    currentSongIndex = playlist.Count - 1;

                audioSource.clip = playlist[currentSongIndex];
                audioSource.Play();

                UpdatePlayingObjects();
            }
        }
    }

    public void PlaySpecificSong(int index)
    {
        if (index >= 0 && index < playlist.Count && audioSource != null)
        {
            currentSongIndex = index;
            audioSource.clip = playlist[currentSongIndex];
            audioSource.Play();

            Debug.Log($"Now playing: {playlist[currentSongIndex].name}");
            UpdatePlayingObjects();
        }
        else
        {
            Debug.LogWarning("Invalid song index!");
        }
    }

    private void UpdatePlayingObjects()
    {
        if (FindObjectOfType<MusicManagerHelper>() is MusicManagerHelper helper && helper.playingObjects != null)
        {
            for (int i = 0; i < helper.playingObjects.Count; i++)
            {
                helper.playingObjects[i].SetActive(i == currentSongIndex);
            }
        }
    }
}
