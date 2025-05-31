using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class BitManager : MonoBehaviour
{
    public static BitManager Instance;

    [SerializeField] public ulong currentBits = 0;
    public ulong winCondition = 838860800;

    public float runTime = 0f;
    public TMP_Text runTimeText;

    public float tickRate = 1.0f;
    private float timer;

    public static event Action<float> OnBitrateChanged;

    public List<BitGridManager> activeGrids = new List<BitGridManager>();

    public float globalBitRate = 1f; // Total bits to generate per tick (float for smoother distribution)

    private void Start()
    {
        UpdateGlobalBitRate(globalBitRate);
    }

    private void Awake()
    {
        Instance = this;
        CoreStats.OnStatChanged += HandleStatChanged;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        runTime += Time.deltaTime;

        currentBits = 0;
        foreach (BitGridManager grid in activeGrids)
        {
            currentBits += grid.GetLocalBitValue();
        }

        if (timer >= tickRate)
        {
            timer -= tickRate;
            Tick();
        }

        UpdateRunTimeText();
    }

    private void Tick()
    {
        currentBits = 0;
        foreach (BitGridManager grid in activeGrids)
        {
            currentBits += grid.GetLocalBitValue(); // You'll expose this method
        }
    }

    [ContextMenu("Refresh Grid Bitrates")]
    public void RefreshGridBitrates()
    {
        foreach (BitGridManager grid in activeGrids)
        {
            grid.SetBitrateSource(() => globalBitRate / activeGrids.Count);
        }
    }

    private void UpdateRunTimeText()
    {
        if (runTimeText != null)
        {
            int minutes = Mathf.FloorToInt(runTime / 60f);
            int seconds = Mathf.FloorToInt(runTime % 60f);
            runTimeText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void UpdateGlobalBitRate(float newRate)
    {
        globalBitRate = newRate;
        OnBitrateChanged?.Invoke(globalBitRate);
    }

    public int GetActiveGridCount()
    {
        int count = 0;
        foreach (BitGridManager grid in activeGrids)
        {
            if (!grid.IsAtGridCapacity())
                count++;
        }
        return count;
    }

    private void OnDestroy()
    {
        CoreStats.OnStatChanged -= HandleStatChanged;
    }

    private void HandleStatChanged(string statName, float newValue)
    {
        UnityEngine.Debug.Log($"[BitManager] Received stat change: {statName} = {newValue}");

        if (statName == "FlatBitRate")
        {
            UpdateGlobalBitRate(newValue);
            RefreshGridBitrates();
            UnityEngine.Debug.Log($"[BitManager] Global BitRate updated to {newValue} via CoreStats");
        }
    }
}
