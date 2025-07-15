using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheTune : MonoBehaviour
{
    private CoreStats coreStats;
    private SweptCache sweptCache;
    private BasicUpgrade thisUpgrade;

    private int upgradeLevel = 0;
    private float lastBonus = 0f;

    void Start()
    {
        coreStats = CoreStats.Instance;
        sweptCache = FindObjectOfType<SweptCache>();
        thisUpgrade = GetComponent<BasicUpgrade>();

        if (coreStats == null)
            Debug.LogWarning("[CacheTune] CoreStats not found!");

        if (sweptCache == null)
            Debug.LogWarning("[CacheTune] SweptCache not found!");

        if (thisUpgrade == null)
            Debug.LogWarning("[CacheTune] No BasicUpgrade found on this GameObject!");
    }

    void Update()
    {
        if (coreStats == null || sweptCache == null || thisUpgrade == null) return;

        upgradeLevel = thisUpgrade.currentLevel;
        if (upgradeLevel <= 0) return;

        float fillAmount = coreStats.GetStat("SweptCache");
        float capacity = sweptCache.GetLevelCap();

        float fillRatio = (capacity > 0f) ? Mathf.Clamp01(fillAmount / capacity) : 0f;

        // Determine base per-level %
        float perLevelBonus = 0.5f;
        if (upgradeLevel >= 5) perLevelBonus = 1f;
        if (upgradeLevel >= 50) perLevelBonus = 2f;

        float levelBonus = perLevelBonus * upgradeLevel * fillRatio;

        // Capacity milestone bonus
        float capacityBonus = 0f;
        if (upgradeLevel >= 25)
        {
            float perCap = (upgradeLevel >= 100) ? 50f : 100f;
            capacityBonus = Mathf.Floor(capacity / perCap) * 10f;
        }

        float totalBonus = levelBonus + capacityBonus;

        // Apply delta to PercentBitRate
        coreStats.AddStat("PercentBitRate", totalBonus - lastBonus, StatBranch.LOGIC);
        lastBonus = totalBonus;

        Debug.Log($"[CacheTune] FillRatio: {fillRatio:F2}, LevelBonus: {levelBonus:F2}, CapacityBonus: {capacityBonus:F2}, Total: {totalBonus:F2}");
    }
}
