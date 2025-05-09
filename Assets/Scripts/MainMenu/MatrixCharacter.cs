using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MatrixCharacter : MonoBehaviour
{
    public TextMeshProUGUI text;
    private float changeRate;

    void OnEnable()
    {
        if (text == null)
            text = GetComponent<TextMeshProUGUI>();

        changeRate = UnityEngine.Random.Range(0.1f, 0.25f);
        StartCoroutine(ChangeChar());
    }

    IEnumerator ChangeChar()
    {
        while (gameObject.activeSelf)
        {
            text.text = GetRandomChar();
            yield return new WaitForSeconds(changeRate);
        }
    }

    string GetRandomChar()
    {
        string chars = "0123456789@#$%&";
        return chars[UnityEngine.Random.Range(0, chars.Length)].ToString();
    }
}

