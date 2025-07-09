using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MuteTextColor : MonoBehaviour
{
    [Header("References")]
    public Slider volumeSlider;        
    public TMP_Text targetText;

    [Header("Colors")]
    private Color normalColor;
    public Color mutedColor = Color.red;

    void Start()
    {
        if (targetText != null)
        {
            normalColor = targetText.color;

            // Hook up listener
            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
                OnVolumeChanged(volumeSlider.value);
            }
        }
    }

    void OnVolumeChanged(float value)
    {
        if (targetText != null)
        {
            if (value <= 0.001f)  // Tiny buffer for float imprecision
                targetText.color = mutedColor;
            else
                targetText.color = normalColor;
        }
    }
}
