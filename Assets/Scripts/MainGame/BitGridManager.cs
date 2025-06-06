using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BitGridManager : MonoBehaviour
{
    private TMP_Text[] bitCharacters; // Should be size = gridWidth * gridHeight, ordered L to R, top to bottom
    private int maxCapacity => bitCharacters.Length;

    private ulong localBitValue = 0;
    private ulong localBitMax;
    private float internalBitProgress = 0f; // To handle fractional bit accumulation

    private Coroutine bitAnimRoutine;

    private Func<float> bitrateSource;

    [Header("Animation Settings")]
    public float bitStepDelay = 0.02f; // delay per bit increment
    public float waveStaggerDelay = 0.005f; // delay per character in ripple

    private float animTimer = 0f;
    public float maxVisualRefreshRate = 0.2f;

    public TMP_Text debugText;

    private Image borderImage;
    private TMP_Text[] bitTexts;

    private static readonly Color BorderGreen = new Color32(0x00, 0xDC, 0x36, 0xFF);
    private static readonly Color BorderRed = new Color32(0xDC, 0x1E, 0x00, 0xFF);

    private static readonly Color TextGreen = new Color32(0x6D, 0xD2, 0x7A, 0xFF);
    private static readonly Color TextRed = new Color32(0xD2, 0x73, 0x6E, 0xFF);

    private void Start()
    {
        // Automatically gather all TMP_Texts under this object
        bitCharacters = GetComponentsInChildren<TMP_Text>();
        bitTexts = bitCharacters; // optional alias
        borderImage = GetComponent<Image>();

        UpdateVisuals(); // Start clean

        localBitMax = (ulong)Mathf.Pow(2, maxCapacity) - 1;
    }

    private void Update()
    {
        if (bitrateSource == null) return;

        float bitRate = bitrateSource.Invoke();
        internalBitProgress += bitRate * Time.deltaTime;

        while (internalBitProgress >= 1f && GetBitLength(localBitValue + 1) <= maxCapacity)
        {
            localBitValue += 1;
            internalBitProgress -= 1;

            UpdateDebugText();
        }

        animTimer += Time.deltaTime;
        if (animTimer >= maxVisualRefreshRate)
        {
            animTimer = 0f;

            if (bitAnimRoutine != null)
            {
                StopCoroutine(bitAnimRoutine);
            }

            bitAnimRoutine = StartCoroutine(BitWaveVisualUpdate());
        }
    }

    private void UpdateDebugText()
    {
        if (debugText != null)
        {
            debugText.text = Mathf.FloorToInt(localBitValue).ToString();
        }
    }

    private IEnumerator BitWaveVisualUpdate()
    {
        string binary = Convert.ToString((long)localBitValue, 2);
        int bitCount = binary.Length;

        for (int i = 0; i < bitCharacters.Length; i++)
        {
            if (i < bitCount)
            {
                char bit = binary[bitCount - 1 - i];
                bitCharacters[i].text = bit.ToString();
            }
            else
            {
                bitCharacters[i].text = "0";
            }

            yield return new WaitForSeconds(waveStaggerDelay);
        }

        float fillRatio = (float)localBitValue / localBitMax;
        float adjustedRatio = Mathf.InverseLerp(0.5f, 1f, fillRatio);

        Color borderColor = Color.Lerp(BorderGreen, BorderRed, adjustedRatio); // or red-to-green
        Color textColor = Color.Lerp(TextGreen, TextRed, adjustedRatio);     // tweak to your style

        if (borderImage != null)
            borderImage.color = borderColor;

        foreach (TMP_Text txt in bitTexts)
            txt.color = textColor;
    }

    private void UpdateVisuals()
    {
        string binary = Convert.ToString((long)localBitValue, 2);
        int bitCount = binary.Length;

        for (int i = 0; i < bitCharacters.Length; i++)
        {
            bitCharacters[i].text = "0";
        }

        for (int i = 0; i < bitCount && i < bitCharacters.Length; i++)
        {
            char bit = binary[bitCount - 1 - i];
            bitCharacters[i].text = bit.ToString();
        }

    }

    private int GetBitLength(ulong value)
    {
        if (value == 0) return 0;
        int length = 0;
        while (value > 0)
        {
            value >>= 1;
            length++;
        }
        return length;
    }

    public ulong GetLocalBitValue()
    {
        return localBitValue;
    }

    void OnEnable()
    {
        BitManager.OnBitrateChanged += HandleBitrateUpdate;
    }

    void OnDisable()
    {
        BitManager.OnBitrateChanged -= HandleBitrateUpdate;
    }

    private void HandleBitrateUpdate(float newGlobalRate)
    {
        SetBitrateSource(() => newGlobalRate / BitManager.Instance.activeGrids.Count);
    }

    public void SetBitrateSource(Func<float> source)
    {
        bitrateSource = source;
    }

    public bool IsAtGridCapacity()
    {
        return GetBitLength(localBitValue + 1) > maxCapacity;
    }

    private int CountSetBits(ulong value)
    {
        int count = 0;
        while (value != 0)
        {
            count += (int)(value & 1);
            value >>= 1;
        }
        return count;
    }

    public void RefreshBitCharacters()
    {
        bitCharacters = GetComponentsInChildren<TMP_Text>();
        bitTexts = bitCharacters;
        localBitMax = (ulong)Mathf.Pow(2, bitCharacters.Length) - 1;
    }

    public ulong GetBitCapacity()
    {
        return (ulong)Mathf.Pow(2, maxCapacity) - 1;
    }
}
