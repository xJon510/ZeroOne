using UnityEngine;

public class EchoPulse : MonoBehaviour
{
    private BasicUpgrade thisUpgrade;
    private int lastRunTimeSecond = -1;

    private void Awake()
    {
        thisUpgrade = GetComponent<BasicUpgrade>();
    }

    private void Update()
    {
        if (thisUpgrade == null || thisUpgrade.currentLevel < 1) return;

        int currentRunTimeSecond = Mathf.FloorToInt(BitManager.Instance.runTime);

        if (currentRunTimeSecond > lastRunTimeSecond)
        {
            lastRunTimeSecond = currentRunTimeSecond;
            TryEchoPulse();
        }
    }

    private void TryEchoPulse()
    {
        int level = thisUpgrade.currentLevel;

        float chancePerLevel = 0.025f;
        if (level >= 100) chancePerLevel = 0.5f;
        else if (level >= 50) chancePerLevel = 0.25f;
        else if (level >= 25) chancePerLevel = 0.1f;
        else if (level >= 5) chancePerLevel = 0.05f;

        float critChance = level * chancePerLevel / 100f;

        if (Random.value <= critChance)
        {
            float extraBits = BitManager.Instance.globalBitRate;
            BitManager.Instance.AddBufferedBits(extraBits);

            // Debug.Log($"[EchoPulse] Echo Pulse triggered! +{extraBits:F2} Bits.");
            LogPrinter.Instance?.PrintLog($"Echo Pulse Triggered! +{extraBits:F2} Bits.",BranchType.CPU);
        }

        // Always update static stat for display
        float displayChance = level * chancePerLevel;
        float oldValue = CoreStats.Instance.GetStat("Echo Pulse");
        float delta = displayChance - oldValue;
        CoreStats.Instance.AddStat("Echo Pulse", delta, StatBranch.CPU);
    }
}
