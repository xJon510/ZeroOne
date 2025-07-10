using UnityEngine;
using UnityEngine.UI;

public class CantAffordSFX : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip cantAffordClip;
    public AudioClip buttonDownClip;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("Upgrade Button")]
    public Button mainUpgradeButton;

    void Awake()
    {
        if (sfxSource == null)
        {
            Debug.LogWarning("[CantAffordSFX] No AudioSource assigned — please assign one in the Inspector!");
        }
    }

    public void PlayCantAfford()
    {
        if (sfxSource != null && cantAffordClip != null)
        {
            sfxSource.pitch = Random.Range(0.9f, 1.0f);
            sfxSource.PlayOneShot(cantAffordClip);
        }
    }

    public void PlayNormalClick()
    {
        if (sfxSource != null && buttonDownClip != null)
        {
            sfxSource.pitch = Random.Range(0.9f, 1.0f);
            sfxSource.PlayOneShot(buttonDownClip);
        }
    }
}