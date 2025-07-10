using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootSFX : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip normalLineClip;
    public AudioClip oneLinerClip;
    public AudioClip bootCompleteClip;

    private static bool bootCompletePlayed = false;

    public void PlayNormalLine()
    {
        if (audioSource != null && normalLineClip != null)
        {
            audioSource.pitch = Random.Range(0.90f, 1f);
            audioSource.PlayOneShot(normalLineClip);
        }
    }

    public void PlayOneLiner()
    {
        if (audioSource != null && oneLinerClip != null)
        {
            audioSource.pitch = Random.Range(0.90f, 1f);
            audioSource.PlayOneShot(oneLinerClip);
        }
    }

    public void PlayBootComplete()
    {
        if (bootCompletePlayed) return;

        if (audioSource != null && bootCompleteClip != null)
        {
            audioSource.pitch = 0.9f;
            audioSource.PlayOneShot(bootCompleteClip);
        }

        bootCompletePlayed = true;
    }

    public static void ResetBootCompleteFlag()
    {
        bootCompletePlayed = false;
    }
}