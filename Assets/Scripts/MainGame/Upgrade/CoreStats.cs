using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatBranch { BASIC, CPU, MEM, LOGIC }

public class StatData
{
    public float value;
    public StatBranch branch;

    public StatData(float val, StatBranch br)
    {
        value = val;
        branch = br;
    }
}

public class CoreStats : MonoBehaviour
{
    public static event System.Action<string, float> OnStatChanged;

    private void Start()
    {
        AddStat("FlatBitRate", 1f, StatBranch.BASIC);
        AddStat("PercentBitRate", 0f, StatBranch.BASIC);
    }

    public static CoreStats Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private Dictionary<string, StatData> statMap = new();

    public void AddStat(string statName, float amount, StatBranch branch = StatBranch.BASIC)
    {
        if (!statMap.ContainsKey(statName))
            statMap[statName] = new StatData(0f, branch);

        statMap[statName].value += amount;

        UnityEngine.Debug.Log($"[CoreStats] Added {amount} to {statName}. New total: {statMap[statName].value}");

        // Fire the event
        OnStatChanged?.Invoke(statName, statMap[statName].value);
    }

    public float GetStat(string statName) => statMap.TryGetValue(statName, out var data) ? data.value : 0f;

    public Dictionary<string, StatData> GetAllStats() => statMap;
}



