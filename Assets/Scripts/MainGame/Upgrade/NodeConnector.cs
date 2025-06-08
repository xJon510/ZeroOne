using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeConnector : MonoBehaviour
{
    [Header("Tier Info")]
    public int tier;

    [Header("Hexagonal Neighbors")]
    public NodeConnector north;
    public NodeConnector northEast;
    public NodeConnector northWest;
    public NodeConnector south;
    public NodeConnector southEast;
    public NodeConnector southWest;

    private bool hasReceivedPulse = false;

    // Gets all connected neighbors
    public List<NodeConnector> GetConnectedNodes()
    {
        List<NodeConnector> neighbors = new List<NodeConnector>();

        if (north) neighbors.Add(north);
        if (northEast) neighbors.Add(northEast);
        if (northWest) neighbors.Add(northWest);
        if (south) neighbors.Add(south);
        if (southEast) neighbors.Add(southEast);
        if (southWest) neighbors.Add(southWest);

        return neighbors;
    }

    // Placeholder unlock next tier method
    //public void UnlockNextTier()
    //{
    //    foreach (var node in GetConnectedNodes())
    //    {
    //        if (node != null)
    //        {
    //            // Placeholder: you can expand with specific unlock logic later
    //            //UnityEngine.Debug.Log($"[UnlockNextTier] Unlocking node at tier {node.tier}");
    //            // node.Unlock(); // if you implement such a method
    //        }
    //    }
    //}

    // Pulse spreader with level decay
    public void SpreadUpgradePulse(int strength)
    {
        if (strength <= 0 || hasReceivedPulse)
            return;

        hasReceivedPulse = true;

        var upgrade = GetComponent<UpgradeInfo>();
        if (upgrade != null)
        {
            upgrade.currentLevel += strength;
            UnityEngine.Debug.Log($"[SpreadUpgradePulse] {upgrade.upgradeName} upgraded by {strength}, now at level {upgrade.currentLevel}");
        }

        foreach (var node in GetConnectedNodes())
        {
            if (node != null)
            {
                node.SpreadUpgradePulse(strength - 1);
            }
        }
    }

    // Optional: call this before starting a new pulse
    public void ResetPulseFlags()
    {
        hasReceivedPulse = false;
        foreach (var node in GetConnectedNodes())
        {
            if (node != null && node.hasReceivedPulse)
            {
                node.ResetPulseFlags();
            }
        }
    }
}
