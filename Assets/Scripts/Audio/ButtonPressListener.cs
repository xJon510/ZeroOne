using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPressListener : MonoBehaviour, IPointerDownHandler
{
    public ButtonSFX buttonSFX;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonSFX != null)
        {
            buttonSFX.PlayButtonDown();
        }
    }
}
