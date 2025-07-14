using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfStatement : MonoBehaviour
{
    private BasicUpgrade upgrade;
    private CoreStats coreStats;

    private float lastPercentBitRate = 0f;
    private bool lastWasIfBonus = false;

    void Start()
    {
        upgrade = GetComponent<BasicUpgrade>();
    }

    void Update()
    {
        if (upgrade.currentLevel < 1) return;

        int level = upgrade.currentLevel;

        // === Calculate total storage ratio ===
        ulong totalStored = 0;
        ulong totalCapacity = 0;

        foreach (var grid in BitManager.Instance.activeGrids)
        {
            totalStored += grid.GetLocalBitValue();
            totalCapacity += grid.GetBitCapacity();
        }

        float storageRatio = (totalCapacity == 0) ? 0f : (float)totalStored / totalCapacity;
        float storagePercent = storageRatio * 100f;

        float ifThreshold = GetThreshold(level);
        float ifBonusPerLevel = GetIfBonus(level);
        float elseBonusPerLevel = GetElseBonus(level);

        float percentPerLevel = (storagePercent < ifThreshold) ? ifBonusPerLevel : elseBonusPerLevel;
        bool isIfBonusNow = (storagePercent < ifThreshold);
        float newPercentBitRate = percentPerLevel * level;

        if (!Mathf.Approximately(newPercentBitRate, lastPercentBitRate))
        {
            float delta = newPercentBitRate - lastPercentBitRate;
            CoreStats.Instance.AddStat("PercentBitRate", delta);
            lastPercentBitRate = newPercentBitRate;

            // Debug.Log($"[IfStatement] Storage: {storagePercent:F1}% | IF < {ifThreshold}% -> +{percentPerLevel}%/Lvl | Total Bonus: {newPercentBitRate}%");
        }

        if (isIfBonusNow != lastWasIfBonus)
        {
            string stateText = isIfBonusNow ? $"IF < {ifThreshold}% Applied" : $"ELSE >= {ifThreshold}% Applied";

            LogPrinter.Instance?.PrintLog($"If Statement Bonus: {stateText}",BranchType.CPU);

            lastWasIfBonus = isIfBonusNow;
        }
    }

    private float GetThreshold(int level)
    {
        if (level >= 50) return 70f;
        if (level >= 5) return 50f;
        return 30f;
    }

    private float GetIfBonus(int level)
    {
        if (level >= 100) return 0.5f;
        if (level >= 25) return 0.25f;
        return 0.1f;
    }

    private float GetElseBonus(int level)
    {
        if (level >= 100) return 0.2f;
        if (level >= 25) return 0.1f;
        return 0.05f;
    }
}
