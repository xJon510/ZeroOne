using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string saveFolder => Application.persistentDataPath + "/saves/";
    private float autoSaveInterval = 60f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // Optional, in case you want this to persist
    }

    private void Start()
    {
        StartCoroutine(AutoSaveTimer());
    }

    IEnumerator AutoSaveTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            SaveGame();
        }
    }

    public void SaveGame()
    {
        int slot = PlayerPrefs.GetInt("SelectedSaveSlot", -1);
        if (slot == -1)
        {
            UnityEngine.Debug.LogWarning("[SaveManager] No valid save slot selected.");
            return;
        }

        GameState saveData = new GameState();
        saveData.saveSlot = slot;
        saveData.lastSaveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        saveData.playTime = BitManager.Instance.runTime;

        // Core Stats
        saveData.CoreStats = new CoreStatsSaveData
        {
            globalBitCount = BitManager.Instance.currentBits,
            globalBitRate = BitManager.Instance.globalBitRate
        };

        // Upgrades
        var upgradeTree = new UpgradeTree();
        foreach (var upgrade in UpgradeTrackerManager.Instance.GetAllValidUpgrades())
        {
            var data = new UpgradeSaveData
            {
                name = upgrade.upgradeName,
                level = upgrade.level,
                path = upgrade.pathType
            };

            switch (upgrade.pathType.ToLower())
            {
                case "cpu": upgradeTree.cpu.Add(data); break;
                case "mem": upgradeTree.mem.Add(data); break;
                case "logic": upgradeTree.logic.Add(data); break;
            }
        }
        saveData.upgrades = upgradeTree;

        // BitGrids
        saveData.bitGrids = new List<BitGridSaveData>();
        for (int i = 0; i < BitManager.Instance.activeGrids.Count; i++)
        {
            BitGridManager grid = BitManager.Instance.activeGrids[i];
            BitGridSaveData gridData = new BitGridSaveData
            {
                gridID = i,
                bitValue = grid.GetLocalBitValue()
            };
            saveData.bitGrids.Add(gridData);
        }

        // Save to file
        string path = saveFolder + $"slot{slot}.json";
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(path, json);
        UnityEngine.Debug.Log($"[SaveManager] Saved to {path}");
    }
}
