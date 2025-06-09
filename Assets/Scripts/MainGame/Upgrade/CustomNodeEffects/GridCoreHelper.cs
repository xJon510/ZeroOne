using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCoreHelper : MonoBehaviour
{
    public GameObject gridToActivate;
    private bool activated = false;

    public void TryActivateGrid()
    {
        if (!activated && gridToActivate != null)
        {
            gridToActivate.SetActive(true);
            activated = true;

            StartCoroutine(RegisterWithBitManagerDelayed());
        }
    }

    private IEnumerator RegisterWithBitManagerDelayed()
    {
        yield return null; // wait one frame to let Start() finish in BitGridManager

        BitGridManager gridManager = gridToActivate.GetComponent<BitGridManager>();
        if (gridManager != null && !BitManager.Instance.activeGrids.Contains(gridManager))
        {
            BitManager.Instance.activeGrids.Add(gridManager);
            BitManager.Instance.RefreshGridBitrates();
            UnityEngine.Debug.Log($"[GridCoreHelper] Added {gridManager.name} to BitManager.activeGrids (delayed)");
        }
        else if (gridManager == null)
        {
            UnityEngine.Debug.LogWarning($"[GridCoreHelper] No BitGridManager found on {gridToActivate.name} after delay");
        }
    }
}