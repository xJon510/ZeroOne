using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class BitRemoveTest : MonoBehaviour
{
    public int bitsToRemove = 100;
    public Button removeBitsButton;

    
    private void Start()
    {
        if (removeBitsButton != null)
        {
            removeBitsButton.onClick.AddListener(RemoveBits);
        }
    }

    private void RemoveBits()
    {
        var grids = BitManager.Instance.activeGrids;
        ulong total = BitManager.Instance.currentBits;

        if (total == 0 || grids.Count == 0) return;

        float totalFloat = (float)total;
        int remaining = bitsToRemove;

        Dictionary<BitGridManager, int> removalMap = new Dictionary<BitGridManager, int>();

        foreach (var grid in grids)
        {
            ulong gridBits = grid.GetLocalBitValue();
            float ratio = gridBits / totalFloat;
            int toRemove = Mathf.FloorToInt(ratio * bitsToRemove);
            removalMap[grid] = toRemove;
            remaining -= toRemove;
        }

        foreach (var grid in grids)
        {
            if (remaining <= 0) break;
            if (grid.GetLocalBitValue() > (ulong)removalMap[grid])
            {
                removalMap[grid] += 1;
                remaining -= 1;
            }
        }

        foreach (var kvp in removalMap)
        {
            BitGridManager grid = kvp.Key;
            int toRemove = kvp.Value;
            ulong current = grid.GetLocalBitValue();
            ulong newValue = (ulong)Mathf.Max(0, (long)current - toRemove);
            SetGridBits(grid, newValue);   
        }

        UnityEngine.Debug.Log($"[BitRemoveTest] Proportionally removed {bitsToRemove} bits.");

    }

    private void SetGridBits(BitGridManager grid, ulong newValue)
    {
        var field = typeof(BitGridManager).GetField("localBitValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(grid, newValue);
        }

        var method = typeof(BitGridManager).GetMethod("UpdateDebugText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (method != null)
        {
            method.Invoke(grid, null);
        }
    }
}
