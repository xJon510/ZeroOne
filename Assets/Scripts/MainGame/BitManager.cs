using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public static event Action OnGameTick;

    public float overflowBits;

    [Header("Win UI")]
    public CanvasGroup winGameUIPanel;
    public Button continueButton;
    public Button returnToTitleButton;
    public TMP_Text DemoRunTimeText;
    public float demoWinCondition = 8388608f;
    private bool demoEnded = false;

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

        if (!demoEnded && currentBits + overflowBits >= (ulong)demoWinCondition)
        {
            TriggerDemoWin();
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
        OnGameTick?.Invoke();
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
            int totalSeconds = Mathf.FloorToInt(runTime);
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            runTimeText.text = $"{hours:00}:{minutes:00}:{seconds:00}";
        }
    }

    public void UpdateGlobalBitRate(float newRate)
    {
        globalBitRate = newRate;
        OnBitrateChanged?.Invoke(globalBitRate);
        RefreshGridBitrates();
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
        Debug.Log($"[BitManager] Received stat change: {statName} = {newValue}");

        float flat = CoreStats.Instance.GetStat("FlatBitRate");
        float swept = CoreStats.Instance.GetStat("SweptCache");
        float percent = CoreStats.Instance.GetStat("PercentBitRate");

        float combinedFlat = flat + swept;
        float combinedRate = combinedFlat * (1f + percent / 100f);

        UpdateGlobalBitRate(combinedRate);

        Debug.Log($"[BitManager] Global BitRate recalculated: Flat:{flat} + Swept:{swept} + {percent}% = {combinedRate}");
    }

    public void AddToRunTime(float savedSeconds)
    {
        runTime += savedSeconds;
        UpdateRunTimeText();
    }

    public void RecalculateTotalBits()
    {
        currentBits = (ulong)Mathf.Floor(overflowBits);
        foreach (BitGridManager grid in activeGrids)
        {
            currentBits += grid.GetLocalBitValue();
        }
    }

    public void AddBufferedBits(float amount)
    {
        overflowBits += amount;
        //Debug.Log($"[BitManager] Added {amount} overflow bits. Total bufferedBits: {overflowBits}");
    }

    public float GetOverflowBits()
    {
        return overflowBits;
    }

    void SetCanvasGroupState(CanvasGroup cg, bool visible)
    {
        cg.alpha = visible ? 1 : 0;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }

    public void TriggerDemoWin()
    {
        demoEnded = true;

        UpgradeScreenToggle toggle = FindObjectOfType<UpgradeScreenToggle>();
        if (toggle != null)
            toggle.CloseAllUIPanels();

        int totalSeconds = Mathf.FloorToInt(runTime);
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        if (hours > 0)
            DemoRunTimeText.text = $"In {hours:00}:{minutes:00}:{seconds:00} Time";
        else
            DemoRunTimeText.text = $"In {minutes:00}:{seconds:00} Time";

        if (winGameUIPanel != null)
            SetCanvasGroupState(winGameUIPanel, true);

        if (continueButton != null)
            continueButton.onClick.AddListener(() =>
            {
                SetCanvasGroupState(winGameUIPanel, false);
            });

        if (returnToTitleButton != null)
            returnToTitleButton.onClick.AddListener(() =>
            {
                SaveManager.Instance?.SendMessage("ExitToTitleScreen");
            });
    }
}
