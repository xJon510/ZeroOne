using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class SaveInitializer : MonoBehaviour
{
    public GameObject loadingScreen; // Assign in Inspector
    public GameObject upgradeUI;
    public float minLoadingTime = 0f;

    private void Start()
    {
        StartCoroutine(InitializeGameData());
    }

    IEnumerator InitializeGameData()
    {
        yield return null;

        if (loadingScreen != null) loadingScreen.SetActive(true);

        float startTime = Time.time;

        int selectedSlot = PlayerPrefs.GetInt("SelectedSaveSlot", -1);
        if (selectedSlot == -1)
        {
            UnityEngine.Debug.LogWarning("[SaveInitializer] No save slot selected!");
            yield break;
        }

        string filePath = Path.Combine(Application.persistentDataPath + "/saves", $"slot{selectedSlot}.json");
        if (!File.Exists(filePath))
        {
            UnityEngine.Debug.LogWarning($"[SaveInitializer] No save file found at {filePath}");
            yield break;
        }

        string json = File.ReadAllText(filePath);
        GameState loadedState = JsonUtility.FromJson<GameState>(json);

        // Set Core Stats
        BitManager.Instance.currentBits = loadedState.CoreStats.globalBitCount;
        BitManager.Instance.globalBitRate = loadedState.CoreStats.globalBitRate;

        if (loadedState.CoreStats.overflowBits > 0f)
        {
            BitManager.Instance.overflowBits = loadedState.CoreStats.overflowBits;
        }

        BitManager.Instance.AddToRunTime(loadedState.playTime);

        float loadedSweptCacheValue = loadedState.CoreStats.sweptCache;
        if (loadedSweptCacheValue > 0f)
        {
            CoreStats.Instance.AddStat("SweptCache", loadedSweptCacheValue, StatBranch.LOGIC);
        }

        // Apply Upgrades
        ApplyCPUUpgrades(loadedState.upgrades.cpu);
        ApplyUpgrades(loadedState.upgrades.mem);
        ApplyUpgrades(loadedState.upgrades.logic);

        // Apply BitGrids
        for (int i = 0; i < BitManager.Instance.activeGrids.Count && i < loadedState.bitGrids.Count; i++)
        {
            ulong savedValue = loadedState.bitGrids[i].bitValue;
            BitManager.Instance.activeGrids[i].SetLocalBitValue(savedValue);
        }

        // Ensure loading screen stays up for minimum duration
        float elapsed = Time.time - startTime;
        if (elapsed < minLoadingTime)
        {
            yield return new WaitForSeconds(minLoadingTime - elapsed);
        }

        if (upgradeUI != null) upgradeUI.SetActive(false);
        if (loadingScreen != null) loadingScreen.SetActive(false);

        UnityEngine.Debug.Log($"(ME) {loadedState.CoreStats.globalBitRate} ");

        UnityEngine.Debug.Log("[SaveInitializer] Save loaded successfully.");
    }

    void ApplyUpgrades(List<UpgradeSaveData> upgrades)
    {
        foreach (var upgrade in upgrades)
        {
            BasicUpgrade[] allUpgrades = FindObjectsOfType<BasicUpgrade>(true);
            foreach (var u in allUpgrades)
            {
                if (u.upgradeName == upgrade.name)
                {
                    for (int i = 0; i < upgrade.level; i++)
                    {
                        u.ApplyUpgrade(CoreStats.Instance);
                    }

                    // Ensure visuals reflect unlocked status if level >= 1
                    if (upgrade.level >= 1)
                    {
                        u.UnlockLevelOne();
                    }
                }
            }
        }
    }

    void ApplyCPUUpgrades(List<UpgradeSaveData> upgrades)
    {
        // Apply upgrade levels
        BasicUpgrade[] allUpgrades = FindObjectsOfType<BasicUpgrade>(true);

        foreach (var upgrade in upgrades)
        {
            foreach (var u in allUpgrades)
            {
                if (u.upgradeName == upgrade.name)
                {
                    for (int i = 0; i < upgrade.level; i++)
                    {
                        u.ApplyUpgrade(CoreStats.Instance);
                    }

                    if (upgrade.level >= 1)
                    {
                        u.UnlockLevelOne();
                    }
                }
            }
        }


        // Apply CPU discount to upgrade cost visuals
        float cpuDiscount = CoreStats.Instance.GetStat("CPU Discount");
        if (cpuDiscount <= 0f) return;

        foreach (var u in allUpgrades)
        {
                float rawCost = u.GetUpgradeCost(u.currentLevel);
                float finalCost = rawCost - (rawCost * (cpuDiscount / 100f));
                u.upgradeInfo.upgradeCost = finalCost;
        }
    }
}
