using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class BitGridManager : MonoBehaviour
{
    private TMP_Text[] bitCharacters; // Should be size = gridWidth * gridHeight, ordered L to R, top to bottom
    private int maxCapacity => bitCharacters.Length;

    private ulong localBitValue = 0;
    private float internalBitProgress = 0f; // To handle fractional bit accumulation

    private Coroutine bitAnimRoutine;

    private Func<float> bitrateSource;

    [Header("Animation Settings")]
    public float bitStepDelay = 0.02f; // delay per bit increment
    public float waveStaggerDelay = 0.005f; // delay per character in ripple

    private float animTimer = 0f;
    public float maxVisualRefreshRate = 0.2f;

    public TMP_Text debugText;

    private void Start()
    {
        // Automatically gather all TMP_Texts under this object
        bitCharacters = GetComponentsInChildren<TMP_Text>();

        UpdateVisuals(); // Start clean
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
}
