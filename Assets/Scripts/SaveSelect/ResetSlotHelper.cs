using UnityEngine;
using UnityEngine.UI;

public class ResetSlotHelper : MonoBehaviour
{
    public GameObject objectToActivate;

    private Button button;

    void Awake()
    {
        // Get the Button component on this GameObject
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("ResetSlotHelper requires a Button component on the same GameObject.");
            return;
        }

        // Add listener to the button click event
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No object assigned to activate in ResetSlotHelper.");
        }
    }
}
