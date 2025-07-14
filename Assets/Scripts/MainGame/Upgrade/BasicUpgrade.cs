using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum CoreStatType
{
    FlatBitRate,
    PercentBitRate,
    // Future stat keys...
}

public enum BranchType
{
    CPU,
    MEM,
    LOGIC,
}

public class BasicUpgrade : MonoBehaviour
{
    [Header("Upgrade Branch")]
    public BranchType upgradeBranch;

    public UpgradeInfo upgradeInfo;
    public string upgradeName;
    public CoreStatType statToModify;
    public float baseCost = 8f;
    public float linearRate = 5f;
    public float quadraticRate = 0.5f;

    public float statGainPerLevel = 1f; // Raw +1 per level, or +1% per level for percent stats
    public int currentLevel = 0;

    public TextMeshProUGUI levelCounterText;
    public TextMeshProUGUI levelNameText;

    public UnityEngine.UI.Button upgradeButton;
    public Color normalColor;
    public Color highlightedColor;
    public Color pressedColor;

    public Color textColor;

    public Animator upgradeAnimator;

    private bool levelTenUnlocked = false;
    private bool printLevelUnlock = false;

    public float GetUpgradeCost(int level)
    {
        return baseCost + linearRate * level + quadraticRate * level * level;
    }

    public int UpgradeLevel()
    {
        return currentLevel;
    }

    public void ApplyUpgrade(CoreStats coreStats)
    {
        currentLevel++;

        if (currentLevel == 1)
        {
            UnlockLevelOne();

            // Activate assigned grid if helper exists
            var gridHelper = GetComponent<GridCoreHelper>();
            if (gridHelper != null)
            {
                gridHelper.TryActivateGrid();
            }
        }

        if (currentLevel >= 10 && !levelTenUnlocked)
        {
            if (upgradeBranch == BranchType.CPU)
            {
                UnlockCPUNodes();
            }
            if (upgradeBranch == BranchType.MEM)
            {
                UnlockMemNodes();
            }
            if (upgradeBranch == BranchType.LOGIC)
            {
                UnlockLogicNodes();
            }
            levelTenUnlocked = true;
        }

        //CorePulse
        var pulse = GetComponent<CorePulse>();
        if (pulse != null)
        {
            pulse.RecalculateFlatBitRate();
        }

        float value = statGainPerLevel;

        // Add stat to Core
        coreStats.AddStat(statToModify.ToString(), value);

        // Update TrackerManager
        if (UpgradeTrackerManager.Instance != null && upgradeInfo != null)
        {
            string name = upgradeName;
            int newLevel = currentLevel;
            string path = upgradeInfo.upgradeBranch.ToString().ToLower();
            UpgradeTrackerManager.Instance.RecordUpgrade(name, currentLevel, path, this);
        }

        var gridCores = GameObject.FindObjectsOfType<GridCore>();
        //UnityEngine.Debug.Log($"[BasicUpgrade] Checking GridCores for upgradeName: {upgradeName}");
        foreach (var core in gridCores)
        {
            //UnityEngine.Debug.Log($"[BasicUpgrade] Found GridCore: {core.upgradeName}");
            if (core.upgradeName == upgradeName)
            {
                //UnityEngine.Debug.Log($"[BasicUpgrade] Match found. Calling CheckUpgradeMilestone() on {core.upgradeName}");
                core.CheckUpgradeMilestone(currentLevel);
            }
        }

        UpdateLvlText(currentLevel);

        // Update UpgradeInfo UI values
        if (upgradeInfo != null)
        {
            upgradeInfo.currentLevel = currentLevel;

            float rawCost = GetUpgradeCost(currentLevel);
            float finalCost = rawCost;

            if (upgradeBranch == BranchType.CPU)
            {
                float cpuDiscount = CoreStats.Instance.GetStat("CPU Discount");
                if (cpuDiscount > 0f)
                {
                    finalCost -= rawCost * (cpuDiscount / 100);
                }
            }

            upgradeInfo.upgradeCost = finalCost; // Use discounted cost
            upgradeInfo.passiveEffect = statGainPerLevel * (currentLevel); // total effect
        }

        if (upgradeInfo.currentLevel > 1)
        {
            LogPrinter.Instance.PrintLog($"Upgraded {upgradeName} to Level {currentLevel}", upgradeBranch);
        }

        Debug.Log($"[BasicUpgrade] {upgradeName} upgraded to level {currentLevel}. Cost now: {upgradeInfo.upgradeCost}");
    }

    public void UpdateLvlText(float currLevel)
    {
        if (levelCounterText != null)
        {
            levelCounterText.text = $"Lvl {currLevel}";
        }
        else
        {
            Debug.LogWarning($"[Upgrade] No TextMeshProUGUI found for {upgradeName}");
        }
    }

    public void UnlockLevelOne()
    {
        // Tell the Animator to switch to the Unlocked visuals
        if (upgradeAnimator != null)
        {
            upgradeAnimator.SetBool("UnlockedLevel1", true);
        }
        else
        {
            Debug.LogWarning($"[UnlockLevelOne] No Animator assigned to {upgradeName}");
        }

        if (!printLevelUnlock)
        {
            LogPrinter.Instance.PrintLog($"Unlocked {upgradeName}", upgradeBranch);
            printLevelUnlock = true;
        }
    }

    private void UnlockCPUNodes()
    {
        NodeConnector connector = GetComponent<NodeConnector>();
        if (connector == null)
        {
            Debug.LogWarning($"[UnlockNorthernNodes] No NodeConnector on {upgradeName}");
            return;
        }

        List<NodeConnector> cpuNodes = new List<NodeConnector> {
        connector.northWest,
        connector.north,
        connector.northEast
        };

        foreach (var node in cpuNodes)
        {
            if (node != null)
            {
                var upgrade = node.GetComponent<BasicUpgrade>();
                if (upgrade != null && upgrade.upgradeButton != null)
                {
                    upgrade.upgradeButton.interactable = true;
                    Debug.Log($"[UnlockCPUNodes] Unlocked: {upgrade.upgradeName}");
                }
                else
                {
                    Debug.LogWarning($"[UnlockCPUNodes] Node or button missing for a neighbor of {upgradeName}");
                }
            }
        }
    }

    private void UnlockLogicNodes()
    {
        NodeConnector connector = GetComponent<NodeConnector>();
        if (connector == null)
        {
            Debug.LogWarning($"[UnlockLogicNodes] No NodeConnector on {upgradeName}");
            return;
        }

        List<NodeConnector> logicNodes = new List<NodeConnector> {
        connector.northEast,
        connector.southEast,
        connector.south
        };

        foreach (var node in logicNodes)
        {
            if (node != null)
            {
                var upgrade = node.GetComponent<BasicUpgrade>();
                if (upgrade != null && upgrade.upgradeButton != null)
                {
                    upgrade.upgradeButton.interactable = true;
                    Debug.Log($"[UnlockLogicNodes] Unlocked: {upgrade.upgradeName}");
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"[UnlockLogicNodes] Node or button missing for a neighbor of {upgradeName}");
                }
            }
        }
    }

    private void UnlockMemNodes()
    {
        NodeConnector connector = GetComponent<NodeConnector>();
        if (connector == null)
        {
            Debug.LogWarning($"[UnlockMEMNodes] No NodeConnector on {upgradeName}");
            return;
        }

        List<NodeConnector> memNodes = new List<NodeConnector> {
        connector.northWest,
        connector.southWest,
        connector.south
        };

        foreach (var node in memNodes)
        {
            if (node != null)
            {
                var upgrade = node.GetComponent<BasicUpgrade>();
                if (upgrade != null && upgrade.upgradeButton != null)
                {
                    upgrade.upgradeButton.interactable = true;
                    Debug.Log($"[UnlockMEMNodes] Unlocked: {upgrade.upgradeName}");
                }
                else
                {
                    Debug.LogWarning($"[UnlockMEMNodes] Node or button missing for a neighbor of {upgradeName}");
                }
            }
        }
    }
}
