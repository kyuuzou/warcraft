using UnityEngine;

public abstract class InteractionMode {

    protected InputHandler inputHandler;
    protected Camera camera;
    protected ContextMenu contextMenu;

    public InteractionMode() {
        this.inputHandler = ServiceLocator.Instance.InputHandler;
        this.camera = ServiceLocator.Instance.GUICamera;
        this.contextMenu = ServiceLocator.Instance.ContextMenu;
    }

    public abstract void DisableMode();

    public abstract void EnableMode(InteractionModeArgs args = null);

    protected virtual RaycastHit2D HandleClick() {
        Vector2 mousePosition = this.camera.ScreenToWorldPoint(Input.mousePosition);
        return Physics2D.Raycast(mousePosition, Vector2.zero, 1000.0f);
    }

    public virtual void Start() {

    }

    public virtual void Update() {
        /*
        if (InputHandler.Enabled) {
            if (Input.GetMouseButtonDown(0) && InputHandler.MouseDown == false) {
                InputHandler.MouseDown = true;

                this.HandleClick();
            } else if (Input.GetMouseButtonUp(0) && InputHandler.MouseDown) {
                InputHandler.MouseDown = false;
            }
        }
        */
    }
}
