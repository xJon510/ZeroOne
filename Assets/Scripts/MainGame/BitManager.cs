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

    public List<BitGridManager> activeGrids = new List<BitGridManager>();

    public float globalBitRate = 1f; // Total bits to generate per tick (float for smoother distribution)

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
        float bitsPerGrid = globalBitRate / activeGrids.Count;
        float totalBits = 0f;

        foreach (BitGridManager grid in activeGrids)
        {
            grid.ReceiveBits(bitsPerGrid);
            totalBits += bitsPerGrid;
        }

        currentBits += (ulong)totalBits;

        if (currentBits > maxBits)
            currentBits = maxBits;

        UnityEngine.Debug.Log($"Ticked: +{totalBits}, now at {currentBits}");

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
}
