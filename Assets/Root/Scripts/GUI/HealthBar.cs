using System.Collections;
using UnityEngine;

public class HealthBar : SceneObject {

    [Header ("Sprites")]
    [SerializeField]
    private SpriteRenderer emptySprite;

    [SerializeField]
    private Transform fullPivot;

    [SerializeField]
    private MeshRenderer fullSprite;

    [Header ("Textures")]
    [SerializeField]
    private Texture friendlyTexture;

    [SerializeField]
    private Texture enemyTexture;

    public void Initialize (Unit owner) {
        if (owner.Faction.ControllingPlayer.Data.HumanPlayer) {
            this.fullSprite.material.mainTexture = this.friendlyTexture;
        } else {
            this.fullSprite.material.mainTexture = this.enemyTexture;
        }
    }

    public void SetHitpoints (float hitPointPercentage) {
        this.fullPivot.SetLocalScaleX (hitPointPercentage);

        Vector2 scale = this.fullSprite.material.mainTextureScale;
        scale.x = hitPointPercentage;
        this.fullSprite.material.mainTextureScale = scale;

        this.GameObject.SetActive (hitPointPercentage != 1.0f);
    }

    public void SetSize (float scale) {
        this.Transform.SetLocalScale (scale, scale, 1.0f);
    }
}
