using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StaticSurge : MonoBehaviour
{
    private BasicUpgrade upgrade;
    private CoreStats coreStats;

    private float surgeTimer = 0f;
    private float surgeDuration = 5f;
    private float surgeCooldown = 20f;
    private bool isSurging = false;
    private float lastRecordedBonus = 0f;

    private float lockedBoostAmount = 0f;

    private Dictionary<string, float> surgeAppliedBonuses = new();

    void Start()
    {
        upgrade = GetComponent<BasicUpgrade>();
        coreStats = CoreStats.Instance;

        ApplyMilestoneEffects();
    }

    void Update()
    {
        if (upgrade == null || upgrade.currentLevel <= 0 || coreStats == null)
            return;

        surgeTimer += Time.deltaTime;

        if (!isSurging && surgeTimer >= surgeCooldown)
        {
            ActivateSurge();
        }

        if (isSurging && surgeTimer >= surgeCooldown + surgeDuration)
        {
            EndSurge();
        }

        SyncExternalStaticSurgeBonus();
    }

    void ActivateSurge()
    {
        isSurging = true;
        surgeAppliedBonuses.Clear();

        float perLevelBonus = GetMilestoneBonus(upgrade.currentLevel);
        float boostAmount = perLevelBonus * upgrade.currentLevel;

        lockedBoostAmount = GetMilestoneBonus(upgrade.currentLevel) * upgrade.currentLevel;

        foreach (var kvp in coreStats.GetAllStats())
        {
            string statName = kvp.Key;
            StatData stat = kvp.Value;

            float actualBoost = (lockedBoostAmount / 100f) * stat.value;

            if (stat.branch == StatBranch.MEM) continue; // skip MEM

            if (statName == "System Sweep")
            {
                coreStats.AddStat(statName, -actualBoost, stat.branch);
            }
            else
            {
                coreStats.AddStat(statName, actualBoost, stat.branch);
            }

            surgeAppliedBonuses[statName] = actualBoost;

            Debug.Log($"[StaticSurge] Boosted {statName} by {actualBoost:F2}%");
        }

        LogPrinter.Instance?.PrintLog($"[StaticSurge] Surge Activated: +{boostAmount:F2}% to non-MEM stats for {surgeDuration} seconds", BranchType.MEM);
    }

    void EndSurge()
    {
        foreach (var kvp in surgeAppliedBonuses)
        {
            string statName = kvp.Key;
            float actualBoost = kvp.Value;

            if (statName == "System Sweep")
            {
                coreStats.AddStat(kvp.Key, actualBoost);
            }
            else
            {
                coreStats.AddStat(kvp.Key, -actualBoost);
            }
            Debug.Log($"[StaticSurge] Removed bonus from {kvp.Key}: -{actualBoost:F2}%");
        }

        surgeAppliedBonuses.Clear();
        isSurging = false;
        surgeTimer = 0f;

        LogPrinter.Instance?.PrintLog($"[StaticSurge] Surge Ended", BranchType.MEM);
    }

    void ApplyMilestoneEffects()
    {
        int lvl = upgrade.currentLevel;

        if (lvl >= 25)
            surgeDuration += 5f;

        if (lvl >= 100)
            surgeCooldown -= 5f;

        surgeCooldown = Mathf.Clamp(surgeCooldown, 5f, 999f);
    }

    float GetMilestoneBonus(int level)
    {
        if (level >= 50) return 0.5f;
        if (level >= 5) return 0.25f;
        return 0.1f;
    }

    void SyncExternalStaticSurgeBonus()
    {
        if (upgrade == null || coreStats == null || upgrade.currentLevel <= 0)
            return;

        float perLevelBonus = GetMilestoneBonus(upgrade.currentLevel);
        float currentBonus = perLevelBonus * upgrade.currentLevel;

        if (!Mathf.Approximately(currentBonus, lastRecordedBonus))
        {
            // Remove old value first, if not the first time
            if (lastRecordedBonus > 0f)
            {
                coreStats.AddStat("Static Surge", -lastRecordedBonus, StatBranch.MEM);
            }

            // Apply new value
            coreStats.AddStat("Static Surge", currentBonus, StatBranch.MEM);
            lastRecordedBonus = currentBonus;

            Debug.Log($"[StaticSurge] External stat synced: Static Surge = {currentBonus:F2}%");
        }
    }
}
