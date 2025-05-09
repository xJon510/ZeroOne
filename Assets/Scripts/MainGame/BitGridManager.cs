using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BitGridManager : MonoBehaviour
{
    private TMP_Text[] bitCharacters; // Should be size = gridWidth * gridHeight, ordered L to R, top to bottom

    private List<bool> bitStates = new List<bool>();
    private int maxCapacity => bitCharacters.Length;

    private ulong localBitValue = 0;

    private float internalBitProgress = 0f; // To handle fractional bit accumulation

    private void Start()
    {
        // Automatically gather all TMP_Texts under this object
        bitCharacters = GetComponentsInChildren<TMP_Text>();

        // Initialize bitStates to all false (0)
        for (int i = 0; i < maxCapacity; i++)
            bitStates.Add(false);

        UpdateVisuals();
    }

    public void ReceiveBits(float bitAmount)
    {
        internalBitProgress += bitAmount;

        while (internalBitProgress >= 1f && GetBitLength(localBitValue + 1) <= maxCapacity)
        {
            localBitValue += 1;
            internalBitProgress -= 1f;
        }

        UpdateVisuals();
    }

    private void SetNextBit(bool value)
    {
        int index = GetFilledBitCount();
        if (index < maxCapacity)
            bitStates[index] = value;
    }

    private int GetFilledBitCount()
    {
        int count = 0;
        foreach (bool bit in bitStates)
        {
            if (bit) count++;
            else break; // stop at first false to simulate dense little-endian fill
        }
        return count;
    }

    private void UpdateVisuals()
    {
        string binary = System.Convert.ToString((long)localBitValue, 2); // e.g. "101"
        int bitCount = binary.Length;

        // Clear all first
        for (int i = 0; i < bitCharacters.Length; i++)
            bitCharacters[i].text = "0";

        // Write bits from right to left (little-endian)
        for (int i = 0; i < bitCount && i < bitCharacters.Length; i++)
        {
            char bit = binary[bitCount - 1 - i]; // reverse to get little-endian
            bitCharacters[i].text = bit.ToString();
        }
    }

    private int GetBitLength(ulong value)
    {
        if (value == 0) return 1;
        int length = 0;
        while (value > 0)
        {
            value >>= 1;
            length++;
        }
        return length;
    }

    public int GetTotalBits() => bitStates.FindAll(b => b).Count;
}
