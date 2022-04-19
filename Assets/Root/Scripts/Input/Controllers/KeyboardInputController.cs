using System;
using UnityEngine;

[Serializable]
public class KeyboardInputController {

    public static KeyCode[] GroupKeyCodes = null;

    private InputController inputController;

    static KeyboardInputController() {
        KeyboardInputController.GroupKeyCodes = new KeyCode[] {
            KeyCode.Alpha0,
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9
        };
    }

    public void Initialize() {
        this.inputController = ServiceLocator.Instance.InputController;
    }

    public void Update() {
        this.UpdateKey(InputType.Left, KeyCode.LeftArrow);
        this.UpdateKey(InputType.Right, KeyCode.RightArrow);

        if (Settings.Instance.AlternateControls) {
            this.UpdateKey(InputType.Down, KeyCode.DownArrow);
            this.UpdateKey(InputType.Up, KeyCode.UpArrow);
        }
    }

    private void UpdateKey(InputType type, KeyCode code) {
        if (Input.GetKeyDown(code)) {
            this.inputController.SetInputStatus(InputSource.Keyboard, type, true);
        } else if (Input.GetKeyUp(code)) {
            this.inputController.SetInputStatus(InputSource.Keyboard, type, false);
        }
    }
}
