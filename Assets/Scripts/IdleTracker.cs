using UnityEngine;

public class IdleTracker : MonoBehaviour
{
    public static IdleTracker Instance { get; private set; }

    public float IdleTime { get; private set; }

    private Vector3 lastMousePosition;
    private float lastInputTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        lastMousePosition = Input.mousePosition;
        lastInputTime = Time.time;
    }

    private void Update()
    {
        // Detect movement or input
        if (Input.anyKeyDown || Input.mousePosition != lastMousePosition)
        {
            lastInputTime = Time.time;
            lastMousePosition = Input.mousePosition;
        }

        // Update idle time
        IdleTime = Time.time - lastInputTime;
    }
}