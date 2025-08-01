using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemSweep : MonoBehaviour
{
    private CoreStats coreStats;
    private BasicUpgrade thisUpgrade;
    private int upgradeLevel = 0;

    private float sweepCooldown;
    private float timeUntilNextSweep;
    private int sweepCount = 1;

    private float lastSweepCooldown = 0f;

    private void Start()
    {
        coreStats = FindObjectOfType<CoreStats>();
        thisUpgrade = GetComponent<BasicUpgrade>();

        if (BitManager.Instance != null)
            BitManager.OnGameTick += TickHandler;
    }

    private void OnDestroy()
    {
        if (BitManager.Instance != null)
            BitManager.OnGameTick -= TickHandler;
    }

    private void TickHandler()
    {
        if (thisUpgrade == null || coreStats == null) return;

        upgradeLevel = thisUpgrade.currentLevel;
        if (upgradeLevel <= 0) return;

        ApplyMilestoneEffects();

        // Calculate cooldown
        float milestoneBonus = (upgradeLevel >= 5) ? -5f : 0f;
        sweepCooldown = Mathf.Clamp(45f - (0.25f * upgradeLevel) + milestoneBonus, 15f, 45f);

        if (timeUntilNextSweep <= 0f)
        {
            timeUntilNextSweep = sweepCooldown;
            return; // Skip immediate first sweep
        }

        timeUntilNextSweep -= 1f;

        if (timeUntilNextSweep <= 0f)
        {
            timeUntilNextSweep = sweepCooldown;
            OnTick();
        }

        float displayCooldown = sweepCooldown;

        // Store it in CoreStats as "Sweep Cooldown"
        if (!Mathf.Approximately(displayCooldown, lastSweepCooldown))
        {
            coreStats.AddStat("System Sweep", displayCooldown - lastSweepCooldown, StatBranch.LOGIC);
            lastSweepCooldown = displayCooldown;
        }
    }

    public void OnTick()
    {
        if (thisUpgrade == null || coreStats == null) return;

        upgradeLevel = thisUpgrade.currentLevel;
        if (upgradeLevel <= 0) return;

        var pool = UpgradeTrackerManager.Instance.GetAllValidUpgrades();
        pool.RemoveAll(r => r.upgradeComponent == thisUpgrade);

        Debug.Log($"[SystemSweep] Pool size after filtering: {pool.Count}");

        if (pool.Count == 0) return;

        PerformSystemSweep(pool);
        sweepCount++;
    }

    private void PerformSystemSweep(List<UpgradeTrackerManager.UpgradeRecord> pool)
    {
        int index = UnityEngine.Random.Range(0, pool.Count);
        var target = pool[index];

        int timesToApply = 1;

        // Milestone 25: Every 5th sweep applies +2 levels
        if (bonusSweepEnabled && (sweepCount % 5 == 0))
        {
            timesToApply = 2;
        }

        // Milestone 50+: Chance to double each sweep
        if (doubleChanceEnabled)
        {
            float doubleChance = 0.005f * upgradeLevel; // +0.5% per level
            if (UnityEngine.Random.value < doubleChance)
            {
                timesToApply *= 2;
            }
        }

        for (int j = 0; j < timesToApply; j++)
        {
            target.upgradeComponent.ApplyUpgrade(coreStats);
        }

        Debug.Log($"[SystemSweep] Upgraded: {target.upgradeName} to level {target.upgradeComponent.currentLevel}");
        LogPrinter.Instance.PrintLog($"[SystemSweep] Upgraded: {target.upgradeName} to level {target.upgradeComponent.currentLevel}", BranchType.LOGIC);

        // Also add to SweptCache if present
        var sweptCache = FindObjectOfType<SweptCache>();
        float sweptAmount = 1f;

        if (sweptCache != null)
        {
            int max = sweptCache.GetLevelCap();
            float current = coreStats.GetStat("SweptCache");

            if (upgradeLevel >= 50)
            {
                sweptAmount = 2f; // Milestone 50+
            }

            if (current < max)
            {
                float spaceLeft = max - current;
                float clampedSweep = Mathf.Min(sweptAmount, spaceLeft);

                coreStats.AddStat("SweptCache", clampedSweep, StatBranch.LOGIC);

                float newTotal = current + clampedSweep;

                Debug.Log($"[SystemSweep] +{clampedSweep} SweptCache => {newTotal} / {max}");
                LogPrinter.Instance.PrintLog($"[SweptCache] +{clampedSweep} -> {newTotal} / {max}", BranchType.LOGIC);
            }
        }
    }

    private void ApplyMilestoneEffects()
    {
        // +2 every 5th sweep (25+)
        if (upgradeLevel >= 25)
            EnableBonusSweeps();

        // Double Chance (50+)
        if (upgradeLevel >= 50)
            EnableDoubleChance();

        // Upgrade Pulse Range (100+) � Placeholder
        //if (upgradeLevel >= 100) 
            //GrantUpgradePulseRange();
    }

    private bool doubleChanceEnabled = false;
    private void EnableDoubleChance()
    {
        doubleChanceEnabled = true;
    }

    private bool bonusSweepEnabled = false;
    private void EnableBonusSweeps()
    {
        bonusSweepEnabled = true;
    }
}