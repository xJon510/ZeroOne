using UnityEngine;

public class RuntimeCheck : MonoBehaviour
{
    private BasicUpgrade upgrade;

    private float lastPercentBitRate = 0f;

    void Awake()
    {
        upgrade = GetComponent<BasicUpgrade>();
    }

    void Update()
    {
        if (upgrade == null || upgrade.currentLevel < 1) return;

        int level = upgrade.currentLevel;

        int gridCount = BitManager.Instance.GetActiveGridCount();
        bool isForgiven = level >= 100;
        bool useIfPath = isForgiven || gridCount % 2 == 0;

        float ratePerLevel = GetBitRatePerLevel(level, useIfPath);

        float newPercentBitRate = ratePerLevel * level;

        if (!Mathf.Approximately(newPercentBitRate, lastPercentBitRate))
        {
            float delta = newPercentBitRate - lastPercentBitRate;
            CoreStats.Instance.AddStat("PercentBitRate", delta);
            lastPercentBitRate = newPercentBitRate;

            Debug.Log($"[RuntimeCheck] Level:{level} | Grids:{gridCount} | IfPath:{useIfPath} | +{ratePerLevel * 100}%/lvl | Total:{newPercentBitRate}%");
        }
    }

    private float GetBitRatePerLevel(int level, bool useIfPath)
    {
        if (level >= 100) useIfPath = true;

        if (level >= 50) return useIfPath ? 4f : 1f;    // 4% or 1%
        if (level >= 25) return useIfPath ? 2f : 0.5f;   // 2% or 0.5%
        if (level >= 5) return useIfPath ? 1f : 0.25f;  // 1% or 0.25%

        return useIfPath ? 0.5f : 0.1f;                   // 0.5% or 0.1%
    }
}
