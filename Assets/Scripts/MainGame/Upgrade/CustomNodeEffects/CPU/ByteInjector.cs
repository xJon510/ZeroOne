using UnityEngine;

public class ByteInjector : MonoBehaviour
{
    private CoreStats coreStats;
    private BasicUpgrade upgrade;

    private bool isBoostActive = false;
    private float boostTimer = 0f;
    private float cooldownTimer = 0f;

    private float lastPermanentBonus = 0f;
    private float activeBoostAmount = 0f;

    void Start()
    {
        coreStats = CoreStats.Instance;
        upgrade = GetComponent<BasicUpgrade>();
    }

    void Update()
    {
        if (upgrade == null || coreStats == null || upgrade.UpgradeLevel() < 1) return;

        int level = upgrade.UpgradeLevel();

        // Handle permanent bonus at level >= 100
        if (level >= 100)
        {
            float permanentBonus = GetBonusPerLevel(level) * level;
            if (!Mathf.Approximately(lastPermanentBonus, permanentBonus))
            {
                float delta = permanentBonus - lastPermanentBonus;
                coreStats.AddStat("FlatBitRate", delta);
                //Debug.Log($"[ByteInjector] Permanent +{delta} BitRate applied (Lvl {level})");
                LogPrinter.Instance?.PrintLog($"Byte Injector Permanent +{activeBoostAmount} BitRate", BranchType.CPU);

                lastPermanentBonus = permanentBonus;
            }

            // Disable the periodic buff once permanent is active
            isBoostActive = false;
            return;
        }

        // Countdown cooldown timer
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f && !isBoostActive)
        {
            activeBoostAmount = GetBonusPerLevel(level) * level;

            coreStats.AddStat("FlatBitRate", activeBoostAmount);
            //Debug.Log($"[ByteInjector] Injected +{activeBoostAmount} BitRate for {GetBoostDuration(level)}s (Lvl {level})");
            LogPrinter.Instance?.PrintLog($"Byte Injector INJECTED {activeBoostAmount} BitRate for {GetBoostDuration(level)}s", BranchType.CPU);

            boostTimer = GetBoostDuration(level);
            isBoostActive = true;

            cooldownTimer = GetCooldownTime(level);
        }

        if (isBoostActive)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                coreStats.AddStat("FlatBitRate", -activeBoostAmount);
                //Debug.Log($"[ByteInjector] Removed +{activeBoostAmount} BitRate");

                isBoostActive = false;
                activeBoostAmount = 0f;
            }
        }
    }

    private float GetBonusPerLevel(int level)
    {
        return (level >= 50) ? 4f : 2f;
    }

    private float GetBoostDuration(int level)
    {
        float duration = 10f;
        if (level >= 25) duration += 3f; // +3s duration at level 25
        return duration;
    }

    private float GetCooldownTime(int level)
    {
        float cooldown = 20f;
        if (level >= 5) cooldown -= 3f;
        if (level >= 100) cooldown -= 4f; // Applies only if below 100; safe here for logic clarity
        return cooldown;
    }
}
