using UnityEngine;

public class HeatSink : MonoBehaviour
{
    private CoreStats coreStats;
    private BasicUpgrade thisUpgrade;

    private bool isBuffActive = false;
    private float buffTimer = 0f;
    private float lastIdleMilestone = 0f;

    private float activeBonusAmount = 0f;

    private void Start()
    {
        coreStats = CoreStats.Instance;
        thisUpgrade = GetComponent<BasicUpgrade>();
    }

    private void Update()
    {
        if (thisUpgrade == null || coreStats == null || thisUpgrade.UpgradeLevel() < 1)
            return;

        float idleTime = IdleTracker.Instance.IdleTime;
        int level = thisUpgrade.UpgradeLevel();

        // Milestone-based values
        float bonusPerLevel = GetBonusPerLevel(level) * level;      // % per level
        float buffDuration = GetBuffDuration(level);        // How long the buff lasts

        // Every 10s idle -> trigger 5s buff
        if (!isBuffActive && idleTime >= lastIdleMilestone + 10f)
        {
            activeBonusAmount = GetBonusPerLevel(level) * level;

            coreStats.AddStat("PercentBitRate", activeBonusAmount);
            // UnityEngine.Debug.Log($"[HeatSink] Applied +{activeBonusAmount}% BitRate for {buffDuration}s (Lvl {level})");

            buffTimer = buffDuration;
            isBuffActive = true;
            lastIdleMilestone += 10f;

            LogPrinter.Instance?.PrintLog($"Heat Sink Bonus Is ACTIVATED For {buffTimer}s",BranchType.CPU);
        }

        // Countdown and remove buff after 5s
        if (isBuffActive)
        {
            buffTimer -= Time.deltaTime;
            if (buffTimer <= 0f)
            {
                coreStats.AddStat("PercentBitRate", -activeBonusAmount);
                // UnityEngine.Debug.Log($"[HeatSink] Removed +{activeBonusAmount}% BitRate");

                isBuffActive = false;
            }
        }

        // Reset milestone if player becomes active again
        if (idleTime < 0.1f)
        {
            lastIdleMilestone = 0f;
        }
    }

    private float GetBonusPerLevel(int level)
    {
        if (level >= 50) return 2f;
        if (level >= 25) return 1.5f;
        return 1f;
    }

    private float GetBuffDuration(int level)
    {
        if (level >= 100) return 8f;
        if (level >= 5) return 6f;
        return 5f;
    }
}
