using UnityEngine;

public class ThrottleDelay : MonoBehaviour
{
    private CoreStats coreStats;
    private BasicUpgrade thisUpgrade;

    private float lastAppliedBonus = 0f;

    void Start()
    {
        coreStats = CoreStats.Instance;
        thisUpgrade = GetComponent<BasicUpgrade>();
    }

    void Update()
    {
        if (coreStats == null || thisUpgrade == null || thisUpgrade.UpgradeLevel() < 1)
            return;

        int level = thisUpgrade.UpgradeLevel();
        float idleTime = IdleTracker.Instance.IdleTime;

        // === Milestone-based step and max ===
        float idleStep = 5f;
        float maxIdle = 25f;

        if (level >= 5)
            maxIdle += 5f;
        if (level >= 50)
            maxIdle += 10f;

        if (level >= 25)
            idleStep -= 1f; // 5s -> 4s
        if (level >= 100)
            idleStep -= 1f; // 4s -> 3s

        // Clamp to reasonable min step
        idleStep = Mathf.Max(3f, idleStep);

        // Calculate how many steps fit in current idle time
        float effectiveIdleTime = Mathf.Min(idleTime, maxIdle);
        int stepCount = Mathf.FloorToInt(effectiveIdleTime / idleStep);

        // Calculate new bonus
        float newBonus = stepCount * 0.1f * level;

        // Apply delta if changed
        if (!Mathf.Approximately(newBonus, lastAppliedBonus))
        {
            float delta = newBonus - lastAppliedBonus;
            coreStats.AddStat("PercentBitRate", delta);
            lastAppliedBonus = newBonus;

            //Debug.Log($"[ThrottleDelay] Applied delta: {delta:F2}% | Total: {newBonus:F2}% (Steps: {stepCount}, StepSize: {idleStep}s, MaxIdle: {maxIdle}s)");

            LogPrinter.Instance?.PrintLog($"Throttle Delay Bonus: {newBonus:F2}% BitRate ({stepCount} Steps @ {idleStep}s, Idle: {effectiveIdleTime:F1}s)", BranchType.CPU);
        }

        // Reset if player is active again
        if (idleTime < 0.1f && lastAppliedBonus > 0f)
        {
            coreStats.AddStat("PercentBitRate", -lastAppliedBonus);
            lastAppliedBonus = 0f;

            //Debug.Log($"[ThrottleDelay] Reset bonus to 0 due to activity.");
            LogPrinter.Instance?.PrintLog($"Throttle Delay Bonus Reset Due To Activity.", BranchType.CPU);
        }
    }
}
