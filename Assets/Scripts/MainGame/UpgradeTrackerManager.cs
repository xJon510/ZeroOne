using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTrackerManager : MonoBehaviour
{
    public static UpgradeTrackerManager Instance;

    public static event System.Action<string, int, string> OnUpgradeRecorded;

    [System.Serializable]
    public class UpgradeRecord
    {
        public string upgradeName;
        public int level;
        public string pathType;
        public BasicUpgrade upgradeComponent;

        public UpgradeRecord(string name, int lvl, string path, BasicUpgrade component)
        {
            upgradeName = name;
            level = lvl;
            pathType = path;
            upgradeComponent = component;
        }
    }

    private Dictionary<string, UpgradeRecord> allUpgrades = new Dictionary<string, UpgradeRecord>();
    private List<UpgradeRecord> cpuUpgrades = new List<UpgradeRecord>();
    private List<UpgradeRecord> memUpgrades = new List<UpgradeRecord>();
    private List<UpgradeRecord> logicUpgrades = new List<UpgradeRecord>();
    private List<UpgradeRecord> trackedUpgrades = new List<UpgradeRecord>();

    private void Start()
    {
        AutoRegisterAllUpgradesInScene();
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RecordUpgrade(string upgradeName, int level, string pathType, BasicUpgrade component = null)
    {
        var existing = trackedUpgrades.Find(r => r.upgradeName == upgradeName);

        if (existing != null)
        {
            existing.level = level;
            if (component != null)
                existing.upgradeComponent = component;
        }
        else
        {
            trackedUpgrades.Add(new UpgradeRecord(upgradeName, level, pathType, component));
        }

        OnUpgradeRecorded?.Invoke(upgradeName, level, pathType);
    }

    // Getters
    public List<UpgradeRecord> GetCPUUpgrades() => cpuUpgrades;
    public List<UpgradeRecord> GetMemUpgrades() => memUpgrades;
    public List<UpgradeRecord> GetLogicUpgrades() => logicUpgrades;
    public List<UpgradeRecord> GetAllValidUpgrades()
    {
        return trackedUpgrades.FindAll(r => r.level >= 1 && r.upgradeComponent != null);
    }

    public UpgradeRecord GetUpgradeByName(string name)
    {
        return allUpgrades.ContainsKey(name) ? allUpgrades[name] : null;
    }

    public void AutoRegisterAllUpgradesInScene()
    {
        var upgrades = GameObject.FindObjectsOfType<BasicUpgrade>();

        foreach (var upgrade in upgrades)
        {
            if (upgrade.upgradeInfo == null) continue;

            string name = upgrade.upgradeName;
            int level = upgrade.currentLevel;
            string pathType = upgrade.upgradeInfo.upgradeBranch.ToString().ToLower();

            RecordUpgrade(name, level, pathType);
            UnityEngine.Debug.Log($"[AutoRegister] Registered upgrade: {name}, Level: {level}, Path: {pathType}");
        }
    }
}