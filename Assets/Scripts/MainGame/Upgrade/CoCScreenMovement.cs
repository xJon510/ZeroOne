using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class CoCScreenMovement : MonoBehaviour
{
    [SerializeField] private RectTransform upgradeHolder;
    [SerializeField] private RectTransform boundsRect; // This should be UpgradeBkRnd
    [SerializeField] private CanvasGroup upgradeUI;

    public float sensitvity = 2.5f;

    private Vector3 lastMousePosition;
    private bool isDragging;

    void Update()
    {
        if (upgradeUI != null && upgradeUI.alpha == 0f)
        {
            // UI is hidden, skip drag!
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;

            float minX = 165f;
            float maxX = Screen.width - 165f;
            float minY = 145f;
            float maxY = Screen.height - 145f;

            if (mousePos.x >= minX && mousePos.x <= maxX &&
                mousePos.y >= minY && mousePos.y <= maxY)
            {
                lastMousePosition = mousePos;
                isDragging = true;
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 delta = currentMousePosition - lastMousePosition;

            upgradeHolder.anchoredPosition += (Vector2)(delta * sensitvity);
            upgradeHolder.anchoredPosition = ClampToBounds(upgradeHolder.anchoredPosition);

            lastMousePosition = currentMousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private Vector2 ClampToBounds(Vector2 targetPos)
    {
        Vector2 boundsSize = boundsRect.rect.size;

        float halfX = boundsSize.x / 4f;
        float halfY = boundsSize.y;

        float clampedX = Mathf.Clamp(targetPos.x, -halfX, halfX);
        float clampedY = Mathf.Clamp(targetPos.y, -halfY, halfY);

        return new Vector2(clampedX, clampedY);
    }

    //void OnGUI()      Debug Visual
    //{
    //    float minX = 165f;
    //    float maxX = Screen.width - 165f;
    //    float minY = 145f;
    //    float maxY = Screen.height - 145f;

    //    float width = maxX - minX;
    //    float height = maxY - minY;

    //    Color prevColor = GUI.color;
    //    GUI.color = new Color(0f, 0.2f, 1f, 0.2f); // semi-transparent blue
    //    GUI.DrawTexture(new Rect(minX, Screen.height - maxY, width, height), Texture2D.whiteTexture);
    //    GUI.color = prevColor;
    //}
}
