using UnityEngine;
using System.Collections;

public class SingleStatusPane : SceneObject {

    [SerializeField]
    private TiledElement portrait;

    [SerializeField]
    private MapTypeTexture portraitTexture;
    
    [SerializeField]
    private Color[] hitPointColors;

    [SerializeField]
    private Transform hitPointsRoot;

    [SerializeField]
    private SpriteRenderer hitPoints;

    private float defaultHitPointsScale;

    [SerializeField]
    private Transform manaPointsRoot;
    
    [SerializeField]
    private SpriteRenderer manaPoints;

    private SpawnableSprite owner;
    public SpawnableSprite Owner {
        get { return this.owner; }
        set {
            this.owner = value;
            this.portrait.SetCurrentTileY ((int) owner.Portrait);

            this.SetPoints (this.hitPointsRoot, this.hitPoints, owner.HitPointPercentage);

            if (this.manaPointsRoot != null) {
                this.SetPoints (this.manaPointsRoot, this.manaPoints, owner.ManaPointPercentage);
            }
        }
    }
    
    protected override void Awake () {
        base.Awake ();

        this.defaultHitPointsScale = this.hitPointsRoot.localScale.x;
    }

    private void SetPoints (Transform root, SpriteRenderer sprite, float percentage) {
        percentage = Mathf.Max (percentage, 0.0f);
        
        root.SetLocalScaleX (percentage * this.defaultHitPointsScale);
        sprite.color = this.hitPointColors[Mathf.Min (2, (int) (3 * percentage))];
    }

    protected override void Start () {
        MapType mapType = ServiceLocator.Instance.GameController.CurrentLevel.MapType;
        this.portrait.SetTexture (this.portraitTexture.GetTexture (mapType));
    }
}
