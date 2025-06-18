using UnityEngine.SceneManagement;
using UnityEngine;

public class Back2MainMenu : MonoBehaviour
{
    [SerializeField] private float delayBeforeLoad = 0.1f; // tweak in inspector

    public void LoadScene()
    {
        PlayerPrefs.SetInt("SkipBoot", 1);
        PlayerPrefs.Save();
        StartCoroutine(DelayedLoad());
    }

    private System.Collections.IEnumerator DelayedLoad()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene("TitleScreen");
    }
}
