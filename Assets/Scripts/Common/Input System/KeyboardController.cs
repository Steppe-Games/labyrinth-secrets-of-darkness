using UnityEngine;
using UnityEngine.InputSystem;

namespace Common.Input_System {

    public class KeyboardController : MonoBehaviour {

        //@formatter:off
        [Header("Input Actions")]
        [SerializeField] private InputActionReference upAction;
        [SerializeField] private InputActionReference downAction;
        [SerializeField] private InputActionReference leftAction;
        [SerializeField] private InputActionReference rightAction;
        //@formatter:on

        private void OnEnable() {
            // Подписываемся на события нажатия клавиш
            if (upAction != null) {
                upAction.action.performed += OnUpPressed;
                upAction.action.Enable();
            }

            if (downAction != null) {
                downAction.action.performed += OnDownPressed;
                downAction.action.Enable();
            }

            if (leftAction != null) {
                leftAction.action.performed += OnLeftPressed;
                leftAction.action.Enable();
            }

            if (rightAction != null) {
                rightAction.action.performed += OnRightPressed;
                rightAction.action.Enable();
            }
        }

        private void OnDisable() {
            // Отписываемся от событий
            if (upAction != null) {
                upAction.action.performed -= OnUpPressed;
                upAction.action.Disable();
            }

            if (downAction != null) {
                downAction.action.performed -= OnDownPressed;
                downAction.action.Disable();
            }

            if (leftAction != null) {
                leftAction.action.performed -= OnLeftPressed;
                leftAction.action.Disable();
            }

            if (rightAction != null) {
                rightAction.action.performed -= OnRightPressed;
                rightAction.action.Disable();
            }
        }

        // Обработчики событий
        private void OnUpPressed(InputAction.CallbackContext context) {
            Up();
        }

        private void OnDownPressed(InputAction.CallbackContext context) {
            Down();
        }

        private void OnLeftPressed(InputAction.CallbackContext context) {
            Left();
        }

        private void OnRightPressed(InputAction.CallbackContext context) {
            Right();
        }

        public void Up() {
            InputSystemChannels.Up.Execute();
        }

        public void Down() {
            InputSystemChannels.Down.Execute();
        }

        public void Left() {
            InputSystemChannels.Left.Execute();
        }

        public void Right() {
            InputSystemChannels.Right.Execute();
        }
        
        

    }

}