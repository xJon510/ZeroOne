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
    public ulong winCondition = 83886080;
    private ulong maxBits = ulong.MaxValue;

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
    }

    private void Update()
    {
        timer += Time.deltaTime;
        runTime += Time.deltaTime;

        if (timer >= tickRate)
        {
            timer -= tickRate;
            Tick();
        }

        UpdateRunTimeText();
    }

    private void Tick()
    {
        float bitRatePerGrid = globalBitRate / (activeGrids.Count - 5);
        float totalBitRate = bitRatePerGrid * (activeGrids.Count - 5);

        currentBits += (ulong)totalBitRate;
        if (currentBits > maxBits)
            currentBits = maxBits;
    }

    [ContextMenu("Refresh Grid Bitrates")]
    public void RefreshGridBitrates()
    {
        foreach (BitGridManager grid in activeGrids)
        {
            grid.SetBitrateSource(() => globalBitRate / (activeGrids.Count - 5));
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


}
