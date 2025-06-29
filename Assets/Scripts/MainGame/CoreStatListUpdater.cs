using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoreStatListUpdater : MonoBehaviour
{
    [Header("Stat Row Prefab")]
    public GameObject coreStatRowPrefab;

    [Header("UI Container")]
    public Transform statListContainer;

    [Header("Theme Colors")]
    public Color basicColor;
    public Color cpuColor;
    public Color memoryColor;
    public Color logicColor;

    [Header("Spacer Prefab")]
    public GameObject branchSpacerPrefab;

    private readonly Dictionary<StatBranch, GameObject> spacerMap = new();

    private Dictionary<string, GameObject> statRowMap = new();

    private readonly StatBranch[] displayOrder = {
    StatBranch.BASIC,
    StatBranch.CPU,
    StatBranch.LOGIC,
    StatBranch.MEM
};

    void Start()
    {
        // Clear any stale map
        spacerMap.Clear();

        for (int i = 0; i < displayOrder.Length - 1; i++)
        {
            StatBranch branch = displayOrder[i];

            // Find the pre-made spacer in the container by name
            Transform found = statListContainer.Find($"Spacer_{branch}");
            if (found != null)
            {
                spacerMap[branch] = found.gameObject;
            }
            else
            {
                Debug.LogWarning($"[CoreStatListUpdater] Missing prefab Spacer_{branch} in container!");
            }
        }
    }

    void OnEnable()
    {
        CoreStats.OnStatChanged += HandleStatChanged;
    }

    void OnDisable()
    {
        CoreStats.OnStatChanged -= HandleStatChanged;
    }

    private void HandleStatChanged(string statName, float newValue)
    {
        if (statRowMap.ContainsKey(statName))
        {
            // Update existing row's value
            var row = statRowMap[statName];
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
            foreach (var text in texts)
            {
                if (text.gameObject.name.ToLower().Contains("value"))
                    text.text = newValue.ToString("F2");
            }
        }
        else
        {
            GameObject newRow = Instantiate(coreStatRowPrefab);
            newRow.name = statName + "_Row";

            // Set text & color
            Color branchColor = GetBranchColor(statName);

            TMP_Text[] texts = newRow.GetComponentsInChildren<TMP_Text>();
            foreach (var text in texts)
            {
                if (text.gameObject.name.ToLower().Contains("category"))
                    text.text = statName;
                else if (text.gameObject.name.ToLower().Contains("value"))
                    text.text = newValue.ToString("F2");

                text.color = branchColor;
            }

            Image rowImage = newRow.GetComponentInChildren<Image>();
            if (rowImage != null)
                rowImage.color = branchColor;

            // Find the branch for this stat
            StatBranch branch = CoreStats.Instance.GetAllStats()[statName].branch;

            // MEM: place at end without a spacer
            if (branch == StatBranch.MEM)
            {
                newRow.transform.SetParent(statListContainer, false);
                newRow.transform.SetAsLastSibling();
            }
            else
            {
                Transform spacer = statListContainer.Find($"Spacer_{branch}");
                if (spacer != null)
                {
                    int spacerIndex = spacer.GetSiblingIndex();
                    newRow.transform.SetParent(statListContainer, false);
                    newRow.transform.SetSiblingIndex(spacerIndex);
                }
                else
                {
                    // fallback
                    newRow.transform.SetParent(statListContainer, false);
                    newRow.transform.SetAsLastSibling();
                }
            }

            statRowMap[statName] = newRow;
        }
    }

    private Color GetBranchColor(string statName)
    {
        if (CoreStats.Instance == null) return basicColor;

        var stats = CoreStats.Instance.GetAllStats();
        if (stats.TryGetValue(statName, out var data))
        {
            return data.branch switch
            {
                StatBranch.CPU => cpuColor,
                StatBranch.MEM => memoryColor,
                StatBranch.LOGIC => logicColor,
                _ => basicColor,
            };
        }

        return basicColor;
    }
}
