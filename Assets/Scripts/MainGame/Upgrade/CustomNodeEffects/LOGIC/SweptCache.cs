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
        if (thisUpgrade == null) return 0;

        int level = thisUpgrade.UpgradeLevel();

        if (level >= 100) return level * 4;
        if (level >= 25) return level * 3;
        if (level >= 5) return level * 2;

        return level * 1; // Below level 5, base 1×
    }
}