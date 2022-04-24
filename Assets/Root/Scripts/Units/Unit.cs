#define HERE_TO_CATCH_A_BUG
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class Unit : SpawnableSprite {

    protected override SpawnableSpriteData BasicData {
        get { return this.Data; }
    }

    public new UnitData Data { get; private set; }

    public override MapTile TargetTile { get; set; }

    public SpawnableSpriteGroup Group { get; set; }
    public bool Invisible { get; set; }
    public bool OffensiveMoving { get; set; }

    private StatusBar statusBar;

    public override SpawnableSpriteType SpriteType {
        get { return (SpawnableSpriteType)this.Type; }
    }

    public UnitType Type {
        get { return this.Data.Type; }
    }

    public void ApproachBuilding() {
        this.Collider.enabled = false;
        this.selection.SetVisible(false);
        this.PhasedOut = true;

        if (this.Selected) {
            this.GameController.CurrentGroup.Remove(this);
            //this.SetSelected (false);
            this.GameController.ClearSelection();
        }
    }

    public void Attack(SpawnableSprite target) {
        this.GetTrait<IUnitTraitAttacker>().Attack(target);
    }

    public void Attack(MapTile target) {
        this.GetTrait<IUnitTraitAttacker>().Attack(target);
    }

    protected override void Awake() {
        base.Awake();

        this.OffensiveMoving = false;
    }

    public void Cast(SpellType spellType, Building building, MapTile tile) {
        this.GetTrait<IUnitTraitSpellcaster>().Cast(spellType, building, tile);
    }

    public void Cast(SpellType spellType, MapTile tile) {
        this.GetTrait<IUnitTraitSpellcaster>().Cast(spellType, tile);
    }

    public void Cast(SpellType spellType, Unit unit, MapTile tile) {
        this.GetTrait<IUnitTraitSpellcaster>().Cast(spellType, unit, tile);
    }

    public void ChangePath(List<MapTile> waypoints) {
        this.GetTrait<IUnitTraitMoving>().ChangePath(waypoints);
    }

    public void Decay() {
        this.GetTrait<IUnitTraitDecaying>().Activate();
    }

    public void Detect(Building building) {
        //this.CurrentMode.Detect (building);
    }

    public void Detect(Unit unit) {
        //this.CurrentMode.Detect (unit);
    }

    public override void Die() {
        if (!this.Dead) {
            this.Collider.enabled = false;

            Vector3 relativePosition = this.GetTrait<IUnitTraitMoving>().RelativePosition;
            MapTile spawnTile = this.Tile;

            if (this.TargetTile != null) {
                float distanceToTargetTile = Vector3.Distance(this.Transform.position, this.TargetTile.RealPosition);
                float distanceToTile = Vector3.Distance(this.Transform.position, this.Tile.RealPosition);

                spawnTile = distanceToTargetTile > distanceToTile ? this.Tile : this.TargetTile;
            }

            base.Die();

            if (this.Selectable && this.Selected) {
                this.SetSelected(false);
                this.GameController.CurrentGroup.Remove(this);
            }

            this.SetTrait<IUnitTraitAttacker>(this.GameObject.AddComponentIfNecessary<UnitTraitNonAttacker>());
            this.SetTrait<IUnitTraitBuilder>(this.GameObject.AddComponentIfNecessary<UnitTraitNonBuilder>());
            this.SetTrait<IUnitTraitDecaying>(this.GameObject.AddComponentIfNecessary<UnitTraitNonDecaying>());
            this.SetTrait<IUnitTraitInteractive>(this.GameObject.AddComponentIfNecessary<UnitTraitNonInteractive>());
            this.SetTrait<IUnitTraitMender>(this.GameObject.AddComponentIfNecessary<UnitTraitNonMender>());
            this.SetTrait<IUnitTraitMiner>(this.GameObject.AddComponentIfNecessary<UnitTraitNonMiner>());
            this.SetTrait<IUnitTraitMoving>(this.GameObject.AddComponentIfNecessary<UnitTraitNonMoving>());
            this.SetTrait<IUnitTraitShooter>(this.GameObject.AddComponentIfNecessary<UnitTraitNonShooter>());
            this.SetTrait<IUnitTraitSpellcaster>(this.GameObject.AddComponentIfNecessary<UnitTraitNonSpellcaster>());

            UnitTraitNonMoving trait = (UnitTraitNonMoving)this.GetTrait<IUnitTraitMoving>();
            trait.Initialize(this, this.SpawnFactory.GetData<UnitTraitDataNonMoving>());
            trait.RelativePosition = relativePosition;

            this.Play(AnimationType.Dying);
            this.ReleaseClaimedTiles();

            this.MissionStatistics.Score -= 20;
            this.MissionStatistics.UnitsEnemyDestroyed++;

            this.Faction.RemoveUnit(this);
            this.Map.RemoveUnit(this);
        }
    }

    private void EnterAttackingMode() {
        InteractionHandler.Instance.SetMode(InteractionModeType.Attacking, new InteractionModeAttackingArgs(this));
        this.ContextMenu.SetNode(this.ContextMenu.CancelNode);
    }

    public void EnterBuilding() {
        this.Renderer.enabled = false;
        this.Play(AnimationType.Idle);
    }

    private void EnterMendingMode() {
        this.ContextMenu.SetNode(this.ContextMenu.CancelNode);
    }

    private void EnterSpellcastingMode(SpellType spellType, bool targetMustBeAlive = true) {
        IUnitTraitSpellcaster trait = this.GetTrait<IUnitTraitSpellcaster>();

        if (!trait.MayCast(spellType)) {
            this.statusBar.SetText("Not enough mana to cast spell", 3.0f);
            //Debug.Log (this.Type + " may not cast " + spellType);
            return;
        }

        if (trait.RequiresTarget(spellType)) {
            this.ContextMenu.SetNode(this.ContextMenu.CancelNode);
        } else {
            trait.Cast(spellType, this.Tile);
        }
    }

    private IEnumerator Fall(UnitTraitNonMoving trait) {
        this.GameController.CurrentGroup.Clear();

        this.Play(AnimationType.Walking);
        this.GetComponent<Light>().enabled = true;

        MapTile targetTile = this.Tile.GetNeighbour(this.Direction);
        Vector3 basePosition = this.Tile.RealPosition;
        float hitPoints = this.CurrentHitPoints;
        float delta = 1.0f;
        float totalDistance = Vector3.Distance(this.Tile.RealPosition, targetTile.RealPosition);

        do {
            yield return null;

            Vector3 origin = basePosition + trait.RelativePosition;
            Vector3 destination = targetTile.RealPosition;

            float distance = Vector3.Distance(origin, destination);

            trait.RelativePosition = Vector3.Lerp(origin, destination, (50.0f * Time.deltaTime) / distance);
            this.transform.position = trait.RelativePosition;
            trait.RelativePosition -= basePosition;

            delta = Mathf.InverseLerp(
                1.0f,
                totalDistance,
                Vector3.Distance(this.Transform.position, targetTile.RealPosition)
            );

            this.SetHitPoints(Mathf.FloorToInt(delta * hitPoints));
        } while (delta > 0.0f);

        this.GetComponent<Light>().enabled = false;

        this.Play(AnimationType.Dying);
    }

    public void ForcedMove(
        IMovementDestination destination,
        bool overlapTarget = true,
        bool recalculation = false
    ) {
        this.ForcedMove(destination, this.GetTrait<IUnitTraitMoving>(), overlapTarget, recalculation);
    }

    public void ForcedMove(
        IMovementDestination destination,
        IMovementListener movementListener,
        bool overlapTarget = true,
        bool recalculation = false
    ) {
        this.GetTrait<IUnitTraitAttacker>().Deactivate();
        this.GetTrait<IUnitTraitMender>().Deactivate();
        this.GetTrait<IUnitTraitMiner>().Deactivate();

        this.Move(destination, movementListener, overlapTarget, recalculation);
    }

    //Returns either Tile or TargetTile, whichever is closest
    public override MapTile GetClosestTile() {
        if (this.TargetTile == null) {
            return this.Tile;
        }

        float distanceFromTarget = Vector3.Distance(this.Transform.position, this.TargetTile.RealPosition);
        float totalDistance = Vector3.Distance(this.Tile.RealPosition, this.TargetTile.RealPosition);
        float progress = Mathf.InverseLerp(totalDistance, 0.0f, distanceFromTarget);

        return (progress > 0.5f) ? this.TargetTile : this.Tile;
    }

    public override MapTile GetRealTile() {
        return this.TargetTile ?? this.Tile;
    }

    public MapTile GetTargetTile() {
        return this.TargetTile;
    }

    public override void Initialize(MapTile tile) {
        base.Initialize(tile);

        this.statusBar = ServiceLocator.Instance.StatusBar;

        this.ClaimTile(tile);
        this.Transform.position = tile.RealPosition;

        this.Collider.enabled = true;

        if (this.Faction.ControllingPlayer.Data.HumanPlayer) {
            //this.Tile.Discover ();
        }

        this.MeshAnimator.RegisterTriggerListener(this);
    }

    protected override void InitializeSelection() {
        this.selection.InitializeSelection(this.transform, this.Data.SelectionSize);
        //this.GetComponent<BoxCollider2D> ().size = this.Data.SelectionSize * 32.0f;
    }

    public void Interact(Unit unit) {
        this.GetTrait<IUnitTraitInteractive>().Interact(unit.GetTrait<IUnitTraitInteractive>());
    }

    public void Interact(MapTile tile) {
        this.GetTrait<IUnitTraitInteractive>().Interact(tile);
    }

    private void LateUpdate() {
        this.GetTrait<IUnitTraitMoving>().LateManualUpdate();
    }

    public void LeaveBuilding() {
        this.Renderer.enabled = true;
        this.Collider.enabled = true;
    }

    public override void ManualUpdate() {
        if (this.GameController.Paused) {
            return;
        }

        base.ManualUpdate();

#if HERE_TO_CATCH_A_BUG
        if (this.GetTrait<IUnitTraitMoving>() == null) {
            Debug.Log(this);
            return;
        }
#endif

        this.GetTrait<IUnitTraitMoving>().ManualUpdate();
        this.GetTrait<IUnitTraitAttacker>().ManualUpdate();
    }

    public bool MayCast(SpellType spellType) {
        return this.GetTrait<IUnitTraitSpellcaster>().MayCast(spellType);
    }

    public void Mend(Building building) {
        this.GetTrait<IUnitTraitMender>().Mend(building);
    }

    public void Mend(Decoration decoration) {
        this.GetTrait<IUnitTraitMender>().Mend(decoration);
    }

    public void Mine(Building mine) {
        this.GetTrait<IUnitTraitMiner>().Mine(mine);
    }

    public void Move(
        IMovementDestination destination,
        bool overlapTarget = true,
        bool recalculation = false
    ) {
        this.Move(destination, this.GetTrait<IUnitTraitMoving>(), overlapTarget, recalculation);
    }

    public void Move(
        IMovementDestination destination,
        IMovementListener movementListener,
        bool overlapTarget = true,
        bool recalculation = false
    ) {
        this.GetTrait<IUnitTraitMoving>().Move(destination, movementListener, overlapTarget, recalculation);
    }

    public override void OnAnimationTrigger(AnimationType animationType, AnimationTriggerType triggerType) {
        base.OnAnimationTrigger(animationType, triggerType);

        switch (triggerType) {
            case AnimationTriggerType.OnAttack:
                this.OnAttack();
                break;

            case AnimationTriggerType.OnDecomposed:

                break;

            case AnimationTriggerType.OnFinished:
                switch (animationType) {
                    case AnimationType.Attacking:
                        this.GetTrait<IUnitTraitAttacker>().AttackAfterCooldown();
                        break;

                    case AnimationType.Mending:
                        this.GetTrait<IUnitTraitMender>().MendAfterCooldown();
                        break;
                    
                    default:
                        // do nothing
                        break;
                }

                break;

            case AnimationTriggerType.OnMending:
                this.OnMend();
                break;

            default:
                throw new NotSupportedException($"Received unexpected value: {triggerType}");
        }
    }

    protected void OnAttack() {
        this.GetTrait<IUnitTraitAttacker>().OnAttack();
    }

    private void OnDeathAnimationFinished(object sender, EventArgs args) {
    }

    public override bool OnManualMouseDown() {
        base.OnManualMouseDown();

        this.GameController.CurrentGroup.Set(true, this);

        return true;
    }

    private void OnMend() {
        this.GetTrait<IUnitTraitMender>().OnMend();
    }

    public void OnOrderAccepted() {
        this.AudioManager.Play(this.Data.AcknowledgeSound);
    }

    public void OnPathFindingFailed() {

    }
    
    private void PressArrow() {
        this.EnterAttackingMode();
    }

    private void PressAxe() {
        this.EnterAttackingMode();
    }

    private void PressBuildAdvancedStructure() {
        this.GetTrait<IUnitTraitBuilder>().ShowAdvancedStructures();
    }

    private void PressBuildBasicStructure() {
        this.GetTrait<IUnitTraitBuilder>().ShowBasicStructures();
    }

    public override void PressCancel() {
        this.GetTrait<IUnitTraitBuilder>().Deactivate();

        base.PressCancel();
    }

    private void PressCloudOfPoison() {
        this.EnterSpellcastingMode(SpellType.CloudOfPoison);
    }

    private void PressDaemon() {
        this.EnterSpellcastingMode(SpellType.MajorSummon);
    }

    private void PressDarkVision() {
        this.EnterSpellcastingMode(SpellType.DarkVision);
    }

    private void PressElementalBlast() {
        this.EnterAttackingMode();
    }

    private void PressFarSeeing() {
        this.EnterSpellcastingMode(SpellType.FarSeeing);
    }

    private void PressFireball() {
        this.EnterAttackingMode();
    }

    private void PressHarvestLumberMineGold() {
        this.ContextMenu.SetNode(this.ContextMenu.CancelNode);

        ServiceLocator.Instance.InteractionHandler.SetMode(
            InteractionModeType.Harvest,
            new InteractionModeHarvestArgs(this)
        );
    }

    private void PressHealing() {
        this.EnterSpellcastingMode(SpellType.Healing);
    }

    private void PressHolyLance() {
        this.EnterAttackingMode();
    }

    private void PressHumanBarracks() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.HumanBarracks);
    }

    private void PressHumanBlacksmith() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.HumanBlacksmith);
    }

    private void PressHumanChurch() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.HumanChurch);
    }

    private void PressHumanFarm() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.HumanFarm);
    }

    private void PressHumanLumberMill() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.HumanLumberMill);
    }

    private void PressHumanMove() {
        this.ContextMenu.SetNode(this.ContextMenu.CancelNode);
    }

    private void PressHumanShield() {
        this.Stop();
    }

    private void PressHumanStables() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.HumanStables);
    }

    private void PressHumanTower() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.HumanTower);
    }

    private void PressHumanTownHall() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.HumanTownHall);
    }

    private void PressInvisibility() {
        this.EnterSpellcastingMode(SpellType.Invisibility);
    }

    private void PressOrcBarracks() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.OrcBarracks);
    }

    private void PressOrcBlacksmith() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.OrcBlacksmith);
    }

    private void PressOrcFarm() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.OrcFarm);
    }

    private void PressOrcKennels() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.OrcKennels);
    }

    private void PressOrcLumberMill() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.OrcLumberMill);
    }

    private void PressOrcMove() {
        this.ContextMenu.SetNode(this.ContextMenu.CancelNode);
    }

    private void PressOrcShield() {
        this.Stop();
    }

    private void PressOrcTemple() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.OrcTemple);
    }

    private void PressOrcTownHall() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.OrcTownHall);
    }

    private void PressOrcTower() {
        this.GetTrait<IUnitTraitBuilder>().Build(BuildingType.OrcTower);
    }

    private void PressRainOfFire() {
        this.EnterSpellcastingMode(SpellType.RainOfFire);
    }

    private void PressRaiseDead() {
        this.EnterSpellcastingMode(SpellType.RaiseDead, false);
    }

    private void PressRepair() {
        this.EnterMendingMode();
    }

    private void PressScorpion() {
        this.EnterSpellcastingMode(SpellType.MinorSummon);
    }

    private void PressShadowSpear() {
        this.EnterAttackingMode();
    }

    private void PressSpider() {
        this.EnterSpellcastingMode(SpellType.MinorSummon);
    }

    private void PressSword() {
        this.EnterAttackingMode();
    }

    private void PressUnholyArmor() {
        this.EnterSpellcastingMode(SpellType.UnholyArmor);
    }

    private void PressWaterElemental() {
        this.EnterSpellcastingMode(SpellType.MajorSummon);
    }    

    public override void RefreshPosition() {
        this.GetTrait<IUnitTraitMoving>().RefreshPosition();
    }

    protected void RegisterAnimationFinished(AnimationType type, EventHandler handler) {
    }

    protected void SetData(UnitData data) {
        this.Data = data;
        base.SetData(data);
        this.SetTraits(data);

        this.RegisterAnimationFinished(AnimationType.Dying, this.OnDeathAnimationFinished);
        this.SetRandomDirection();
    }

    public void SetDestination(MapTile tile) {
        this.GetTrait<IUnitTraitMoving>().SetDestination(tile);
    }

    public void SetRandomDirection() {
        if (this.GetTrait<IUnitTraitMoving>().MayMoveDiagonally) {
            this.Direction = (Direction)Random.Range((int)Direction.North, (int)Direction.Northwest);
        } else {
            this.Direction = (Direction)(Random.Range(0, 4) * 2 + 1);
        }

        this.Play(AnimationType.Idle);
    }

    public override void SetSelected(bool selected) {
        base.SetSelected(selected);

        //this.GetTrait<IUnitTraitMoving> ().SetSelected (selected);
    }

    private void SetTraits(UnitData data) {
        this.InitializeTraits();

        if (data.AttackerTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonAttacker>().AddTrait(this);
        } else {
            data.AttackerTrait.AddTrait(this);
        }

        if (data.BuilderTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonBuilder>().AddTrait(this);
        } else {
            data.BuilderTrait.AddTrait(this);
        }

        if (data.DecayingTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonDecaying>().AddTrait(this);
        } else {
            data.DecayingTrait.AddTrait(this);
        }

        if (data.InteractiveTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonInteractive>().AddTrait(this);
        } else {
            data.InteractiveTrait.AddTrait(this);
        }

        if (data.MenderTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonMender>().AddTrait(this);
        } else {
            data.MenderTrait.AddTrait(this);
        }

        if (data.MinerTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonMiner>().AddTrait(this);
        } else {
            data.MinerTrait.AddTrait(this);
        }

        if (data.MovingTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonMoving>().AddTrait(this);
        } else {
            data.MovingTrait.AddTrait(this);
        }

        if (data.ShooterTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonShooter>().AddTrait(this);
        } else {
            data.ShooterTrait.AddTrait(this);
        }

        if (data.SpellcasterTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonSpellcaster>().AddTrait(this);
        } else {
            data.SpellcasterTrait.AddTrait(this);
        }
    }

    public void SetUnitType(UnitType type) {
        this.SetData(this.SpawnFactory.GetData(type));
    }

    public void Shoot(IShootingListener listener, Projectile projectilePrefab, ITarget target) {
        this.GetTrait<IUnitTraitShooter>().Shoot(listener, projectilePrefab, target);
    }

    public void Stop() {
        this.GetTrait<IUnitTraitMoving>().Deactivate();
    }
}
