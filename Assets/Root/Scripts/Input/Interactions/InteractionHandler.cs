using UnityEngine;

public class InteractionHandler : MonoBehaviour {

    public static InteractionHandler Instance { get; private set; }

    private static bool mouseDown = false;
    public static bool MouseDown {
        get { return InteractionHandler.mouseDown; }
        set { InteractionHandler.mouseDown = value; }
    }

    public static bool Enabled { get; set; }

    private InteractionMode[] modes;
    private InteractionMode currentMode;

    private void Awake() {
        if (InteractionHandler.Instance != null && InteractionHandler.Instance != this) {
            MonoBehaviour.Destroy(this.gameObject);
        }

        InteractionHandler.Instance = this;

        this.modes = new InteractionMode[4];
        this.modes[(int)InteractionModeType.Attacking] = new InteractionModeAttacking();
        this.modes[(int)InteractionModeType.Building] = new InteractionModeBuilding();
        this.modes[(int)InteractionModeType.Harvest] = new InteractionModeHarvest();
        this.modes[(int)InteractionModeType.Regular] = new InteractionModeDefault();

        this.currentMode = this.modes[(int)InteractionModeType.Regular];

        InteractionHandler.Enabled = true;
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
