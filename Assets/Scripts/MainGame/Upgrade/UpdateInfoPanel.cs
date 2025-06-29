using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UpdateInfoPanel : MonoBehaviour
{
    public static UpdateInfoPanel Instance;

    [Header("UI Text Fields")]
    public TMP_Text titleText;
    public TMP_Text levelText;
    public TMP_Text costText;
    public TMP_Text effectText;

    [Header("Milestone Texts")]
    public TMP_Text milestone5Text;
    public TMP_Text milestone10Text;
    public TMP_Text milestone25Text;
    public TMP_Text milestone50Text;
    public TMP_Text milestone100Text;

    [Header("Milestone Covers")]
    public GameObject coverLv5;
    public GameObject coverLv10;
    public GameObject coverLv25;
    public GameObject coverLv50;
    public GameObject coverLv100;

    [Header("Dynamic Color Elements")]
    public List<Image> imagesToRecolor;
    public List<TMP_Text> textsToRecolor;

    [Header("Branch Theme Colors")]
    public Color cpuColor;
    public Color memoryColor;
    public Color logicColor;

    public static BasicUpgrade CurrentSelectedUpgrade { get; private set; }

    private CoreStats coreStats;

    private void Awake()
    {
        Instance = this; // So UpgradeInfo buttons can easily call it
        coreStats = CoreStats.Instance;
    }

    public void DisplayUpgradeInfo(UpgradeInfo upgrade)
    {
        if (upgrade == null) return;

        // Recalculate discounted upgrade cost
        if (upgrade.upgradeBranch == UpgradeBranch.CPU)
        {
            float rawCost = upgrade.GetUpgradeCost((int)upgrade.currentLevel);
            float cpuDiscount = CoreStats.Instance.GetStat("CPU Discount");
            float finalCost = rawCost;

            if (cpuDiscount > 0f)
            {
                finalCost -= rawCost * (cpuDiscount / 100f);
            }

            upgrade.upgradeCost = finalCost;
        }

        titleText.text = upgrade.upgradeName;
        levelText.text = $"Level: {upgrade.currentLevel}";
        costText.text = $"Cost: {upgrade.upgradeCost:F1} bits";
        effectText.text = upgrade.passiveEffectDescription;

        milestone5Text.text = $"Lv5: {upgrade.unlockAt5}";
        milestone10Text.text = $"Lv10: {upgrade.unlockAt10}";
        milestone25Text.text = $"Lv25: {upgrade.unlockAt25}";
        milestone50Text.text = $"Lv50: {upgrade.unlockAt50}";
        milestone100Text.text = $"Lv100: {upgrade.unlockAt100}";

        Color themeColor = GetBranchColor(upgrade.upgradeBranch);

        foreach (var img in imagesToRecolor)
        {
            if (img != null) img.color = themeColor;
        }

        foreach (var txt in textsToRecolor)
        {
            if (txt != null) txt.color = themeColor;
        }

        // Try to cache the connected BasicUpgrade (if present)
        if (upgrade.TryGetComponent(out BasicUpgrade basic))
        {
            CurrentSelectedUpgrade = basic;
        }
        else
        {
            CurrentSelectedUpgrade = null;
        }

        coverLv5.SetActive(upgrade.currentLevel < 5);
        coverLv10.SetActive(upgrade.currentLevel < 10);
        coverLv25.SetActive(upgrade.currentLevel < 25);
        coverLv50.SetActive(upgrade.currentLevel < 50);
        coverLv100.SetActive(upgrade.currentLevel < 100);
    }

    public Color GetBranchColor(UpgradeBranch branch)
    {
        switch (branch)
        {
            case UpgradeBranch.CPU: return cpuColor;
            case UpgradeBranch.MEM: return memoryColor;
            case UpgradeBranch.LOGIC: return logicColor;
            default:return Color.white;
        }
    }
}
