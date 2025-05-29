using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreStats : MonoBehaviour
{
    private Dictionary<string, float> statMap = new();

    public void AddStat(string statName, float amount)
    {
        if (!statMap.ContainsKey(statName))
            statMap[statName] = 0f;

        statMap[statName] += amount;
    }

    public float GetStat(string statName)
    {
        return statMap.TryGetValue(statName, out var value) ? value : 0f;
    }

    public Dictionary<string, float> GetAllStats() => statMap;
}

