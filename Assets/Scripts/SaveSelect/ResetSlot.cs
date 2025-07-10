using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ResetSlot : MonoBehaviour
{
    [Header("Slot Info")]
    public int slotID = 1; // The slot number to reset (1-4)

    [Header("Popup Elements")]
    public GameObject popupObject; // The popup to show/hide
    public Button confirmButton;
    public Button cancelButton;

    [Header("Hide Reset Button")]
    public HideResetButton hideResetButton;
    public LoadSlotData loadSlotData; // Reference to your LoadSlotData

    private string savePath => Application.persistentDataPath + "/saves/";

    void Start()
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmReset);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelReset);
    }

    public void OpenPopup(int targetSlot)
    {
        slotID = targetSlot;
        if (popupObject != null)
            popupObject.SetActive(true);
    }

    void ConfirmReset()
    {
        string path = savePath + $"slot{slotID}.json";

        if (File.Exists(path))
            File.Delete(path);

        // Re-initialize this slot only
        string defaultLog = LogInitializer.GenerateDefaultSaveLog(slotID);
        File.WriteAllText(path, defaultLog);

        Debug.Log($"[ResetSlot] Slot {slotID} has been reset.");

        // Refresh the slot UI
        if (loadSlotData != null)
            loadSlotData.Start(); // re-run Start to reload slot meta

        if (hideResetButton != null)
            hideResetButton.CheckSlotStatus();

        CancelReset();
    }

    void CancelReset()
    {
        if (popupObject != null)
            popupObject.SetActive(false);
    }
}
