using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveHeal : MonoBehaviour
{
    private CoreStats coreStats;
    private BasicUpgrade thisUpgrade;
    private int upgradeLevel = 0;

    private float healCooldown;
    private float timeUntilNextHeal;

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

        float perLevelReduction = upgradeLevel >= 50 ? 0.25f : 0.1f;
        float milestoneBonus = (upgradeLevel >= 5) ? -10f : 0f;

        healCooldown = Mathf.Clamp(60f - (perLevelReduction * upgradeLevel) + milestoneBonus, 15f, 60f);

        if (timeUntilNextHeal <= 0f)
        {
            timeUntilNextHeal = healCooldown;
            return; // skip first tick immediately
        }

        timeUntilNextHeal -= 1f;

        if (timeUntilNextHeal <= 0f)
        {
            timeUntilNextHeal = healCooldown;
            OnTick();
        }
    }

    private void OnTick()
    {
        var pool = UpgradeTrackerManager.Instance.GetAllValidUpgrades();
        pool.RemoveAll(r => r.upgradeComponent == thisUpgrade);

        if (pool.Count < 2) return;

        // Pick random source + target
        var source = pool[Random.Range(0, pool.Count)];
        var target = pool[Random.Range(0, pool.Count)];

        while (source == target && pool.Count > 1)
            target = pool[Random.Range(0, pool.Count)];

        int removeAmount = 1;
        int addAmount = 2;

        if (upgradeLevel >= 100)
        {
            removeAmount = 3;
            addAmount = 6;
        }
        else if (upgradeLevel >= 25)
        {
            removeAmount = 2;
            addAmount = 4;
        }

        for (int i = 0; i < removeAmount; i++)
        {
            source.upgradeComponent.RemoveUpgradeLevel(coreStats);
        }

        for (int i = 0; i < addAmount; i++)
        {
            target.upgradeComponent.ApplyUpgrade(coreStats);
        }

        // Save :P
        SaveManager.Instance?.SaveGame();

        Debug.Log($"[RecursiveHeal] -{removeAmount} Lvls from {source.upgradeName} -> +{addAmount} Lvls to {target.upgradeName}");
        LogPrinter.Instance.PrintLog($"[RecursiveHeal] -{removeAmount} {source.upgradeName} -> +{addAmount} {target.upgradeName}", BranchType.LOGIC);
    }
}
