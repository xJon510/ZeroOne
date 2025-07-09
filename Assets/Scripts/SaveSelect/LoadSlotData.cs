using System.IO;
using UnityEngine;
using TMPro;
using System;

public class LoadSlotData : MonoBehaviour
{
    [System.Serializable]
    public class SlotUI
    {
        public TMP_Text playTimeText;
        public TMP_Text dateText;
    }

    public SlotUI[] slotUIs = new SlotUI[4];

    private string savePath => Application.persistentDataPath + "/saves/";

    [Serializable]
    private class SlotMeta
    {
        public int saveSlot;
        public string lastSaveTime;
        public float playTime;
    }

    public void Start()
    {
        for (int i = 1; i <= 4; i++)
        {
            string path = savePath + $"slot{i}.json";

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                SlotMeta data = JsonUtility.FromJson<SlotMeta>(json);

                // Format time: 00:00:00
                TimeSpan ts = TimeSpan.FromSeconds(data.playTime);
                string formattedPlayTime = string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);

                // Format date: MM/dd/yy
                DateTime parsedDate;
                string formattedDate = "";
                if (DateTime.TryParse(data.lastSaveTime, out parsedDate))
                {
                    formattedDate = parsedDate.ToString("MM/dd/yy");
                }

                // Apply to UI
                int index = i - 1;
                if (slotUIs[index] != null)
                {
                    if (slotUIs[index].playTimeText != null)
                        slotUIs[index].playTimeText.text = formattedPlayTime;

                    if (slotUIs[index].dateText != null)
                        slotUIs[index].dateText.text = formattedDate;
                }
            }
        }
    }
}