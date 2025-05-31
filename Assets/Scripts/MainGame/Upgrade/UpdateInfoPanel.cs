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

    [Header("Dynamic Color Elements")]
    public List<Image> imagesToRecolor;
    public List<TMP_Text> textsToRecolor;

    [Header("Branch Theme Colors")]
    public Color cpuColor;
    public Color memoryColor;
    public Color logicColor;

    public static BasicUpgrade CurrentSelectedUpgrade { get; private set; }

    private void Awake()
    {
        Instance = this; // So UpgradeInfo buttons can easily call it
    }

    public void DisplayUpgradeInfo(UpgradeInfo upgrade)
    {
        if (upgrade == null) return;

        titleText.text = upgrade.upgradeName;
        levelText.text = $"Level: {upgrade.currentLevel}";
        costText.text = $"Cost: {upgrade.upgradeCost} bits";
        effectText.text = upgrade.passiveEffectDescription;

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
    }

    public Color GetBranchColor(UpgradeBranch branch)
    {
        switch (branch)
        {
            case UpgradeBranch.CPU: return cpuColor;
            case UpgradeBranch.MEMORY: return memoryColor;
            case UpgradeBranch.LOGIC: return logicColor;
            default:return Color.white;
        }
    }
}
