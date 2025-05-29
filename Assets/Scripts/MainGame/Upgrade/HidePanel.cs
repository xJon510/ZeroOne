using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidePanel : MonoBehaviour
{
    [Header("Panel to Move")]
    public RectTransform panel;

    [Header("Move Positions")]
    public Vector2 visiblePosition; // Optional if you want to bring it back later
    public Vector2 hiddenPosition;

    [Header("Animation")]
    public float moveDuration = 0.3f;

    public void Hide()
    {
        if (panel != null)
        {
            StopAllCoroutines();
            StartCoroutine(MovePanel(panel.anchoredPosition, hiddenPosition));
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
