using System.IO;
using UnityEngine;

public class HideResetButton : MonoBehaviour
{
    [Header("Target Slot")]
    public int slotID = 1; // 1–4

    [Header("Reset Button to Toggle")]
    public GameObject resetButton;

    private string savePath => Application.persistentDataPath + "/saves/";

    void Start()
    {
        CheckSlotStatus();
    }

    public void CheckSlotStatus()
    {
        string path = savePath + $"slot{slotID}.json";

        if (!File.Exists(path))
        {
            resetButton.SetActive(false);
            return;
        }

        string json = File.ReadAllText(path);
        SlotMeta data = JsonUtility.FromJson<SlotMeta>(json);

        if (data == null || data.playTime <= 0f)
        {
            // Slot is basically empty/default
            resetButton.SetActive(false);
        }
        else
        {
            resetButton.SetActive(true);
        }
    }

    [System.Serializable]
    private class SlotMeta
    {
        public int saveSlot;
        public string lastSaveTime;
        public float playTime;
    }
}