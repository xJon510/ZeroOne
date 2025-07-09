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

    [Header("Exit Panel")]
    public CanvasGroup exitUI;
    public Button openExitButton;
    public Button closeExitButton;

    [Header("Achievement Panel")]
    public CanvasGroup achievementUI;
    public Button openAchievementButton;
    public Button closeAchievementButton;

    [Header("Credits Panel")]
    public CanvasGroup creditsUI;
    public Button openCreditsButton;
    public Button closeCreditsButton;

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

        if (openExitButton != null && closeExitButton != null)
        {
            openExitButton.onClick.AddListener(OpenExitUI);
            closeExitButton.onClick.AddListener(CloseExitUI);
        }

        if (openAchievementButton != null && closeAchievementButton != null)
        {
            openAchievementButton.onClick.AddListener(OpenAchievementUI);
            closeAchievementButton.onClick.AddListener(CloseAchievementUI);
        }

        if (openCreditsButton != null && closeCreditsButton != null)
        {
            openCreditsButton.onClick.AddListener(OpenCreditsUI);
            closeCreditsButton.onClick.AddListener(CloseCreditsUI);
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
            if (exitUI != null && exitUI.alpha > 0)
            {
                CloseExitUI();
            }
            if (achievementUI != null && achievementUI.alpha > 0)
            {
                CloseAchievementUI();
            }
            if (creditsUI != null && creditsUI.alpha > 0)
            {
                CloseCreditsUI();
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

    void OpenExitUI()
    {
        if (exitUI != null)
        {
            SetCanvasGroupState(exitUI, true);
        }
    }

    void CloseExitUI()
    {
        if (exitUI != null)
        {
            SetCanvasGroupState(exitUI, false);
        }
    }

    void OpenAchievementUI()
    {
        if (achievementUI != null)
        {
            SetCanvasGroupState(achievementUI, true);
        }
    }

    void CloseAchievementUI()
    {
        if (achievementUI != null)
        {
            SetCanvasGroupState(achievementUI, false);
        }
    }

    void OpenCreditsUI()
    {
        if (creditsUI != null)
        {
            SetCanvasGroupState(creditsUI, true);
        }
    }

    void CloseCreditsUI()
    {
        if (creditsUI != null)
        {
            SetCanvasGroupState(creditsUI, false);
        }
    }
}
