using TMPro;
using UnityEngine;

public class LoadingDots : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    public string baseText = "Loading";
    private float timer;
    private int dotCount;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 0.2f)
        {
            dotCount = (dotCount + 1) % 4;
            loadingText.text = baseText + new string('.', dotCount);
            timer = 0f;
        }
    }
}