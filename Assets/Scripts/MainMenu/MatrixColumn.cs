using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using static System.Net.Mime.MediaTypeNames;

public class MatrixColumn : MonoBehaviour
{
    private TextMeshProUGUI[] charLines;
    public Color trailColor = new Color(0f, 1f, 0f, 1f); // Green
    public Color headColor = Color.white;

    void Start()
    {
        // Get children from top to bottom (child 0 = top)
        int childCount = transform.childCount;
        charLines = new TextMeshProUGUI[childCount];

        for (int i = 0; i < childCount; i++)
        {
            charLines[i] = transform.GetChild(i).GetComponent<TextMeshProUGUI>();
        }

        ApplyGradient();
    }

    void ApplyGradient()
    {
        int total = charLines.Length;

        for (int i = 0; i < total; i++)
        {
            float t = (float)Math.Pow((float)i / (total - 1), 3f); // 0 (top) -> 1 (bottom)
            Color c = Color.Lerp(trailColor, headColor, t);

            var text = charLines[i];
            text.color = c;
        }
    }
}
