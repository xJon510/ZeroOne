using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadSaveSelect : MonoBehaviour
{
    [SerializeField] private float delayBeforeLoad = 0.1f; // tweak in inspector

    public void LoadScene()
    {
        PlayerPrefs.SetInt("SelectedSaveSlot", -1);
        StartCoroutine(DelayedLoad());
    }

    private System.Collections.IEnumerator DelayedLoad()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene("SaveSelection");
    }
}
