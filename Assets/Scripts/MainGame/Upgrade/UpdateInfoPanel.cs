using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateInfoPanel : MonoBehaviour
{
    [Header("UI Text Fields")]
    public TMP_Text titleText;
    public TMP_Text levelText;
    public TMP_Text costText;
    public TMP_Text effectText;

    public static UpdateInfoPanel Instance;

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
    }
}
