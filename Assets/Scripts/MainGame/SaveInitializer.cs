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
        BitManager.Instance.AddToRunTime(loadedState.playTime);

        // Apply Upgrades
        ApplyUpgrades(loadedState.upgrades.cpu);
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
                        UnityEngine.Debug.Log("[SaveInitializer] Unlocked Level 1 (ME)");
                    }
                }
            }
        }
    }
}
