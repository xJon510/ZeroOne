using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScreenToggle : MonoBehaviour
{
    [Header("Upgrade Panel")]
    public CanvasGroup upgradeUI;
    public Button openButton;
    public Button closeButton;

    [Header("Audio Panel")]
    public CanvasGroup audioUI;
    public Button openAudioButton;
    public Button closeAudioButton;

    void Start()
    {
        if (openButton != null && closeButton != null)
        {
            openButton.onClick.AddListener(OpenUpgradeUI);
            closeButton.onClick.AddListener(CloseUpgradeUI);
        }

        // New Audio listeners
        if (openAudioButton != null && closeAudioButton != null)
        {
            openAudioButton.onClick.AddListener(OpenAudioUI);
            closeAudioButton.onClick.AddListener(CloseAudioUI);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (upgradeUI != null && upgradeUI.alpha > 0)
            {
                CloseUpgradeUI();
            }

            if (audioUI != null && audioUI.alpha > 0)
            {
                CloseAudioUI();
            }
        }
    }

    void OpenUpgradeUI()
    {
        if (upgradeUI != null)
        {
            SetCanvasGroupState(upgradeUI, true);
        }
    }

    void CloseUpgradeUI()
    {
        if (upgradeUI != null)
        {
            SetCanvasGroupState(upgradeUI, false);
        }
    }

    void OpenAudioUI()
    {
        if (audioUI != null)
        {
            SetCanvasGroupState(audioUI, true);
        }
    }

    void CloseAudioUI()
    {
        if (audioUI != null)
        {
            SetCanvasGroupState(audioUI, false);
        }
    }

    void SetCanvasGroupState(CanvasGroup cg, bool visible)
    {
        cg.alpha = visible ? 1 : 0;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }
}
