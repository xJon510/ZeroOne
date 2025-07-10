using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoSlotPop : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip errorClip;
    public AudioClip buttonDownClip;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("Button")]
    public Button errorButton;

    void Awake()
    {
        if (sfxSource == null)
        {
            Debug.LogWarning("[ErrorSFX] No AudioSource assigned — please assign one in the Inspector!");
        }

        if (errorButton != null)
        {
            errorButton.onClick.AddListener(PlayBasedOnSlot);
        }
    }

    public void PlayError()
    {
        if (sfxSource != null && errorClip != null)
        {
            sfxSource.PlayOneShot(errorClip);
        }
    }

    public void PlayOriginalClick()
    {
        if (sfxSource != null && buttonDownClip != null)
        {
            sfxSource.PlayOneShot(buttonDownClip);
        }
    }

    public void PlayBasedOnSlot()
    {
        int selectedSlot = PlayerPrefs.GetInt("SelectedSaveSlot", -1);
        if (selectedSlot == -1)
        {
            PlayError();
        }
        else
        {
            PlayOriginalClick();
        }
    }
}
