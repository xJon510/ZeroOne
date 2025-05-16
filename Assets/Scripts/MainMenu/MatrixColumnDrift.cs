using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixColumnDrift : MonoBehaviour
{
    public float fallSpeed = 1f; // units per second
    public bool isBrightColumn = false;

    private float destroyY;

    private RectTransform rect;
    private RectTransform parentRect;
    private MatrixSpawner spawner;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponentInParent<RectTransform>();
        spawner = FindObjectOfType<MatrixSpawner>();

        fallSpeed *= UnityEngine.Random.Range(0.8f, 1.2f); // slight variation per column

        // Get world bottom of parent background
        Vector3[] corners = new Vector3[4];
        parentRect.GetWorldCorners(corners);
        float bottomY = corners[0].y;

        // Convert column height to world units
        float columnHeight = rect.rect.height * parentRect.lossyScale.y;
        destroyY = bottomY - columnHeight * 2.5f;
    }

    void Update()
    {
        if (rect == null || spawner == null) return;

        Vector3 pos = rect.position;
        pos.y -= fallSpeed * Time.deltaTime;
        rect.position = pos;

        if (rect.position.y < destroyY)
        {
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        gameObject.SetActive(false); // Hide until reused
        if (isBrightColumn)
            spawner.ReturnToBrightPool(gameObject);
        else
            spawner.ReturnToFadedPool(gameObject);
    }
}
