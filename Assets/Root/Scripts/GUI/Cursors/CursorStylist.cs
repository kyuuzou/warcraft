using System.Collections.Generic;
using UnityEngine;

public class CursorStylist : SceneObject {

    [SerializeField]
    private CustomCursor[] cursors;

    [SerializeField]
    private bool visibleCursor = true;

    private Dictionary<CursorType, CustomCursor> cursorByType;

    public override void InitializeExternals() {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals();

#if ! UNITY_EDITOR
        Cursor.visible = this.visibleCursor;
#endif

        ServiceLocator serviceLocator = ServiceLocator.Instance;

        this.InitializeCursors();
        this.SetCursor(CursorType.Default);
    }

    private void InitializeCursors() {
        this.cursorByType = new Dictionary<CursorType, CustomCursor>();

        foreach (CustomCursor cursor in this.cursors) {
            this.cursorByType[cursor.Type] = cursor;
        }
    }

    public void SetCursor(CursorType type) {
        this.InitializeExternals();

        if (!this.visibleCursor) {
            return;
        }

        CustomCursor cursor = this.cursorByType[type];
        Cursor.SetCursor(cursor.Texture, cursor.Hotspot, CursorMode.ForceSoftware);
    }

    private void Update() {

    }
}
