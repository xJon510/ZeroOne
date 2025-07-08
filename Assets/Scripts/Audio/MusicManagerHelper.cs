using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicManagerHelper : MonoBehaviour
{
    [Header("Volume Controls")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Playback Buttons")]
    public Button playButton;
    public Button pauseButton;
    public Button skipButton;
    public Button rewindButton;

    [Header("Playback Toggles")]
    public Button repeatButton;
    public Button shuffleButton;
    public TMP_Text repeatButtonText;
    public TMP_Text shuffleButtonText;

    [Header("Song Buttons")]
    public List<Button> songButtons = new List<Button>();

    [Header("Now Playing Objects")]
    public List<GameObject> playingObjects = new List<GameObject>();
}
