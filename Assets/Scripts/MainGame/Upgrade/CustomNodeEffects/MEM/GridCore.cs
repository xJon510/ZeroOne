using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GridCore : MonoBehaviour
{
    [Header("Upgrade Binding")]
    public string upgradeName;
    public Transform bitContainer;
    public GameObject bitPrefab;

    [Header("Grid Height Settings")]
    public RectTransform parentRect;
    public List<int> milestoneLevels = new List<int> { 5, 10, 25, 50, 100};
    public List<float> presetHeights; // You can set these manually in inspector for each milestone level
    public List<float> presetYPositions;

    private int currentMilestoneIndex = -1;

    public void CheckUpgradeMilestone(int currentLevel)
    {
        if (currentMilestoneIndex + 1 < milestoneLevels.Count)
        {
            int nextMilestone = milestoneLevels[currentMilestoneIndex + 1];
            if (currentLevel >= nextMilestone)
            {
                UnityEngine.Debug.Log($"[GridCore] Milestone {nextMilestone} reached.");

                switch (nextMilestone)
                {
                    case 5:
                        AddBits(8);
                        break;
                    case 10:
                        break;
                    case 25:
                    case 50:
                    case 100:
                        AddBits(16);
                        break;
                    default:
                        UnityEngine.Debug.LogWarning($"[GridCore] Unhandled milestone: {nextMilestone}");
                        break;
                }

                ApplyVisualChange(currentMilestoneIndex + 1);
                currentMilestoneIndex++;
            }
        }
    }

    private void AddBits(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(bitPrefab, bitContainer);
        }

        // Notify any BitGridManager components on this object to refresh
        foreach (var manager in GetComponents<BitGridManager>())
        {
            manager.RefreshBitCharacters();
            UnityEngine.Debug.Log($"[GridCore] Refreshed BitGridManager after adding {count} bits.");
        }
    }

    private void ApplyVisualChange(int index)
    {
        if (parentRect != null)
        {
            if (index < presetHeights.Count)
                parentRect.sizeDelta = new Vector2(parentRect.sizeDelta.x, presetHeights[index]);

            if (index < presetYPositions.Count)
                parentRect.anchoredPosition = new Vector2(parentRect.anchoredPosition.x, presetYPositions[index]);
        }
    }
}