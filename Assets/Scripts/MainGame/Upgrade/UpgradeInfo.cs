using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class UpgradeInfo : MonoBehaviour
{
    [Header("Upgrade Info")]
    public string upgradeName;
    public string passiveEffectDescription;

    [Header("Starting Values")]
    public float startCost = 5f;
    public float upgradeCost;
    public float currentLevel = 0;

    [Header("Cost and Leveling")]
    public float upgradeCostScale;
    public float passiveEffect;

    // Start is called before the first frame update
    void Start()
    {
        upgradeCost = startCost;
    }

    [Header("Panel Switcher")]
    public CoreNodePanelSwitcher panelSwitcher;

    public void OnUpgradeClicked()
    {
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

    public void AttemptUpgrade()
    {
        ulong currentBits = BitManager.Instance.currentBits;

        if (currentBits < (ulong)upgradeCost)
        {
            UnityEngine.Debug.Log($"[Upgrade] Not enough bits to upgrade {upgradeName}. Need {upgradeCost}, have {currentBits}");
            return;
        }

        // Placeholder removal logic — we’ll eventually hook this into the proportional bit remover
        //BitRemoveUtility.RemoveBitsFromGrids((int)upgradeCost);

        currentLevel++;
        UnityEngine.Debug.Log($"[Upgrade] {upgradeName} upgraded to level {currentLevel}!");
    }

}
