using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class CoCScreenMovement : MonoBehaviour
{
    [SerializeField] private RectTransform upgradeHolder;
    [SerializeField] private RectTransform boundsRect; // This should be UpgradeBkRnd
    [SerializeField] private float padding = 200f;

    public float sensitvity = 2.5f;

    private Vector3 lastMousePosition;
    private bool isDragging;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
            isDragging = true;
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

        float halfX = boundsSize.x / 2f;
        float halfY = boundsSize.y / 2f;

        float clampedX = Mathf.Clamp(targetPos.x, -halfX + 10f, halfX - 10f);
        float clampedY = Mathf.Clamp(targetPos.y, -halfY + 10f, halfY - 10f);

        return new Vector2(clampedX, clampedY);
    }
}
