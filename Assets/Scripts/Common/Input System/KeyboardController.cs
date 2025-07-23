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

        [Header("WASD Action")]
        [SerializeField] private InputActionReference wasdAction;
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

            if (wasdAction != null) {
                wasdAction.action.performed += OnWASDInput;
                wasdAction.action.canceled += OnWASDInput;
                wasdAction.action.Enable();
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
            
            if (wasdAction != null) {
                wasdAction.action.performed -= OnWASDInput;
                wasdAction.action.canceled -= OnWASDInput;
                wasdAction.action.Enable();
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

        public void ToggleUp(bool value) {
            InputSystemChannels.UpPressed.Value = value;
        }

        public void ToggleDown(bool value) {
            InputSystemChannels.DownPressed.Value = value;
        }

        public void ToggleLeft(bool value) {
            InputSystemChannels.LeftPressed.Value = value;
        }

        public void ToggleRight(bool value) {
            InputSystemChannels.RightPressed.Value = value;
        }
        
        private void OnWASDInput(InputAction.CallbackContext context) {
            Vector2 input = context.ReadValue<Vector2>();

            if (context.phase == InputActionPhase.Performed) {
                InputSystemChannels.LeftPressed.Value = input.x < -0.5f;
                InputSystemChannels.RightPressed.Value = input.x > 0.5f;
                InputSystemChannels.UpPressed.Value = input.y > 0.5f;
                InputSystemChannels.DownPressed.Value = input.y < -0.5f;
            } else if (context.phase == InputActionPhase.Canceled) {
                InputSystemChannels.LeftPressed.Value = input.x < -0.5f;
                InputSystemChannels.RightPressed.Value = input.x > 0.5f;
                InputSystemChannels.UpPressed.Value = input.y > 0.5f;
                InputSystemChannels.DownPressed.Value = input.y < -0.5f;
            }
        }

    }

}