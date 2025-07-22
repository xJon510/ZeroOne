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

    [Header("Tutorial Panel")]
    public CanvasGroup tutorialUI;
    public Button closetutorialButton;

    [Header("UI SFX")]
    public ButtonSFX buttonSFX;

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

        if (closetutorialButton != null)
        {
            closetutorialButton.onClick.AddListener(CloseTutorialUI);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool anyClosed = false;

            if (upgradeUI != null && upgradeUI.alpha > 0)
            {
                CloseUpgradeUI();
                anyClosed = true;
            }

            if (audioUI != null && audioUI.alpha > 0)
            {
                CloseAudioUI();
                anyClosed = true;
            }
            if (exitUI != null && exitUI.alpha > 0)
            {
                CloseExitUI();
                anyClosed = true;
            }
            if (achievementUI != null && achievementUI.alpha > 0)
            {
                CloseAchievementUI();
                anyClosed = true;
            }
            if (creditsUI != null && creditsUI.alpha > 0)
            {
                CloseCreditsUI();
                anyClosed = true;
            }
            if (tutorialUI != null && tutorialUI.alpha > 0)
            {
                CloseTutorialUI();
                anyClosed = true;
            }

            if (anyClosed && buttonSFX != null)
            {
                buttonSFX.PlayButtonDown();
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

    public void CloseTutorialUI()
    {
        if (tutorialUI != null)
        {
            SetCanvasGroupState(tutorialUI, false);

            PlayerPrefs.SetInt("TutorialSeen", 1);
            PlayerPrefs.Save();
        }
    }

    public void CloseAllUIPanels()
    {
        if (upgradeUI != null) SetCanvasGroupState(upgradeUI, false);
        if (audioUI != null) SetCanvasGroupState(audioUI, false);
        if (exitUI != null) SetCanvasGroupState(exitUI, false);
        if (achievementUI != null) SetCanvasGroupState(achievementUI, false);
        if (creditsUI != null) SetCanvasGroupState(creditsUI, false);
        if (tutorialUI != null) SetCanvasGroupState(tutorialUI, false);
    }
}
