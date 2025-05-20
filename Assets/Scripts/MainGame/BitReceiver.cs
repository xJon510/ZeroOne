using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BitReceiver : MonoBehaviour
{
    [SerializeField] private TMP_Text totalBitsText;
    [SerializeField] private TMP_Text bitRateText;

    private void Update()
    {
        if (BitManager.Instance == null) return;

        // Format and display currentBits (e.g., 1,024 or 1.02K)
        totalBitsText.text = $"Total Bits: {FormatNumber(BitManager.Instance.currentBits)}";

        // Dynamically adjust bitrate display based on active grids
        int activeGrids = BitManager.Instance.GetActiveGridCount();
        int totalGrids = BitManager.Instance.activeGrids.Count;

        float actualBitRate = (activeGrids > 0)
            ? BitManager.Instance.globalBitRate / totalGrids * activeGrids
            : 0f;

        bitRateText.text = $"Bit Rate: {actualBitRate:F1} b/s\n({activeGrids}/{totalGrids})";
    }

    private string FormatNumber(ulong value)
    {
        // Simple formatting to keep numbers readable
        if (value >= 1_000_000_000)
            return (value / 1_000_000_000f).ToString("0.00") + "B";
        if (value >= 1_000_000)
            return (value / 1_000_000f).ToString("0.00") + "M";
        if (value >= 1_000)
            return (value / 1_000f).ToString("0.00") + "K";
        return value.ToString("N0"); // with comma separators
    }
}
