using UnityEngine;
using UnityEngine.EventSystems;

public class SlotButton : MonoBehaviour, ISelectHandler
{
    [SerializeField] private int slotID;

    public void OnSelect(BaseEventData eventData)
    {
        PlayerPrefs.SetInt("SelectedSaveSlot", slotID);
        PlayerPrefs.Save();
        UnityEngine.Debug.Log("Selected Save Slot: " + slotID);
    }
}