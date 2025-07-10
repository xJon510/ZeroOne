using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class BitRemoveUtility
{
    public static void RemoveBitsProportionally(int bitsToRemove)
    {
        if (bitsToRemove <= 0) return;

        float overflowBits = BitManager.Instance.GetOverflowBits();
        int overflowAvailable = Mathf.FloorToInt(overflowBits);

        int remaining = bitsToRemove;

        if (overflowAvailable > 0)
        {
            if (overflowAvailable >= bitsToRemove)
            {
                // Fully covered by overflow
                BitManager.Instance.overflowBits -= bitsToRemove;
                Debug.Log($"[BitRemove] Removed {bitsToRemove} bits from overflow buffer only.");
                return;
            }
            else
            {
                // Partial: drain all overflow first
                BitManager.Instance.overflowBits -= overflowAvailable;
                remaining -= overflowAvailable;
                Debug.Log($"[BitRemove] Removed {overflowAvailable} bits from overflow, {remaining} left to remove from grids.");
            }
        }

        var grids = BitManager.Instance.activeGrids;
        if (bitsToRemove <= 0 || grids.Count == 0) return;

        // Filter out any grids with zero capacity
        grids = grids.FindAll(g => g.GetBitCapacity() > 0);

        // Total capacity sum
        float totalCapacity = 0f;
        Dictionary<BitGridManager, float> capacities = new Dictionary<BitGridManager, float>();

        foreach (var grid in grids)
        {
            float cap = grid.GetBitCapacity(); // Assuming you already have this method
            capacities[grid] = cap;
            totalCapacity += cap;
        }

        // Inverted weights: smaller grid = higher priority
        Dictionary<BitGridManager, float> weights = new Dictionary<BitGridManager, float>();
        float weightSum = 0f;

        foreach (var grid in grids)
        {
            float weight = 1f - (capacities[grid] / totalCapacity); // lower capacity = higher weight
            weight = Mathf.Max(0.01f, weight); // avoid zero
            weights[grid] = weight;
            weightSum += weight;
        }

        // Initial removal map
        Dictionary<BitGridManager, int> removalMap = new Dictionary<BitGridManager, int>();
        int totalPlanned = 0;
        foreach (var grid in grids)
        {
            float ratio = weights[grid] / weightSum;
            int planned = Mathf.FloorToInt(ratio * remaining);
            removalMap[grid] = planned;
            totalPlanned += planned;
        }

        // Adjust to match exact amount
        int diff = totalPlanned - remaining;
        if (diff != 0)
        {
            foreach (var grid in grids)
            {
                if (diff == 0) break;

                if (diff > 0 && removalMap[grid] > 0)
                {
                    removalMap[grid]--;
                    diff--;
                }
                else if (diff < 0)
                {
                    removalMap[grid]++;
                    diff++;
                }
            }
        }

        // Apply removals carefully and track how much we actually removed
        int actualRemoved = 0;

        foreach (var kvp in removalMap)
        {
            var grid = kvp.Key;
            int toRemove = kvp.Value;

            ulong current = grid.GetLocalBitValue();
            int safeRemove = Mathf.Min(toRemove, (int)current);

            actualRemoved += safeRemove;

            grid.SetLocalBitValue((ulong)(current - (ulong)safeRemove));
            grid.RefreshBitCharacters();
        }

        // Fix edge case: If rounding prevented full removal, remove the rest
        int shortfall = remaining - actualRemoved;

        if (shortfall > 0)
        {
            foreach (var grid in grids.OrderByDescending(g => g.GetLocalBitValue()))
            {
                if (shortfall <= 0) break;

                ulong current = grid.GetLocalBitValue();
                int extraRemove = Mathf.Min(shortfall, (int)current);

                grid.SetLocalBitValue((ulong)(current - (ulong)extraRemove));
                grid.RefreshBitCharacters();

                shortfall -= extraRemove;
                actualRemoved += extraRemove;
            }
        }

        // Debug safety check
        if (actualRemoved != remaining)
        {
            UnityEngine.Debug.LogWarning($"[BitRemove] Warning! Expected to remove {remaining} but actually removed {actualRemoved} (ME)");
        }
    }
}

public class UpgradeMainButtonHandler : MonoBehaviour
{
    [Header("Visual Feedback")]
    public BkRndShake shakeTarget;
    public TMPro.TMP_Text costText;
    public Color errorColor = Color.red;

    public float flashDuration = 0.3f;

    public BasicUpgrade upgrade;

    public CantAffordSFX cantAffordSFX;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(AttemptUpgrade);
    }

    private void AttemptUpgrade()
    {
        UnityEngine.Debug.Log($"[UpgradeMainButton] CurrentSelectedUpgrade is: {(UpdateInfoPanel.CurrentSelectedUpgrade != null ? UpdateInfoPanel.CurrentSelectedUpgrade.upgradeName : "NULL")}");

        var upgrade = UpdateInfoPanel.CurrentSelectedUpgrade;

        if (upgrade != null)
        {
            float upgradeCost = upgrade.GetUpgradeCost(upgrade.currentLevel);

            // Apply CPU discount if applicable
            if (upgrade.upgradeBranch == BranchType.CPU)
            {
                float cpuDiscount = CoreStats.Instance.GetStat("CPU Discount");
                if (cpuDiscount > 0f)
                {
                    upgradeCost -= upgradeCost * (cpuDiscount / 100);
                }
            }

            ulong bits = BitManager.Instance.currentBits;

            if ((ulong)upgradeCost > bits)
            {
                UnityEngine.Debug.LogWarning($"[UpgradeMainButton] Not enough bits. Needed: {upgradeCost}, Available: {bits}");
                if (costText != null)
                {
                    StopAllCoroutines(); // in case it's still fading
                    StartCoroutine(FlashCostText());
                }

                if (shakeTarget != null)
                {
                    shakeTarget.TriggerShake();
                }

                if (cantAffordSFX != null)
                {
                    cantAffordSFX.PlayCantAfford();
                }

                return;
            }

            // Deduct cost
            BitRemoveUtility.RemoveBitsProportionally((int)upgradeCost);
            UnityEngine.Debug.Log($"[UpgradeMainButton] {(int)upgradeCost} Removed From Total (ME)");

            // Apply upgrade
            upgrade.ApplyUpgrade(CoreStats.Instance);

            // Track upgrade in manager
            if (UpgradeTrackerManager.Instance != null)
            {
                string upgradeName = upgrade.upgradeName;
                int newLevel = upgrade.currentLevel;
                string pathType = upgrade.upgradeInfo.upgradeBranch.ToString().ToLower(); // assuming it's an enum or string

                UpgradeTrackerManager.Instance.RecordUpgrade(upgradeName, newLevel, pathType);
                UnityEngine.Debug.Log($"[Tracker] Recorded upgrade: {upgradeName}, Level: {newLevel}, Path: {pathType}");
            }

            // Update BitRate if applicable
            if (upgrade.statToModify == CoreStatType.FlatBitRate)
            {
                float newBitRate = CoreStats.Instance.GetStat("FlatBitRate");
                BitManager.Instance.UpdateGlobalBitRate(newBitRate);
            }

            // Refresh the panel display with new values
            if (upgrade.upgradeInfo != null)
            {
                UpdateInfoPanel.Instance.DisplayUpgradeInfo(upgrade.upgradeInfo);
            }

            if (cantAffordSFX != null)
            {
                cantAffordSFX.PlayNormalClick();
            }

            // Save :P
            SaveManager.Instance?.SaveGame();

        }
        else
        {
            UnityEngine.Debug.LogWarning("[UpgradeMainButton] No upgrade selected.");
        }
    }

    private IEnumerator FlashCostText()
    {
        costText.color = errorColor;
        yield return new WaitForSeconds(flashDuration);

        // Get the currently selected upgrade's path color
        if (UpdateInfoPanel.CurrentSelectedUpgrade != null)
        {
            var branchType = UpdateInfoPanel.CurrentSelectedUpgrade.upgradeBranch;

            // Convert BranchType -> UpgradeBranch
            UpgradeBranch convertedBranch = UpgradeBranch.CPU;
            switch (branchType)
            {
                case BranchType.CPU:
                    convertedBranch = UpgradeBranch.CPU; break;
                case BranchType.MEM:
                    convertedBranch = UpgradeBranch.MEM; break;
                case BranchType.LOGIC:
                    convertedBranch = UpgradeBranch.LOGIC; break;
            }

            Color branchColor = UpdateInfoPanel.Instance.GetBranchColor(convertedBranch);
            costText.color = branchColor;
        }
        else
        {
            costText.color = Color.white; // fallback
        }
    }
}