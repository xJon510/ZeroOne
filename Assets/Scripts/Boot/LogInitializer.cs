using System.IO;
using UnityEngine;

public class LogInitializer : MonoBehaviour
{
    private const int totalSlots = 4;

    void Start()
    {
        InitializeAllLogs();
    }

    public static void InitializeAllLogs()
    {
        string saveDir = Application.persistentDataPath + "/saves/";
        if (!Directory.Exists(saveDir))
            Directory.CreateDirectory(saveDir);

        for (int i = 1; i <= totalSlots; i++)
        {
            string path = saveDir + $"slot{i}.json";
            if (!File.Exists(path))
            {
                string defaultLog = GenerateDefaultSaveLog(i);
                File.WriteAllText(path, defaultLog);
                UnityEngine.Debug.Log($"[LogInitializer] Created default log for slot {i}");
            }
        }
    }

    static string GenerateDefaultSaveLog(int slotID)
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        return
    $@"{{
  ""saveSlot"": {slotID},
  ""lastSaveTime"": ""{timestamp}"",
  ""playTime"": 0.0,

  ""coreStats"": {{
    ""globalBitCount"": 0,
    ""globalBitRate"": 1.0
  }},

  ""upgrades"": {{
    ""cpu"": [
      {{ ""name"": ""CorePulse"", ""level"": 0, ""path"": ""cpu"" }}
    ],
    ""mem"": [
      {{ ""name"": ""GridCoreAlpha"", ""level"": 0, ""path"": ""mem"" }}
    ],
    ""logic"": [
      {{ ""name"": ""SystemSweep"", ""level"": 0, ""path"": ""logic"" }}
    ]
  }},

  ""bitGrids"": [
    {{
      ""gridID"": 0,
      ""bitValue"": 0
    }}
  ]
}}";
    }
}
