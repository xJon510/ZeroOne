using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidePanel : MonoBehaviour
{
    private CoreNodePanelSwitcher panelSwitcher;

    [Header("Panel to Move")]
    public RectTransform panel;

    [Header("Move Positions")]
    public Vector2 visiblePosition;
    public Vector2 hiddenPosition;

    [Header("Animation")]
    public float moveDuration = 0.3f;

    void Start()
    {
        if (panelSwitcher == null)
            panelSwitcher = FindObjectOfType<CoreNodePanelSwitcher>();
    }

    public void Hide()
    {
        if (panel != null)
        {
            StopAllCoroutines();
            StartCoroutine(MovePanel(panel.anchoredPosition, hiddenPosition));

            UnityEngine.Debug.Log(panelSwitcher != null);
            if (panelSwitcher != null)
            {
                panelSwitcher.currentPanel = ActivePanel.None;
                UnityEngine.Debug.Log("(ME) Panel set to None");
            }
        }
    }

    public void Show()
    {
        if (panel != null)
        {
            StopAllCoroutines();
            StartCoroutine(MovePanel(panel.anchoredPosition, visiblePosition));
        }
    }

    private IEnumerator MovePanel(Vector2 startPos, Vector2 endPos)
    {
        float elapsed = 0f;

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
