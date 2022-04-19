using UnityEngine;

public class Decoration : SpawnableSprite, IInhabitant {

    protected override SpawnableSpriteData BasicData {
        get { return this.Data; }
    }

    public override bool Selectable {
        get { return false; }
    }

    public override SpawnableSpriteType SpriteType {
        get { return SpawnableSpriteType.None; }
    }

    public new DecorationData Data { get; private set; }

    private TileData tileData;
    public TileData TileData {
        private get {
            return this.tileData;
        }
        set {
            this.tileData = value;
            this.SetTileData(value);
        }
    }

    public bool IsBehindEverything { get; set; }

    private Vector3 lastPosition = Vector3.zero;
    private Vector3 offset = Vector3.zero;

    public override void Damage(int damage) {
        if (this.Data.HitPoints == 0) {
            return;
        }

        TileData data;

        if (this.CurrentHitPoints <= 0) {
            data = this.tileData.NextStage.NextStage;
        } else if (this.CurrentHitPoints < this.Data.HitPoints / 2) {
            data = this.tileData.NextStage;
        } else {
            data = this.tileData;
        }

        if (data.Dependencies.Length > 0) {
            int hitPoints = this.CurrentHitPoints - damage;

            foreach (MapTile neighbour in this.Tile.GetNeighboursOfData(data.Dependencies)) {
                Decoration decoration = neighbour.GetInhabitant<Decoration>();

                if (decoration != null) {
                    if (hitPoints > 0) {
                        decoration.SetHitPoints(hitPoints);
                    } else {
                        decoration.Die();
                    }
                }
            }
        }

        base.Damage(damage);
    }

    public override void Die() {
        if (!this.Dead) {
            base.Die();

            this.GetComponent<Renderer>().enabled = false;

            this.Tile.SetData(this.Tile.Data.NextStage.NextStage);
            this.Grid.Refresh();

            this.OnAnimationTrigger(AnimationType.None, AnimationTriggerType.OnDecomposed);
        }
    }

    public override void Initialize(MapTile tile) {
        base.Initialize(tile);

        if (this.MeshAnimator != null) {
            this.MeshAnimator.RegisterTriggerListener(this);
        }

        tile.AddInhabitant(this);

        this.tileData = tile.Data;
    }

    protected virtual void LateUpdate() {
        /*
        if (this.Tile != null) {
            Vector3 position = this.Tile.RealPosition;
            position.y = position.y - this.Grid.DefaultSlotSize.y * 0.5f + this.SpriteRenderer.bounds.size.y * 0.5f;
            position.y -= 47.0f;

            if (this.Tile.Visible) {
                if (this.IsBehindEverything) {
                    position.z = this.Tile.Slot.GetLayer (0).Transform.position.z;
                } else {
                    position.z = this.Tile.Slot.SceneObject.Transform.position.z;
                }
            }

            position = position + this.offset;
            */
        /*
        float rowOffset = this.Tile.Visible ? this.Tile.Slot.Row * 2.0f : 0.0f;
        this.transform.SetZ (-1.0f - rowOffset);
        */
        /*
        if (this.lastPosition != position) {
            this.lastPosition = position;

            this.transform.position = position;
        }
    }*/
    }

    protected virtual void SetData(DecorationData data) {
        this.Data = data;

        base.SetData(data);
    }

    public void SetDecorationType(DecorationType type) {
        this.SetData(this.SpawnFactory.GetData(type));

        MapType mapType = this.GameController.CurrentLevel.MapType;

        if (this.MeshAnimator != null) {
            this.MeshAnimator.GetAnimation(AnimationType.Idle).OverrideTexture = this.Data.GetTexture(mapType);
            this.MeshAnimator.Play(AnimationType.Idle, false);
        }
    }

    protected override void SetHitPoints(int hitPoints) {
        base.SetHitPoints(hitPoints);

        if (hitPoints < this.Data.HitPoints / 2) {
            this.SetTileData(this.tileData.NextStage);
        } else {
            this.SetTileData(this.tileData);
        }

    }

    public void SetOffset(Vector3 offset) {
        this.offset = offset;
    }

    public void SetTileData(TileData data) {
        MeshAnimation animation = this.MeshAnimator.GetAnimation(AnimationType.Idle);
        animation.SetFrameValue(0, data.AtlasIndex - 1);

        this.Play(AnimationType.Idle);
    }
}
