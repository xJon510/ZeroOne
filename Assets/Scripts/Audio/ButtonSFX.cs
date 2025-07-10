using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip buttonDownClip;

    [Header("Buttons to Attach SFX")]
    public List<Button> buttons = new();

    [Header("Audio Source")]
    public AudioSource sfxSource;

    void Awake()
    {
        if (sfxSource == null)
        {
            Debug.LogWarning("[ButtonSFX] No AudioSource assigned — please assign one in the Inspector!");
        }

        foreach (var button in buttons)
        {
            if (button != null)
            {
                // Add a pointer down listener
                var trigger = button.gameObject.AddComponent<ButtonPressListener>();
                trigger.buttonSFX = this;
            }
        }
    }

    public void PlayButtonDown()
    {
        if (sfxSource != null && buttonDownClip != null)
        {
            sfxSource.pitch = Random.Range(0.9f, 1.0f);
            sfxSource.PlayOneShot(buttonDownClip);
        }
    }
}
