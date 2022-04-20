#define FAST_PROGRESS

using System;
using System.Collections;
using UnityEngine;

public partial class Building : SpawnableSprite {

    protected override SpawnableSpriteData BasicData {
        get { return this.Data; }
    }

    public new BuildingData Data { get; private set; }

    [SerializeField]
    private CustomAnimatedSprite collapsingAnimationPrefab;
    protected CustomAnimatedSprite collapsingAnimation;

    private Vector3 lastPosition = Vector3.zero;

    private int progressIndex = -1;

    private Unit builder = null;
    private IEnumerator buildEnumerator = null;
    private bool underConstruction;

    public bool AwaitingBuilder { get; private set; }

    public override SpawnableSpriteType SpriteType {
        get { return (SpawnableSpriteType)this.Type; }
    }

    public BuildingType Type {
        get { return this.Data.Type; }
    }

    protected override void AdaptTexturesToMapType() {
        MapType mapType = this.GameController.CurrentLevel.MapType;

        foreach (MeshAnimation animation in this.Data.Animations) {
            switch (animation.Type) {
                case AnimationType.Idle:
                    animation.OverrideTexture = this.Data.GetTexture(mapType, false);
                    break;

                case AnimationType.ConstructionStage0:
                case AnimationType.ConstructionStage1:
                case AnimationType.ConstructionStage2:
                    animation.OverrideTexture = this.Data.GetTexture(mapType, true);
                    break;

                default:
                    throw new NotSupportedException($"Received unexpected value: {animation}");
            }
        }
    }

    public void Build(Unit builder) {
        this.builder = builder;

        this.buildEnumerator = this.BuildCoroutine(builder);
        this.StartCoroutine(this.buildEnumerator);
    }

    private IEnumerator BuildCoroutine(Unit builder) {
        this.AwaitingBuilder = false;

        yield return this.StartCoroutine(this.Progress(this.Data.TimeToBuild, true));

        builder.GetTrait<IUnitTraitBuilder>().OnWorkComplete();
        this.Play(AnimationType.Idle);

        this.statusBackgroundIndex = 8;

        if (this.Selected) {
            this.StatusPane.SetBackgroundIndex(this.statusBackgroundIndex);
        }

        yield return this.StartCoroutine(this.EjectUnitCoroutine(builder));

        this.underConstruction = false;

        if (this.Selected) {
            this.SetSelected(true);
        }

        this.GameController.OnBuildingComplete(this);

        this.MissionStatistics.Score += 20;
        this.MissionStatistics.StructuresYouBuilt++;
    }

    private void CancelConstruction() {
        this.underConstruction = false;

        this.GameController.IncreaseGold(this.Data.GoldCost / 4);
        this.GameController.IncreaseLumber(this.Data.LumberCost / 4);

        if (this.buildEnumerator != null) {
            this.StopCoroutine(this.buildEnumerator);
            this.buildEnumerator = null;

            this.StartCoroutine(this.EjectUnitCoroutine(this.builder));
        }

        this.Die();
    }

    private MapTile CheckFreeTile(Unit unit, int column, int row, ref bool foundFreeTile, ref int i) {
        MapTile tile = this.Map.GetTile(column, row);

        //tile.Caption = (i++).ToString ();

        if (tile != null && tile.IsTraversable(MovementType.Land, unit)) {
            foundFreeTile = true;
            return tile;
        }

        return null;
    }

    public override void Die() {
        if (!this.Dead) {
            base.Die();

            this.collapsingAnimation.Renderer.enabled = true;
            this.collapsingAnimation.Play(AnimationType.CollapseStage2);

            this.Sprite.Renderer.enabled = false;
            this.ScorchFloor();

            this.MissionStatistics.Score -= 20;
            this.MissionStatistics.StructuresEnemyDestroyed++;

            this.Faction.RemoveBuilding(this);
            this.Map.RemoveBuilding(this);

            this.AudioManager.Play(AudioIdentifier.BuildingCollapse);
        }
    }

    public void Discover() {
        // TODO: extract range to an inspector variable
        Vector2 range = new Vector2(10.0f, 10.0f);
        Vector2Int tilePadding = this.TileSize - new Vector2Int(1, 1);
        Vector2Int position = this.Tile.MapPosition;
        int outlinePadding = 2;

        int column = position.x;
        int row = position.y;

        int columns = (int)range.x;
        int rows = (int)range.y;

        for (int x = -columns + 1; x < columns - 1; x++) {
            for (int y = -rows + 1; y < rows - 1; y++) {
                this.Map.Discover(column + x + tilePadding.x, row + y + tilePadding.y);
            }
        }

        for (int y = -rows + 1 + outlinePadding; y < rows - 1 - outlinePadding; y++) {
            this.Map.Discover(column - columns + tilePadding.x, row + y + tilePadding.y);
            this.Map.Discover(column + columns + tilePadding.x - 1, row + y + tilePadding.y);
        }

        for (int x = -columns + 1 + outlinePadding; x < columns - 1 - outlinePadding; x++) {
            this.Map.Discover(column + x + tilePadding.x, row - rows + tilePadding.y);
            this.Map.Discover(column + x + tilePadding.x, row + rows + tilePadding.y - 1);
        }
    }

    public void EjectUnit(Unit unit, bool becomeIdle = true) {
        this.StartCoroutine(this.EjectUnitCoroutine(unit, becomeIdle));
    }

    public IEnumerator EjectUnitCoroutine(Unit unit, bool becomeIdle = true) {
        Vector2Int position = this.Tile.MapPosition;
        int column = position.x;
        int row = position.y;

        Vector2Int size = this.TileSize;
        int columns = size.x;
        int rows = size.y;

        MapTile tile = null;
        int depth = 1;
        bool foundFreeTile = false;
        int i = 0;

        do {
            for (int x = -depth; x < columns + depth; x++) {
                tile = this.CheckFreeTile(unit, x + column, row - depth, ref foundFreeTile, ref i);

                if (tile == null) {
                    yield return null;
                } else {
                    break;
                }

                tile = this.CheckFreeTile(
                    unit,
                    x + column,
                    row + rows + depth - 1,
                    ref foundFreeTile,
                    ref i
                );

                if (tile == null) {
                    yield return null;
                } else {
                    break;
                }
            }

            if (tile != null) {
                break;
            }

            for (int y = -depth; y < rows + depth; y++) {
                tile = this.CheckFreeTile(unit, column - depth, y + row, ref foundFreeTile, ref i);

                if (tile == null) {
                    yield return null;
                } else {
                    break;
                }

                tile = this.CheckFreeTile(unit, column + columns + depth - 1, y + row, ref foundFreeTile, ref i);

                if (tile == null) {
                    yield return null;
                } else {
                    break;
                }
            }

            depth++;
        } while (tile == null && depth < 10);

        if (!foundFreeTile) {
            Debug.LogWarning("Did not find a free position for the unit: " + unit + ".");
        }

        unit.SetTile(tile);
        unit.ReleaseClaimedTiles();
        unit.ClaimTile(tile);

        unit.RefreshPosition();

        unit.Sprite.Renderer.enabled = true;
        unit.Collider.enabled = true;
        unit.Tile.Discover();

        unit.PhasedOut = false;

        if (becomeIdle) {
            unit.Play(AnimationType.Idle);
        }
    }

    public override void Initialize(MapTile tile) {
        base.Initialize(tile);

        this.Map.AddBuilding(this);

        this.Faction.AddRootedPosition(this);

        this.ClaimTile(tile);

        if (this.Faction.ControllingPlayer.Data.HumanPlayer) {
            this.Discover();
        }

        this.InitializeAnimations();
    }

    private void InitializeAnimations() {
        this.collapsingAnimation = Transform.Instantiate(this.collapsingAnimationPrefab);
        this.collapsingAnimation.transform.parent = this.transform;
        this.collapsingAnimation.transform.localPosition = new Vector3(0.0f, 32.0f, -1.0f);
        this.collapsingAnimation.name = this.collapsingAnimationPrefab.name;
        this.collapsingAnimation.GetComponent<Renderer>().enabled = false;
        this.collapsingAnimation.Register(this.OnCollapseAnimationFinished);

        this.MeshAnimator.RegisterTriggerListener(this);
    }

    public bool IsMinable() {
        return this.GetTrait<IBuildingTraitMinable>().IsMinable();
    }

    public override void ManualUpdate() {
        base.ManualUpdate();

        this.RefreshPosition();
    }

    public int Mine(IUnitTraitMiner miner) {
        return this.GetTrait<IBuildingTraitMinable>().Mine(miner);
    }

    private void OnCollapseAnimationFinished(object sender, EventArgs args) {
        this.Sprite.Renderer.enabled = false;

        this.ReleaseClaimedTiles();

        this.ManualDestroy();
    }

    public override bool OnManualMouseDown() {
        base.OnManualMouseDown();

        this.GameController.CurrentGroup.Set(true, this);

        return true;
    }

    protected override void PlaySelectionSound() {
        if (this.underConstruction) {
            this.AudioManager.PlayUnique(AudioIdentifier.Building);

            return;
        }

        base.PlaySelectionSound();
    }

    public IEnumerator Progress(float timeToBuild, bool underConstruction = false) {
        float start = Time.time;
        float progress;

        this.statusBackgroundIndex = 6;

        if (this.Selected) {
            this.StatusPane.SetBackgroundIndex(this.statusBackgroundIndex);
        }

#if FAST_PROGRESS
        timeToBuild /= 500.0f;
#else
        //1000 build time = 20 seconds
        timeToBuild /= 50.0f;
#endif

        do {
            progress = Mathf.InverseLerp(start, start + timeToBuild, Time.time);

            if (this.Selected) {
                this.StatusPane.SetProgress(progress);
            }

            if (underConstruction) {
                int index = Mathf.Min(2, (int)(3 * progress));

                if (index != this.progressIndex) {
                    this.progressIndex = index;

                    this.Play(
                        (AnimationType)Enum.Parse(typeof(AnimationType), "ConstructionStage" + index)
                    );
                }
            }

            yield return null;
        } while (progress < 1.0f);
    }

    public override void RefreshPosition() {
        if (this.Tile != null) {
            Vector3 position = this.Tile.RealPosition;
            position = position.Add(this.Offset);

            if (this.lastPosition != position) {
                this.lastPosition = position;

                this.transform.position = position;
            }
        }
    }

    private void ScorchFloor() {
        Vector2Int position = this.Tile.MapPosition;
        int column = position.x;
        int row = position.y;

        for (int x = 0; x < this.TileSize.x; x++) {
            for (int y = 0; y < this.TileSize.y; y++) {
                MapTile tile = this.Map.GetTile(column + x, row + y);
                tile.SetType(TileType.ScorchedGround);
            }
        }

        for (int x = 0; x < this.TileSize.x; x++) {
            for (int y = 0; y < this.TileSize.y; y++) {
                MapTile tile = this.Map.GetTile(column + x, row + y);
                tile.RefreshNeighbours(TileType.ScorchedGround);
            }
        }

        this.Grid.Refresh();
    }

    public void SetBuildingType(BuildingType type) {
        BuildingData data = this.SpawnFactory.GetData(type);
        this.SetData(data);
    }

    protected virtual void SetData(BuildingData data) {
        this.Data = data;

        //Before base.SetData, so that the textures are switched by the time the animations are initialized
        this.AdaptTexturesToMapType();

        base.SetData(data);
        this.SetTraits(data);

        this.Play(AnimationType.Idle);
    }

    protected override void SetHitPoints(int hitPoints) {
        base.SetHitPoints(hitPoints);

        if (!this.underConstruction && this.collapsingAnimation != null) {
            int third = this.Data.HitPoints / 3;

            if (hitPoints < third) {
                this.collapsingAnimation.Renderer.enabled = true;
                this.collapsingAnimation.Play(AnimationType.CollapseStage1);
            } else if (hitPoints < third * 2) {
                this.collapsingAnimation.Renderer.enabled = true;
                this.collapsingAnimation.Play(AnimationType.CollapseStage0);
            } else {
                this.collapsingAnimation.Renderer.enabled = false;
            }
        }
    }

    public override void SetSelected(bool selected) {
        base.SetSelected(selected);

        if (this.progressIndex == -1 && selected) {
            this.StatusPane.SetProgress(0);
        }

        if (this.underConstruction && selected) {
            this.ContextMenu.SetNode(this.ContextMenu.CancelNode);
        }
    }

    private void SetTraits(BuildingData data) {
        this.InitializeTraits();

        if (data.MinableTrait == null) {
            this.SpawnFactory.GetData<BuildingTraitDataNonMinable>().AddTrait(this);
        } else {
            data.MinableTrait.AddTrait(this);
        }

        if (data.ResearcherTrait == null) {
            this.SpawnFactory.GetData<BuildingTraitDataNonResearcher>().AddTrait(this);
        } else {
            data.ResearcherTrait.AddTrait(this);
        }

        if (data.TrainerTrait == null) {
            this.SpawnFactory.GetData<BuildingTraitDataNonTrainer>().AddTrait(this);
        } else {
            data.TrainerTrait.AddTrait(this);
        }
    }

    public void SetUpConstructionSite(Unit builder, MapTile tile) {
        this.Initialize(tile);

        builder.GetTrait<IUnitTraitBuilder>().MoveToConstructionSite(this);

        if (builder.Selected) {
            this.ContextMenu.SetNode(builder.Data.RootMenuNode);
        }

        this.Play(AnimationType.ConstructionStage0);

        this.Sprite.Renderer.enabled = true;
        this.Collider.enabled = true;
        this.selection.SetLocked(false);
        this.selection.SetVisible(false);

        this.statusBackgroundIndex = 6;

        this.AwaitingBuilder = true;
        this.underConstruction = true;

        //Force a refresh on Update
        this.lastPosition = Vector2.down;
    }
}
