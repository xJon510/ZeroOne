
using UnityEngine;

public class CycleTrim : MonoBehaviour
{
    private BasicUpgrade upgrade;
    private CoreStats coreStats;

    private float lastCPUDiscount = 0f;
    private float lastFlatBitRate = 0f;

    void Awake()
    {
        upgrade = GetComponent<BasicUpgrade>();
    }

    void Update()
    {
        if (upgrade.currentLevel < 1) return;

        int level = upgrade.currentLevel;

        // === Flat BitRate/Lvl ===
        float flatBitRatePerLevel = GetFlatBitRatePerLevel(level);
        float newFlatBitRate = flatBitRatePerLevel * level;

        //Debug.Log($"[CycleTrim] flatBitRate increase: {newFlatBitRate}");

        if (!Mathf.Approximately(newFlatBitRate, lastFlatBitRate))
        {
            float delta = newFlatBitRate - lastFlatBitRate;
            CoreStats.Instance.AddStat("FlatBitRate", delta);
            lastFlatBitRate = newFlatBitRate;
        }

        // === CPU Discount ===
        int newMaxDiscount = GetMaxDiscount(level);
        float newDiscount = Mathf.Min(level, newMaxDiscount);

        //Debug.Log($"[CycleTrim] Discount increases: {newDiscount}");

        if (!Mathf.Approximately(newDiscount, lastCPUDiscount))
        {
            float delta = newDiscount - lastCPUDiscount;
            CoreStats.Instance.AddStat("CPU Discount", delta, StatBranch.CPU);
            lastCPUDiscount = newDiscount;
        }
    }

    private float GetFlatBitRatePerLevel(int level)
    {
        if (level >= 100) return 1f;     // Level 100 milestone
        if (level >= 50) return 0.2f;    // Level 50 milestone
        if (level >= 5) return 0.1f;     // Level 5 milestone
        return 0f;                       // Below Level 5
    }

    private int GetMaxDiscount(int level)
    {
        if (level >= 100) return 50;
        if (level >= 50) return 40;
        if (level >= 25) return 35;
        return 25;  // Base
    }

}