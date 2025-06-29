using UnityEngine;

public class CycleTrim : MonoBehaviour
{
    private BasicUpgrade upgrade;

    private float lastApplied = 0f;

    void Awake()
    {
        upgrade = GetComponent<BasicUpgrade>();
    }

    void Update()
    {
        if (upgrade != null)
        {
            if (upgrade.currentLevel < 1) return;

            float newDiscount = Mathf.Min(upgrade.currentLevel, 25);

            if (!Mathf.Approximately(lastApplied, newDiscount))
            {
                float delta = newDiscount - lastApplied;
                CoreStats.Instance.AddStat("CPU Discount", delta, StatBranch.CPU);
                lastApplied = newDiscount;
            }
        }
    }
}