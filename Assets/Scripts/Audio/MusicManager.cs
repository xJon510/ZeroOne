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

    [Header("Volume Memory")]
    public float savedMusicVolume = 0.5f;
    public float savedSFXVolume = 0.5f;

    [Header("Playlist Settings")]
    public List<AudioClip> playlist = new List<AudioClip>();
    private List<AudioClip> originalPlaylist = new List<AudioClip>();
    public AudioSource musicSource; // drag in Inspector!
    public int currentSongIndex = 0;

    [Header("Playback Toggles")]
    public bool repeatSong = false;
    public bool shuffleSongs = false;
    public bool isPaused = false;

    private List<int> shufflePool = new List<int>();

    void Awake()
    {
        if (FindObjectsOfType<MusicManager>().Length > 1)
        {
            Debug.Log("Duplicate MusicManager found — destroying this one.");
            Destroy(gameObject);
            return;
        }
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
        UpdatePlayingObjects();
    }

    void Update()
    {
        if (musicSource != null && !musicSource.isPlaying && musicSource.clip != null && !isPaused)
        {
            if (repeatSong)
            {
                musicSource.Play(); // replay same song
            }
            else
            {
                PlayNextSong();
            }
        }
    }

    void PlayNextSong()
    {
        if (shuffleSongs)
        {
            // If pool is empty or null, refill it with all possible indexes EXCEPT the current one
            if (shufflePool == null || shufflePool.Count == 0)
            {
                shufflePool = new List<int>();
                for (int i = 0; i < playlist.Count; i++)
                {
                    if (i != currentSongIndex) // optional: avoid repeating same song immediately
                        shufflePool.Add(i);
                }
            }

            // Pick a random index from the pool
            int randomIndexInPool = Random.Range(0, shufflePool.Count);
            int nextIndex = shufflePool[randomIndexInPool];

            // Remove it so we don't play it again until the pool resets
            shufflePool.RemoveAt(randomIndexInPool);

            currentSongIndex = nextIndex;
        }
        else
        {
            currentSongIndex++;
            if (currentSongIndex >= playlist.Count) currentSongIndex = 0;
        }

        musicSource.clip = playlist[currentSongIndex];
        musicSource.Play();

        UpdatePlayingObjects();
    }

    public void PlayFirstSong()
    {
        if (playlist.Count > 0 && musicSource != null)
        {
            Debug.Log("Playing First Song");
            musicSource.clip = playlist[0];

            OnMusicVolumeChanged(savedMusicVolume);

            musicSource.Play();
            currentSongIndex = 0;

            savedMusicVolume = 0.5f;
            savedSFXVolume = 0.5f;

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

            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = savedMusicVolume;
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = savedSFXVolume;
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
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

        if (shuffleSongs)
        {
            shufflePool = new List<int>();
            for (int i = 0; i < playlist.Count; i++)
            {
                if (i != currentSongIndex)
                    shufflePool.Add(i);
            }
        }

        if (FindObjectOfType<MusicManagerHelper>() is MusicManagerHelper helper && helper.shuffleButtonText != null)
        {
            helper.shuffleButtonText.text = $"Shuffle: [{(shuffleSongs ? "ON" : "OFF")}]";
        }
    }

    public void Play()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
            isPaused = false;
            Debug.Log("Playback resumed");
        }
    }

    public void Pause()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
            isPaused = true;
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
        if (musicSource != null)
        {
            if (musicSource.time > 10f)
            {
                // If we're more than 10s in, restart the same song
                musicSource.time = 0f;
                Debug.Log("Rewinding to start of current song");
            }
            else
            {
                // If we're less than 10s in, go to the previous song
                Debug.Log("Rewinding to previous song");

                currentSongIndex--;

                if (currentSongIndex < 0)
                    currentSongIndex = playlist.Count - 1;

                musicSource.clip = playlist[currentSongIndex];
                musicSource.Play();

                UpdatePlayingObjects();
            }
        }
    }

    public void PlaySpecificSong(int index)
    {
        if (index >= 0 && index < playlist.Count && musicSource != null)
        {
            currentSongIndex = index;
            musicSource.clip = playlist[currentSongIndex];
            musicSource.Play();

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

    public void OnMusicVolumeChanged(float newValue)
    {
        savedMusicVolume = newValue;

        if (musicSource != null)
            musicSource.volume = savedMusicVolume;

        Debug.Log($"Music volume changed to: {savedMusicVolume}");
    }

    public void OnSFXVolumeChanged(float newValue)
    {
        savedSFXVolume = newValue;

        Debug.Log($"SFX volume changed to: {savedSFXVolume}");
    }
}
