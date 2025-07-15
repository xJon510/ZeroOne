
using UnityEngine;

public class BufferPadding : MonoBehaviour
{
    public TMPro.TMP_Text totalBitsText;
    public TMPro.TMP_Text overflowBitsText;

    private CoreStats coreStats;
    private BasicUpgrade thisUpgrade;

    private float lastAppliedAmount = 0f;
    private bool hasActivatedOverflowUI = false;

    private void Start()
    {
        coreStats = CoreStats.Instance;
        thisUpgrade = GetComponent<BasicUpgrade>();
    }

    private void Update()
    {
        if (thisUpgrade == null || coreStats == null || thisUpgrade.UpgradeLevel() < 1)
            return;

        if (!hasActivatedOverflowUI && thisUpgrade.UpgradeLevel() >= 1)
        {
            if (totalBitsText != null)
            {
                // Shift down by, say, 40 units (adjust to your UI)
                Vector3 pos = totalBitsText.rectTransform.localPosition;
                pos.y -= -20f;
                totalBitsText.rectTransform.localPosition = pos;
            }

            if (overflowBitsText != null)
            {
                overflowBitsText.gameObject.SetActive(true);
            }

            hasActivatedOverflowUI = true;
        }

        int level = thisUpgrade.UpgradeLevel();
        float totalOverflowPercent = CalculateTotalOverflow(level);

        // Remove the old value before adding the new one
        if (Mathf.Abs(totalOverflowPercent - lastAppliedAmount) > 0.001f)
        {
            coreStats.AddStat("Overflow", -lastAppliedAmount, StatBranch.MEM);
            coreStats.AddStat("Overflow", totalOverflowPercent, StatBranch.MEM);
            lastAppliedAmount = totalOverflowPercent;

            // Debug.Log($"[BufferPadding] Applied {totalOverflowPercent}% overflow at level {level}");
        }
    }

    private float CalculateTotalOverflow(int level)
    {
        float bonusPerLevel = 0.1f;

        if (level >= 100)
            bonusPerLevel = 0.5f;
        else if (level >= 50)
            bonusPerLevel = 0.4f;
        else if (level >= 25)
            bonusPerLevel = 0.3f;
        else if (level >= 5)
            bonusPerLevel = 0.2f;
        else
            bonusPerLevel = 0.1f;

        return level * bonusPerLevel;
    }
}
