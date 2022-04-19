using UnityEngine;

public class ScrollPrice : SceneObject {

    [SerializeField]
    private SpriteRenderer icon;

    [SerializeField]
    private TextMesh textMesh;

    public void SetColor(Color color) {
        this.textMesh.color = color;
    }

    public override void SetOpacity(float opacity) {
        this.icon.SetOpacity(opacity);
        this.textMesh.SetOpacity(opacity);
    }

    public void SetText(string text) {
        this.textMesh.text = text;
    }
}
