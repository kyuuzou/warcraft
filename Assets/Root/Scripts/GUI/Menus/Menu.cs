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

    public void OnButtonPress(MenuButtonType buttonType) {
        this.Invoke("Press" + buttonType, 0.0f);
    }
    
    private void PressQuit() {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
