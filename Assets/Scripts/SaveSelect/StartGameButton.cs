using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public void StartGame()
    {
        int selectedSlot = PlayerPrefs.GetInt("SelectedSaveSlot", -1);
        if (selectedSlot == -1)
        {
            UnityEngine.Debug.LogWarning("No save slot selected!");
            return;
        }

        SceneManager.LoadScene("MainGame");
    }
}