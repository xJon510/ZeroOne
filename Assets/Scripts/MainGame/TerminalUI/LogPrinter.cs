using System;
using UnityEngine;
using TMPro;

public class LogPrinter : MonoBehaviour
{
    public static LogPrinter Instance;

    [Header("Prefabs")]
    public GameObject cpuLogPrefab;
    public GameObject memLogPrefab;
    public GameObject logicLogPrefab;
    public GameObject errorLogPrefab;

    [Header("Scroll View Content")]
    public Transform contentParent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PrintLog(string message, BranchType branch, bool isError = false)
    {
        GameObject prefabToUse = null;

        if (isError)
        {
            prefabToUse = errorLogPrefab;
        }
        else
        {
            switch (branch)
            {
                case BranchType.CPU:
                    prefabToUse = cpuLogPrefab;
                    break;
                case BranchType.MEM:
                    prefabToUse = memLogPrefab;
                    break;
                case BranchType.LOGIC:
                    prefabToUse = logicLogPrefab;
                    break;
                default:
                    Debug.LogError("[LogPrinter] Unknown BranchType!");
                    return;
            }
        }

        if (prefabToUse == null)
        {
            Debug.LogError($"[LogPrinter] Prefab for {branch} is not assigned!");
            return;
        }

        // Instantiate and parent it under the ScrollView Content
        GameObject newLog = Instantiate(prefabToUse, contentParent);

        // Find TMP_Text and set it
        TMP_Text textComponent = newLog.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            string formattedTime = GetFormattedRunTime();
            textComponent.text = $"{formattedTime} {message}";
        }
        else
        {
            Debug.LogWarning("[LogPrinter] No TMP_Text found in log prefab!");
        }
    }

    private string GetFormattedRunTime()
    {
        if (BitManager.Instance == null)
        {
            return "[00:00]";
        }

        float runTime = BitManager.Instance.runTime;
        int hours = Mathf.FloorToInt(runTime / 3600f);
        int minutes = Mathf.FloorToInt((runTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(runTime % 60f);

        return $"[{hours:00}:{minutes:00}:{seconds:00}]";
    }

    public void ClearLog()
    {
        if (contentParent == null)
        {
            Debug.LogWarning("[LogPrinter] Content parent is not assigned!");
            return;
        }

        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }

        Debug.Log("[LogPrinter] Cleared all logs from Scroll View.");
    }
}
