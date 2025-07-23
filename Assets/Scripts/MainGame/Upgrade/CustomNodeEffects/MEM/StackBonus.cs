using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StackBonus : MonoBehaviour
{
    private BasicUpgrade upgrade;
    private float lastBonus = 0f;

    void Start()
    {
        upgrade = GetComponent<BasicUpgrade>();
    }

    void Update()
    {
        if (upgrade.currentLevel < 1) return;

        float newBonus = CalculateBonus();
        if (!Mathf.Approximately(newBonus, lastBonus))
        {
            float delta = newBonus - lastBonus;
            CoreStats.Instance.AddStat("PercentBitRate", delta);
            CoreStats.Instance.AddStat("Stack Bonus", delta, StatBranch.MEM);
            lastBonus = newBonus;
        }
    }

    float CalculateBonus()
    {
        if (BitManager.Instance == null || BitManager.Instance.activeGrids.Count == 0)
            return 0f;

        Dictionary<int, int> heightCounts = new();
        List<int> heightList = new();

        foreach (var grid in BitManager.Instance.activeGrids)
        {
            int height = grid.GetBitCapacity().ToString().Length;
            heightList.Add(height);

            if (!heightCounts.ContainsKey(height))
                heightCounts[height] = 0;
            heightCounts[height]++;
        }

        int maxMatch = heightCounts.Values.Max();
        float multiplier = GetMilestoneMultiplier(upgrade.currentLevel);

        float bonus = 0f;
        if (maxMatch >= 1)
            bonus = maxMatch * (multiplier * upgrade.currentLevel);

        //Debug.Log($"[StackBonus] Grid Heights: [{string.Join(", ", heightList)}]");
        //Debug.Log($"[StackBonus] Group Counts: {string.Join(" | ", heightCounts.Select(kv => $"Height {kv.Key}: {kv.Value}x"))} | Max Match = {maxMatch}");
        //Debug.Log($"[StackBonus] Using Multiplier: {multiplier} -> Bonus = {bonus:F2}%");

        return bonus;
    }

    float GetMilestoneMultiplier(int level)
    {
        if (level >= 100) return 4f;
        if (level >= 50) return 2f;
        if (level >= 25) return 1f;
        if (level >= 5) return 0.5f;
        return 0.25f;
    }

    void OnStatChanged(string statName, float newValue)
    {
        if (statName == "FlatBitRate") // or any stat change trigger you want
        {
            // Subtract the old bonus cleanly
            if (lastBonus != 0f)
            {
                CoreStats.Instance.AddStat("PercentBitRate", -lastBonus);
                CoreStats.Instance.AddStat("Stack Bonus", -lastBonus, StatBranch.MEM);
                //Debug.Log($"[StackBonus] OnStatChanged: Removed bonus {lastBonus:F2}%");
            }

            lastBonus = 0f; // Reset for fresh apply next Update
        }
    }
}
