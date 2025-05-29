using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum CoreStatType
{
    RawBitRate,
    BonusBitRatePercent,
    // Future stat keys...
}

public class BasicUpgrade : MonoBehaviour
{
    public UpgradeInfo upgradeInfo;
    public string upgradeName;
    public CoreStatType statToModify;
    public float baseCost = 8f;
    public float linearRate = 5f;
    public float quadraticRate = 0.5f;

    public float statGainPerLevel = 1f; // Raw +1 per level, or +1% per level for percent stats
    public int currentLevel = 0;

    public float GetUpgradeCost(int level)
    {
        return baseCost + linearRate * level + quadraticRate * level * level;
    }

    public void ApplyUpgrade(CoreStats coreStats)
    {
        currentLevel++;

        float value = statGainPerLevel;

        // Add stat to Core
        coreStats.AddStat(statToModify.ToString(), value);

        // Update UpgradeInfo UI values
        if (upgradeInfo != null)
        {
            upgradeInfo.currentLevel = currentLevel;
            upgradeInfo.upgradeCost = GetUpgradeCost(currentLevel + 1); // Cost of next level
            upgradeInfo.passiveEffect = statGainPerLevel * (currentLevel); // total effect
        }

        UnityEngine.Debug.Log($"[BasicUpgrade] {upgradeName} upgraded to level {currentLevel}. Cost now: {upgradeInfo.upgradeCost}");
    }
}
