using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class BitRemoveUtility
{
    public static void RemoveBitsProportionally(int bitsToRemove)
    {
        var grids = BitManager.Instance.activeGrids;
        if (bitsToRemove <= 0 || grids.Count == 0) return;

        int remaining = bitsToRemove;

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
            int planned = Mathf.FloorToInt(ratio * bitsToRemove);
            removalMap[grid] = planned;
            totalPlanned += planned;
        }

        // Adjust to match exact amount
        int diff = totalPlanned - bitsToRemove;
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
        int shortfall = bitsToRemove - actualRemoved;

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
        if (actualRemoved != bitsToRemove)
        {
            UnityEngine.Debug.LogWarning($"[BitRemove] Warning! Expected to remove {bitsToRemove} but actually removed {actualRemoved} (ME)");
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

    private Color originalColor;

    public BasicUpgrade upgrade;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(AttemptUpgrade);
        if (costText != null) originalColor = costText.color;
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
        Color currentColor = costText.color;

        costText.color = errorColor;
        yield return new WaitForSeconds(flashDuration);
        costText.color = currentColor;
    }
}