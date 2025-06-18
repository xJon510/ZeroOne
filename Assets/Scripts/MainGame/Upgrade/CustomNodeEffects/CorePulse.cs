using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CorePulse : MonoBehaviour
{
    private CoreStats coreStats;
    private BasicUpgrade thisUpgrade;
    private BitManager bitManager;

    private float lastAppliedFlatAmount = 0f;

    private float[] milestoneRates = { 1.0f, 1.1f, 1.25f, 2.0f };

    private float storageRatioCheckCooldown = 0.5f;
    private float storageRatioCheckTimer = 0f;
    private float lastStorageRatio = 0f;

    private bool initialized = false;

    private void Update()
    {
        if (thisUpgrade == null || thisUpgrade.UpgradeLevel() < 25)
            return;

        storageRatioCheckTimer += Time.deltaTime;
        if (storageRatioCheckTimer >= storageRatioCheckCooldown)
        {
            storageRatioCheckTimer = 0f;

            float currentRatio = GetBitStorageRatio();
            bool previouslyEligible = lastStorageRatio >= 0.5f;
            bool currentlyEligible = currentRatio >= 0.5f;

            if (previouslyEligible != currentlyEligible)
            {
                RecalculateFlatBitRate();
            }

            lastStorageRatio = currentRatio;
        }
    }

    public void RecalculateFlatBitRate()
    {
        LazyInit();

        int level = thisUpgrade.UpgradeLevel();
        if (level < 1) return;

        if (coreStats == null)
        {
            UnityEngine.Debug.LogWarning("[CorePulse] coreStats is null in RecalculateFlatBitRate");
            return;
        }

        // Remove previous amount
        if (lastAppliedFlatAmount != 0f)
        {
            coreStats.AddStat("FlatBitRate", -lastAppliedFlatAmount);
            lastAppliedFlatAmount = 0f;
        }

        int milestoneTier = GetMilestoneTier(level);
        float newRate = milestoneRates[milestoneTier];
        float baseAmount = newRate * level;

        float bonusAmount = CalculateCorePulseBonus(baseAmount);

        float totalAmount = baseAmount + CalculateCorePulseBonus(baseAmount);
        coreStats.AddStat("FlatBitRate", totalAmount);
        lastAppliedFlatAmount = totalAmount;

        UnityEngine.Debug.Log($"[CorePulse] Applied FlatBitRate: {totalAmount}");
    }

    private int GetMilestoneTier(int level)
    {
        if (level >= 100) return 3;
        if (level >= 50) return 2;
        if (level >= 5) return 1;
        return 0;
    }

    private float GetBitStorageRatio()
    {
        ulong totalStored = 0;
        ulong totalCapacity = 0;

        foreach (BitGridManager grid in bitManager.activeGrids)
        {
            totalStored += grid.GetLocalBitValue();
            totalCapacity += grid.GetBitCapacity();
        }

        return totalCapacity == 0 ? 0f : (float)totalStored / totalCapacity;
    }

    private float CalculateCorePulseBonus(float baseAmount)
    {
        int level = thisUpgrade.UpgradeLevel();
        if (level >= 25 && GetBitStorageRatio() >= 0.5f)
        {
            return baseAmount * 0.25f;
        }
        return 0f;
    }

    void LazyInit()
    {
        if (initialized) return;

        thisUpgrade = GetComponent<BasicUpgrade>();
        coreStats = FindObjectOfType<CoreStats>();
        bitManager = FindObjectOfType<BitManager>();

        initialized = true;
    }
}