using UnityEngine;

public class SweptCache : MonoBehaviour
{
    private CoreStats coreStats;
    private BasicUpgrade thisUpgrade;

    private void Start()
    {
        coreStats = CoreStats.Instance;
        thisUpgrade = GetComponent<BasicUpgrade>();

        // Ensure SweptCache stat exists
        if (thisUpgrade != null && thisUpgrade.UpgradeLevel() >= 1)
        {
            coreStats.AddStat("SweptCache", 0f, StatBranch.LOGIC);
        }
    }

    private void Update()
    {
        if (thisUpgrade == null || coreStats == null) return;

        // Keep level cap up to date if the upgrade level changes
        if (thisUpgrade.UpgradeLevel() >= 1 && !coreStats.GetAllStats().ContainsKey("SweptCache"))
        {
            coreStats.AddStat("SweptCache", 0f, StatBranch.LOGIC);
        }
    }

    public int GetLevelCap()
    {
        return thisUpgrade != null ? thisUpgrade.UpgradeLevel() : 0;
    }
}