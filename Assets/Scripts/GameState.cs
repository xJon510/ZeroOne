using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public int saveSlot;
    public string lastSaveTime;
    public float playTime;

    public CoreStatsSaveData CoreStats;
    public UpgradeTree upgrades;
    public List<BitGridSaveData> bitGrids;
}

[System.Serializable]
public class CoreStatsSaveData
{
    public ulong globalBitCount;
    public float globalBitRate;
}

[System.Serializable]
public class UpgradeSaveData
{
    public string name;
    public int level;
    public string path;
}

[System.Serializable]
public class UpgradeTree
{
    public List<UpgradeSaveData> cpu = new();
    public List<UpgradeSaveData> mem = new();
    public List<UpgradeSaveData> logic = new();
}

[System.Serializable]
public class BitGridSaveData
{
    public int gridID;
    public ulong bitValue;
}
