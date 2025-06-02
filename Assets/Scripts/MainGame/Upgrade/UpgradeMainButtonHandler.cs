using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public static class BitRemoveUtility
{
    public static void RemoveBitsProportionally(int bitsToRemove)
    {
        var grids = BitManager.Instance.activeGrids;
        ulong total = BitManager.Instance.currentBits;

        if (total == 0 || grids.Count == 0) return;

        float totalFloat = (float)total;
        int remaining = bitsToRemove;

        Dictionary<BitGridManager, int> removalMap = new Dictionary<BitGridManager, int>();

        foreach (var grid in grids)
        {
            ulong gridBits = grid.GetLocalBitValue();
            float ratio = gridBits / totalFloat;
            int toRemove = Mathf.FloorToInt(ratio * bitsToRemove);
            removalMap[grid] = toRemove;
            remaining -= toRemove;
        }

        foreach (var grid in grids)
        {
            if (remaining <= 0) break;
            if (grid.GetLocalBitValue() > (ulong)removalMap[grid])
            {
                removalMap[grid] += 1;
                remaining -= 1;
            }
        }

        foreach (var kvp in removalMap)
        {
            BitGridManager grid = kvp.Key;
            int toRemove = kvp.Value;
            ulong current = grid.GetLocalBitValue();
            ulong newValue = (ulong)Mathf.Max(0, (long)current - toRemove);

            var field = typeof(BitGridManager).GetField("localBitValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var method = typeof(BitGridManager).GetMethod("UpdateDebugText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field != null) field.SetValue(grid, newValue);
            if (method != null) method.Invoke(grid, null);
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
            float upgradeCost = upgrade.GetUpgradeCost(upgrade.currentLevel); // next level cost
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

            // Apply upgrade
            upgrade.ApplyUpgrade(CoreStats.Instance);

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