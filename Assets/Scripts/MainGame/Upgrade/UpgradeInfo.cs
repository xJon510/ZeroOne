using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum UpgradeBranch
{
    CPU,
    MEM,
    LOGIC
}

public class UpgradeInfo : MonoBehaviour
{
    [Header("Upgrade Branch")]
    public UpgradeBranch upgradeBranch;

    [Header("Upgrade Info")]
    [TextArea]
    public string upgradeName;
    [TextArea]
    public string passiveEffectDescription;

    [Header("Starting Values")]
    public float startCost = 5f;
    public float upgradeCost;
    public float currentLevel = 0;

    [Header("Cost and Leveling")]
    public float upgradeCostScale;
    public float passiveEffect;

    [Header("Unlocks List")]
    public string unlockAt5;
    public string unlockAt10;
    public string unlockAt25;
    public string unlockAt50;
    public string unlockAt100;

    // Start is called before the first frame update
    void Start()
    {
        upgradeCost = startCost;
    }

    [Header("Panel Switcher")]
    public CoreNodePanelSwitcher panelSwitcher;

    public void OnUpgradeClicked()
    {
        StartCoroutine(DelayedSelect());
    }

    private IEnumerator DelayedSelect()
    {
        yield return null; // Wait one frame

        if (UpdateInfoPanel.Instance != null)
        {
            UpdateInfoPanel.Instance.DisplayUpgradeInfo(this);

            // If we came from CoreStats, flip panels back
            if (panelSwitcher != null)
            {
                panelSwitcher.ShowUpgradeInfo(); // bring UpgradeInfo back in
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("[UpgradeInfo] No UpdateInfoPanel.Instance found!");
        }
    }

    public float GetUpgradeCost(int level)
    {
        var basic = GetComponent<BasicUpgrade>();
        if (basic == null)
        {
            UnityEngine.Debug.LogWarning($"[UpgradeInfo] No BasicUpgrade found on {upgradeName}");
            return startCost;
        }

        return basic.GetUpgradeCost(level);
    }
}
