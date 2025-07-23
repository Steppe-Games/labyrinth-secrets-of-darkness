using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Common.Input_System {

    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {

        [SerializeField] private UnityEvent<bool> onValueChanged = new();

        public void OnPointerDown(PointerEventData eventData) {
            onValueChanged.Invoke(true);
        }

        public void OnPointerUp(PointerEventData eventData) {
            onValueChanged.Invoke(false);
        }

        public void OnPointerExit(PointerEventData eventData) {
            onValueChanged.Invoke(false);
        }

    }

}