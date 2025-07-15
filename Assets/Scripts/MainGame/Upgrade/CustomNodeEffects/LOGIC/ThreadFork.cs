using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadFork : MonoBehaviour
{
    private BasicUpgrade upgrade;

    private float lastPercentBitRate = 0f;

    private Dictionary<string, bool> pathLogged = new Dictionary<string, bool>();

    void Awake()
    {
        upgrade = GetComponent<BasicUpgrade>();
    }

    void OnEnable()
    {
        UpgradeTrackerManager.OnUpgradeRecorded += HandleUpgradeRecorded;
    }

    void OnDisable()
    {
        UpgradeTrackerManager.OnUpgradeRecorded -= HandleUpgradeRecorded;
    }

    private void HandleUpgradeRecorded(string upgradeName, int level, string pathType)
    {
        if (upgrade == null || upgrade.currentLevel < 1) return;

        // Only recompute if relevant to path logic
        Recalculate();
    }

    public void Recalculate()
    {
        int level = upgrade.currentLevel;

        int basePaths = 0;
        foreach (BranchType branch in System.Enum.GetValues(typeof(BranchType)))
        {
            int branchPaths = CountPaths(branch);
            basePaths += branchPaths;
        }

        int bonusPaths = GetMilestonePathBonus(basePaths, level);
        int totalPaths = basePaths + bonusPaths;

        float ratePerLevel = GetBitRatePerLevel(level);
        float newPercentBitRate = totalPaths * ratePerLevel * level;

        if (!Mathf.Approximately(newPercentBitRate, lastPercentBitRate))
        {
            float delta = newPercentBitRate - lastPercentBitRate;
            CoreStats.Instance.AddStat("PercentBitRate", delta);
            lastPercentBitRate = newPercentBitRate;
        }
    }

    private int CountPaths(BranchType branch)
    {
        int paths = 0;

        // Tier 0
        int t0 = CountTierNodes(branch, 0);
        if (t0 >= 1)
        {
            paths++;
            PrintOnce($"{branch}_Tier0_Any", $"[ThreadFork] +1 Path ({branch}): Tier0 unlocked", branch);
        }
        if (t0 >= 3)
        {
            paths++;
            PrintOnce($"{branch}_Tier0_All", $"[ThreadFork] +1 Path ({branch}): All Tier0", branch);
        }

        // Tier 1
        int t1 = CountTierNodes(branch, 1);
        if (t1 > 1)
        {
            paths++;
            PrintOnce($"{branch}_Tier1_Any", $"[ThreadFork] +1 Path ({branch}): Tier1 unlocked", branch);
        }
        if (t1 >= 3)
        {
            paths++;
            PrintOnce($"{branch}_Tier1_All", $"[ThreadFork] +1 Path ({branch}): All Tier1", branch);
        }

        // Tier 2
        int t2 = CountTierNodes(branch, 2);
        if (HasTwoSiblingTier2(branch))
        {
            paths++;
            PrintOnce($"{branch}_Tier2_Split", $"[ThreadFork] +1 Path ({branch}): Tier2 split", branch);
        }
        if (t2 >= 4)
        {
            paths++;
            PrintOnce($"{branch}_Tier2_All", $"[ThreadFork] +1 Path ({branch}): All Tier2", branch);
        }

        return paths;
    }

    private int CountTierNodes(BranchType branch, int tier)
    {
        int count = 0;
        foreach (var record in UpgradeTrackerManager.Instance.GetAllValidUpgrades())
        {
            if (record.pathType == branch.ToString().ToLower())
            {
                var nc = record.upgradeComponent.GetComponent<NodeConnector>();
                if (nc != null && nc.tier == tier)
                {
                    count++;
                    //Debug.Log($"[ThreadFork] Found unlocked {branch} Tier {tier} Node: {record.upgradeName}");
                }
            }
        }
        //Debug.Log($"[ThreadFork] Total {branch} Tier {tier} Count: {count}");
        return count;
    }

    private bool HasTwoSiblingTier2(BranchType branch)
    {
        var records = UpgradeTrackerManager.Instance.GetAllValidUpgrades();
        var tier2Nodes = new List<NodeConnector>();

        foreach (var record in records)
        {
            if (record.pathType == branch.ToString().ToLower())
            {
                var nc = record.upgradeComponent.GetComponent<NodeConnector>();
                if (nc != null && nc.tier == 2)
                {
                    tier2Nodes.Add(nc);
                    //Debug.Log($"[ThreadFork] Tier2 Node: {record.upgradeName}");
                }
            }
        }

        foreach (var node in tier2Nodes)
        {
            foreach (var neighbor in node.GetConnectedNodes())
            {
                if (neighbor == null) continue;

                // Debug.Log($"[ThreadFork] Checking neighbor: {neighbor.name} for node: {node.name}");

                if (neighbor.tier == 2 && tier2Nodes.Contains(neighbor))
                {
                    //Debug.Log($"[ThreadFork] Found Tier2 Siblings: {node.name} <-> {neighbor.name}");
                    return true;
                }
            }
        }
        return false;
    }

    private int GetMilestonePathBonus(int paths, int level)
    {
        int bonus = 0;
        if (level >= 50)
        {
            bonus += paths / 5;
        }
        else if (level >= 5)
        {
            bonus += paths / 7;
        }
        return bonus;
    }

    private float GetBitRatePerLevel(int level)
    {
        if (level >= 100) return 0.4f;  // 0.4%
        if (level >= 25) return 0.2f;   // 0.2%
        return 0.1f;                        // Below 5, no effect.
    }

    private void PrintOnce(string key, string message, BranchType branch)
    {
        if (!pathLogged.ContainsKey(key))
        {
            LogPrinter.Instance.PrintLog(message, branch);
            pathLogged[key] = true;
        }
    }
}
