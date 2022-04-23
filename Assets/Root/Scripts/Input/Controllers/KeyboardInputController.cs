using System;
using UnityEngine;

[Serializable]
public class KeyboardInputController {

    private InputController inputController;

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
