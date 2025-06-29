using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BitReceiver : MonoBehaviour
{
    [SerializeField] private TMP_Text totalBitsText;
    [SerializeField] private TMP_Text bitRateText;
    [SerializeField] private TMP_Text overflowText;

    private void Update()
    {
        if (BitManager.Instance == null) return;

        // Format and display currentBits (e.g., 1,024 or 1.02K)
        totalBitsText.text = $"Bits: {FormatNumber(BitManager.Instance.currentBits)}";

        float flat = CoreStats.Instance.GetStat("FlatBitRate");
        float swept = CoreStats.Instance.GetStat("SweptCache");
        float percent = CoreStats.Instance.GetStat("PercentBitRate") / 100f;

        float backendGlobal = (flat + swept) * (1 + percent);

        int activeGrids = BitManager.Instance.GetActiveGridCount();
        int totalGrids = BitManager.Instance.activeGrids.Count;

        // Simulate the same splitting your backend does:
        float producingBitRate = (activeGrids > 0)
            ? backendGlobal / totalGrids * activeGrids
            : 0f;

        int missingGrids = totalGrids - activeGrids;

        float lostCapacity = (backendGlobal / totalGrids) * missingGrids;

        float overflowPercent = CoreStats.Instance.GetStat("% Overflow") / 100f;

        float overflowBonus = lostCapacity * overflowPercent;

        float finalBitRate = producingBitRate + overflowBonus;

        bitRateText.text = $"Bit Rate: {finalBitRate:F1} b/s\n({activeGrids}/{totalGrids})";

        if (overflowText != null)
        {
            float overflow = BitManager.Instance.GetOverflowBits();
            overflowText.text = $"Overflow: {overflow:F0}";
        }
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
