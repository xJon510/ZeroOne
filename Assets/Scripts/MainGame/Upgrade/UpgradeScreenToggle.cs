using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScreenToggle : MonoBehaviour
{
    public GameObject upgradeUI;
    public Button openButton;
    public Button closeButton;

    void Start()
    {
        openButton.onClick.AddListener(OpenUpgradeUI);
        closeButton.onClick.AddListener(CloseUpgradeUI);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (upgradeUI.activeSelf)
            {
                CloseUpgradeUI();
            }
        }
    }

    void OpenUpgradeUI()
    {
        upgradeUI.SetActive(true);
    }

    void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
    }
}
