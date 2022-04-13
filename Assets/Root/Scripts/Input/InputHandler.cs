using UnityEngine;

public class InputHandler : MonoBehaviour {

    public static InputHandler Instance { get; private set; }

    private static bool mouseDown = false;
    public static bool MouseDown {
        get { return InputHandler.mouseDown; }
        set { InputHandler.mouseDown = value; }
    }

    public static bool Enabled { get; set; }

    private InteractionMode[] modes;
    private InteractionMode currentMode;

    private void Awake() {
        if (InputHandler.Instance != null && InputHandler.Instance != this) {
            MonoBehaviour.Destroy(this.gameObject);
        }

        InputHandler.Instance = this;

        this.modes = new InteractionMode[4];
        this.modes[(int)InteractionModeType.Attacking] = new InteractionModeAttacking();
        this.modes[(int)InteractionModeType.Building] = new InteractionModeBuilding();
        this.modes[(int)InteractionModeType.Harvest] = new InteractionModeHarvest();
        this.modes[(int)InteractionModeType.Regular] = new InteractionModeDefault();

        this.currentMode = this.modes[(int)InteractionModeType.Regular];

        InputHandler.Enabled = true;
    }

    public void SetMode(InteractionModeType type, InteractionModeArgs args = null) {
        this.currentMode.DisableMode();
        this.currentMode = this.modes[(int)type];
        this.currentMode.EnableMode(args);
    }

    private void Start() {
        foreach (InteractionMode mode in this.modes) {
            mode.Start();
        }
    }

    private void Update() {
        this.currentMode.Update();
    }
}
