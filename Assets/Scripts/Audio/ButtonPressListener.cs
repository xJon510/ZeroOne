using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPressListener : MonoBehaviour, IPointerDownHandler
{
    public ButtonSFX buttonSFX;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button != null && button.interactable)
        {
            buttonSFX.PlayButtonDown();
        }
    }
}
