using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject brightColumnPrefab;
    public GameObject fadedColumnPrefab;

    [Header("Spawn Settings")]
    public float spawnDelay = 0.2f;
    public float brightSpawnChance = 0.4f;
    public int initialPoolSize = 20;

    private RectTransform backgroundRect;
    private float leftBound;
    private float rightBound;
    private float topY;

    private Vector2 lastBackgroundSize;

    private Queue<GameObject> brightColumnPool = new Queue<GameObject>();
    private Queue<GameObject> fadedColumnPool = new Queue<GameObject>();

    void Start()
    {
        backgroundRect = GetComponent<RectTransform>();

        InitializePool();
        UpdateBounds();

        // Warm-up burst
        for (int i = 0; i < 30; i++)
        {
            SpawnColumn(withYOffset: true);
        }

        StartCoroutine(SpawnColumnsForever());

        lastBackgroundSize = backgroundRect.rect.size;
    }

    void Update()
    {
        if (backgroundRect.rect.size != lastBackgroundSize)
        {
            lastBackgroundSize = backgroundRect.rect.size;
            UpdateBounds();
        }
    }

    void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject bright = Instantiate(brightColumnPrefab, transform);
            bright.SetActive(false);
            brightColumnPool.Enqueue(bright);

            GameObject faded = Instantiate(fadedColumnPrefab, transform);
            faded.SetActive(false);
            fadedColumnPool.Enqueue(faded);
        }
    }

    IEnumerator SpawnColumnsForever()
    {
        while (true)
        {
            float x = UnityEngine.Random.Range(leftBound, rightBound);
            Vector3 spawnPos = new Vector3(x, topY, transform.position.z);

            GameObject column;
            if (UnityEngine.Random.value < brightSpawnChance)
                column = GetFromPool(brightColumnPool, brightColumnPrefab);
            else
                column = GetFromPool(fadedColumnPool, fadedColumnPrefab);

            column.transform.position = spawnPos;
            column.SetActive(true);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    GameObject GetFromPool(Queue<GameObject> pool, GameObject prefab)
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        else
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            return obj;
        }
    }

    void UpdateBounds()
    {
        Vector3[] corners = new Vector3[4];
        backgroundRect.GetWorldCorners(corners);

        leftBound = corners[0].x;
        rightBound = corners[3].x;
        topY = transform.position.y;
    }

    // Optionally, public functions for columns to return themselves to the pool
    public void ReturnToBrightPool(GameObject column)
    {
        column.SetActive(false);
        brightColumnPool.Enqueue(column);
    }

    public void ReturnToFadedPool(GameObject column)
    {
        column.SetActive(false);
        fadedColumnPool.Enqueue(column);
    }

    void SpawnColumn(bool withYOffset)
    {
        float x = UnityEngine.Random.Range(leftBound, rightBound);
        float y = topY;

        if (withYOffset)
            y += UnityEngine.Random.Range(-600f, 0f);

        Vector3 spawnPos = new Vector3(x, y, transform.position.z);
        GameObject column = UnityEngine.Random.value < brightSpawnChance
            ? GetFromPool(brightColumnPool, brightColumnPrefab)
            : GetFromPool(fadedColumnPool, fadedColumnPrefab);

        column.transform.position = spawnPos;
        column.SetActive(true);
    }
}
