using System.Collections;
using UnityEngine;

public class AnimatedSpriteSelection : SceneObject {

    [SerializeField]
    private SpriteSelection selectionPrefab;

    [SerializeField]
    private Material colorMaterial;

    private SpriteSelection innerSelection;
    private SpriteSelection outerSelection;

    public void Activate(Spell owner) {
        this.StartCoroutine(this.Animate(owner));
    }

    private IEnumerator Animate(Spell owner) {
        this.innerSelection.SetVisible(true);
        this.outerSelection.SetVisible(true);

        Color dark = new Color(54.0f / 255.0f, 58.5f / 255.0f, 0.0f);
        Color light = new Color(89.1f / 255.0f, 93.5f / 255.0f, 4.4f / 255.0f);

        this.StartCoroutine(this.AnimateSelection(this.innerSelection, dark, light, 25.0f));
        yield return this.StartCoroutine(this.AnimateSelection(this.outerSelection, dark, light, 30.0f));

        owner.Deactivate();
    }

    private IEnumerator AnimateSelection(SpriteSelection selection, Color dark, Color light, float time) {
        float deltaTime = 0.0f;
        float deltaColor = 0.0f;

        int multiplier = 1;

        do {
            deltaTime += Time.deltaTime;
            deltaColor += Time.deltaTime * multiplier;

            if (deltaColor < 0.0f) {
                deltaColor = 0.0f;
                multiplier = 1;
            } else if (deltaColor > 1.0f) {
                deltaColor = 1.0f;
                multiplier = -1;
            }

            this.innerSelection.SetColor(Color.Lerp(dark, light, deltaColor));
            this.outerSelection.SetColor(Color.Lerp(dark, light, 1.0f - deltaColor));

            yield return null;
        } while (deltaTime < time);

        GameObject.Destroy(selection.gameObject);
    }

    public void Initialize(Unit unit) {
        if (this.InitializedExternals) {
            return;
        }

        this.InitializeExternals();

        this.innerSelection = SpriteSelection.Instantiate<SpriteSelection>(this.selectionPrefab);
        this.innerSelection.InitializeSelection(unit.Transform, unit.Data.SelectionSize, this.colorMaterial);
        this.innerSelection.transform.SetZ((int)DepthLayer.Projectiles);

        this.outerSelection = SpriteSelection.Instantiate<SpriteSelection>(this.selectionPrefab);
        this.outerSelection.InitializeSelection(
            unit.Transform, unit.Data.SelectionSize, this.colorMaterial
        );
        this.outerSelection.transform.SetZ((int)DepthLayer.Projectiles);
    }
}
