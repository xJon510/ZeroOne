using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootSFX : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip normalLineClip;
    public AudioClip oneLinerClip;
    public AudioClip bootCompleteClip;

    public void PlayNormalLine()
    {
        if (audioSource != null && normalLineClip != null)
        {
            audioSource.PlayOneShot(normalLineClip);
        }
    }

    public void PlayOneLiner()
    {
        if (audioSource != null && oneLinerClip != null)
        {
            audioSource.PlayOneShot(oneLinerClip);
        }
    }

    public void PlayBootComplete()
    {
        if (audioSource != null && bootCompleteClip != null)
        {
            audioSource.PlayOneShot(bootCompleteClip);
        }
    }
}