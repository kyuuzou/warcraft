using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Menu : SceneObject {

    protected GameController GameController { get; private set; }

    private Camera guiCamera;

    public override void InitializeExternals() {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals();

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.GameController = serviceLocator.GameController;
        this.guiCamera = serviceLocator.GUICamera;
    }

    private void PressQuit() {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 position = this.guiCamera.ScreenToWorldPoint(Input.mousePosition);

            Collider2D collider = Physics2D.OverlapPoint((Vector2)position);

            if (collider != null) {
                if (collider.tag == "Button") {
                    MenuButton button = collider.GetComponent<MenuButton>();
                    this.Invoke("Press" + button.Type, 0.0f);
                }
            }
        }
    }
}
