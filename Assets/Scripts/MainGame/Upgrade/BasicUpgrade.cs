using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            upgradeInfo.upgradeCost = GetUpgradeCost(currentLevel); // Cost of next level
            upgradeInfo.passiveEffect = statGainPerLevel * (currentLevel); // total effect
        }

        UnityEngine.Debug.Log($"[BasicUpgrade] {upgradeName} upgraded to level {currentLevel}. Cost now: {upgradeInfo.upgradeCost}");
    }

    public void UpdateLvlText(float currLevel)
    {
        if (levelCounterText != null)
        {
            levelCounterText.text = $"Lvl {currLevel}";
        }
        else
        {
            UnityEngine.Debug.LogWarning($"[Upgrade] No TextMeshProUGUI found for {upgradeName}");
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
            UnityEngine.Debug.LogWarning($"[UnlockLevelOne] No Animator assigned to {upgradeName}");
        }
    }

    private void UnlockCPUNodes()
    {
        NodeConnector connector = GetComponent<NodeConnector>();
        if (connector == null)
        {
            UnityEngine.Debug.LogWarning($"[UnlockNorthernNodes] No NodeConnector on {upgradeName}");
            return;
        }

        List<NodeConnector> northernNodes = new List<NodeConnector> {
        connector.northWest,
        connector.north,
        connector.northEast
        };

        foreach (var node in northernNodes)
        {
            if (node != null)
            {
                var upgrade = node.GetComponent<BasicUpgrade>();
                if (upgrade != null && upgrade.upgradeButton != null)
                {
                    upgrade.upgradeButton.interactable = true;
                    UnityEngine.Debug.Log($"[UnlockCPUNodes] Unlocked: {upgrade.upgradeName}");
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"[UnlockCPUNodes] Node or button missing for a neighbor of {upgradeName}");
                }
            }
        }
    }

    private void UnlockLogicNodes()
    {
        NodeConnector connector = GetComponent<NodeConnector>();
        if (connector == null)
        {
            UnityEngine.Debug.LogWarning($"[UnlockLogicNodes] No NodeConnector on {upgradeName}");
            return;
        }

        List<NodeConnector> northernNodes = new List<NodeConnector> {
        connector.northEast,
        connector.southEast,
        connector.south
        };

        foreach (var node in northernNodes)
        {
            if (node != null)
            {
                var upgrade = node.GetComponent<BasicUpgrade>();
                if (upgrade != null && upgrade.upgradeButton != null)
                {
                    upgrade.upgradeButton.interactable = true;
                    UnityEngine.Debug.Log($"[UnlockLogicNodes] Unlocked: {upgrade.upgradeName}");
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
            UnityEngine.Debug.LogWarning($"[UnlockMEMNodes] No NodeConnector on {upgradeName}");
            return;
        }

        List<NodeConnector> northernNodes = new List<NodeConnector> {
        connector.northWest,
        connector.southWest,
        connector.south
        };

        foreach (var node in northernNodes)
        {
            if (node != null)
            {
                var upgrade = node.GetComponent<BasicUpgrade>();
                if (upgrade != null && upgrade.upgradeButton != null)
                {
                    upgrade.upgradeButton.interactable = true;
                    UnityEngine.Debug.Log($"[UnlockMEMNodes] Unlocked: {upgrade.upgradeName}");
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"[UnlockMEMNodes] Node or button missing for a neighbor of {upgradeName}");
                }
            }
        }
    }
}
