using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

namespace Soccer.UI
{
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler , IDragHandler
    {
        public OnScreenStick joystick;

        public void OnPointerDown(PointerEventData eventData)
        {
            joystick.OnPointerDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            joystick.OnPointerUp(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            joystick.OnDrag(eventData);
        }
    }
}