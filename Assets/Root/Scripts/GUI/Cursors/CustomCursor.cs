using UnityEngine;

public class CustomCursor : CustomScriptableObject {

    [SerializeField]
    private CursorType type;
    public CursorType Type {
        get { return this.type; }
    }

    [SerializeField]
    private Vector2 hotspot;
    public Vector2 Hotspot {
        get { return this.hotspot; }
    }

    [SerializeField]
    private Texture2D texture;
    public Texture2D Texture {
        get { return this.texture; }
    }
}
