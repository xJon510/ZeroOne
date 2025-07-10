
using UnityEngine;

public class ThreadWeaver : MonoBehaviour
{
    private BasicUpgrade upgrade;

    private float lastBonus = 0f;

    void Awake()
    {
        upgrade = GetComponent<BasicUpgrade>();
    }

    void Update()
    {
        if (upgrade == null || upgrade.currentLevel < 1) return;

        // Count CPU nodes >= 10 (including itself!)
        int cpuNodeCount = 0;
        var cpuUpgrades = UpgradeTrackerManager.Instance.GetAllValidUpgrades();
        foreach (var record in cpuUpgrades)
        {
            if (record.pathType == "cpu" && record.level >= 10)
            {
                cpuNodeCount++;
            }
        }

        // Make sure we include ourself even if not tracked yet
        if (upgrade.upgradeBranch == BranchType.CPU && upgrade.currentLevel >= 10)
        {
            if (!cpuUpgrades.Exists(r => r.upgradeName == upgrade.upgradeName))
            {
                cpuNodeCount++;
            }
        }

        // Calculate bonus per level based on milestone
        float bonusPerLevel = GetBonusPerLevel(upgrade.currentLevel);

        float newBonus = cpuNodeCount * bonusPerLevel * upgrade.currentLevel;

        if (!Mathf.Approximately(newBonus, lastBonus))
        {
            float delta = newBonus - lastBonus;
            CoreStats.Instance.AddStat("PercentBitRate", delta);
            lastBonus = newBonus;
            Debug.Log($"(ME) {delta}");
        }
    }

    private float GetBonusPerLevel(int level)
    {
        if (level >= 100) return 1f;   // 1% per level
        if (level >= 50) return 0.75f;  // 0.75% per level
        if (level >= 25) return 0.5f;   // 0.5% per level
        if (level >= 5) return 0.25f;   // 0.25% per level
        return 0.1f;                    // 0.1% per level
    }
}
