using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    [Header("No Slot Selected Popup")]
    public GameObject noSlotPopup;
    public float pulseTime = 0.05f;
    public float pulseScale = 1.15f;
    public float fadeDuration = 2.5f;

    [Header("Error SFX")]
    public NoSlotPop errorSFX;

    private Vector3 originalScale;
    private CanvasGroup popupCanvasGroup;

    private void Awake()
    {
        if (noSlotPopup != null)
        {
            originalScale = noSlotPopup.transform.localScale;

            // Ensure there's a CanvasGroup for fade
            popupCanvasGroup = noSlotPopup.GetComponent<CanvasGroup>();
            if (popupCanvasGroup == null)
                popupCanvasGroup = noSlotPopup.AddComponent<CanvasGroup>();

            noSlotPopup.SetActive(false);
        }
    }

    public void StartGame()
    {
        int selectedSlot = PlayerPrefs.GetInt("SelectedSaveSlot", -1);
        if (selectedSlot == -1)
        {
            ShowNoSlotPopup();
            return;
        }

        SceneManager.LoadScene("MainGame");
    }

    private void ShowNoSlotPopup()
    {
        if (noSlotPopup == null) return;

        StopAllCoroutines(); // Reset any running animations
        noSlotPopup.SetActive(true);

        // Reset to default
        noSlotPopup.transform.localScale = originalScale;
        popupCanvasGroup.alpha = 1f;

        StartCoroutine(AnimatePopup());
    }

    private System.Collections.IEnumerator AnimatePopup()
    {
        // Hold normal for 0.2s
        yield return new WaitForSeconds(0.2f);

        // Pulse up
        float elapsed = 0f;
        while (elapsed < pulseTime)
        {
            float t = elapsed / pulseTime;
            noSlotPopup.transform.localScale = Vector3.Lerp(originalScale, originalScale * pulseScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Make sure it's exactly at pulseScale
        noSlotPopup.transform.localScale = originalScale * pulseScale;

        // Shrink back down
        float shrinkTime = 0.2f;
        elapsed = 0f;
        while (elapsed < shrinkTime)
        {
            float t = elapsed / shrinkTime;
            noSlotPopup.transform.localScale = Vector3.Lerp(originalScale * pulseScale, originalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        noSlotPopup.transform.localScale = originalScale;

        // Hold normal for a bit before fading out
        yield return new WaitForSeconds(0.2f);

        // Fade out
        float elapsedFade = 0f;
        while (elapsedFade < fadeDuration)
        {
            popupCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedFade / fadeDuration);
            elapsedFade += Time.deltaTime;
            yield return null;
        }

        popupCanvasGroup.alpha = 0f;
        noSlotPopup.SetActive(false);
    }
}
