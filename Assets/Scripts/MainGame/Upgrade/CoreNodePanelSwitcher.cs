using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActivePanel
{
    UpgradeInfo,
    CoreStats
}

public class CoreNodePanelSwitcher : MonoBehaviour
{
    public RectTransform upgradeInfoPanel;
    public RectTransform coreStatsPanel;

    public Vector2 upgradeInfoHiddenPos;
    public Vector2 upgradeInfoVisiblePos;

    public Vector2 coreStatsHiddenPos;
    public Vector2 coreStatsVisiblePos;

    public float moveDuration = 0.3f;

    public ActivePanel currentPanel = ActivePanel.UpgradeInfo;

    public void ShowCoreStats()
    {
        if (currentPanel == ActivePanel.CoreStats)
            return;

        StopAllCoroutines();
        StartCoroutine(MovePanel(upgradeInfoPanel, upgradeInfoVisiblePos, upgradeInfoHiddenPos));
        StartCoroutine(MovePanel(coreStatsPanel, coreStatsHiddenPos, coreStatsVisiblePos));

        currentPanel = ActivePanel.CoreStats;
    }

    public void ShowUpgradeInfo()
    {
        if (currentPanel == ActivePanel.UpgradeInfo)
            return;

        StopAllCoroutines();
        StartCoroutine(MovePanel(upgradeInfoPanel, upgradeInfoHiddenPos, upgradeInfoVisiblePos));
        StartCoroutine(MovePanel(coreStatsPanel, coreStatsVisiblePos, coreStatsHiddenPos));

        currentPanel = ActivePanel.UpgradeInfo;
    }

    private IEnumerator MovePanel(RectTransform panel, Vector2 startPos, Vector2 endPos)
    {
        if (panel.anchoredPosition == endPos) yield break;
    
        float elapsed = 0f;
        panel.anchoredPosition = startPos;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            panel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        panel.anchoredPosition = endPos;
    }
}