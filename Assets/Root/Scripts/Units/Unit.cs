#define HERE_TO_CATCH_A_BUG
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public partial class Unit : SpawnableSprite {

    protected override SpawnableSpriteData BasicData {
        get { return this.Data; }
    }

    public new UnitData Data { get; private set; }

    public override MapTile TargetTile { get; set; }

    public UnitGroup Group { get; set; }
    public bool Invisible { get; set; }
    public bool OffensiveMoving { get; set; }

    private StatusBar statusBar;
    private IEnumerator wanderEnumerator = null;

    public override SpawnableSpriteType SpriteType {
        get { return (SpawnableSpriteType) this.Type; }
    }
    
    public UnitType Type {
        get { return this.Data.Type; }
    }

    public void ApproachBuilding () {
        this.Collider.enabled = false;
        this.selection.SetVisible (false);
        this.PhasedOut = true;
        
        if (this.Selected) {
            this.GameController.CurrentGroup.Remove (this);
            //this.SetSelected (false);
            this.GameController.ClearSelection ();
        }
    }

    public void Attack (SpawnableSprite target) {
        this.GetTrait<IUnitTraitAttacker> ().Attack (target);
    }

    public void Attack (MapTile target) {
        this.GetTrait<IUnitTraitAttacker> ().Attack (target);
    }

    protected override void Awake () {
        base.Awake ();

        this.OffensiveMoving = false;
    }

    public void Cast (SpellType spellType, Building building, MapTile tile) {
        this.GetTrait<IUnitTraitSpellcaster> ().Cast (spellType, building, tile);
    }
    
    public void Cast (SpellType spellType, MapTile tile) {
        this.GetTrait<IUnitTraitSpellcaster> ().Cast (spellType, tile);
    }

    public void Cast (SpellType spellType, Unit unit, MapTile tile) {
        this.GetTrait<IUnitTraitSpellcaster> ().Cast (spellType, unit, tile);
    }
    
    public void ChangePath (List<MapTile> waypoints) {
        this.GetTrait<IUnitTraitMoving> ().ChangePath (waypoints);
    }

    public void Decay () {
        this.GetTrait<IUnitTraitDecaying> ().Activate ();
    }

    public void Detect (Building building) {
        //this.CurrentMode.Detect (building);
    }

    public void Detect (Unit unit) {
        //this.CurrentMode.Detect (unit);
    }

    public override void Die (DeathType deathType = DeathType.None) {
        if (! this.Dead) {
            if (this.Selected) {
                this.GameController.OnGameOver ();
            }

            this.Group.Die (this.Group.GetIndex (this));

            this.Collider.enabled = false;

            Vector3 relativePosition = this.GetTrait<IUnitTraitMoving> ().RelativePosition;

            MapTile spawnTile = this.Tile;

            if (this.TargetTile != null) {
                float distanceToTargetTile = Vector3.Distance (this.Transform.position, this.TargetTile.RealPosition);
                float distanceToTile = Vector3.Distance (this.Transform.position, this.Tile.RealPosition);

                spawnTile = distanceToTargetTile > distanceToTile ? this.Tile : this.TargetTile;
            }

            if (deathType != DeathType.Falling) {
                ItemIdentifier itemIdentifier = this.SpawnFactory.GetRandomItemIdentifier (
                    ItemIdentifier.Crystal, ItemIdentifier.HealingCrystal
                );

                //TODO: this relative position won't make sense if we're talking about the target tile
                this.SpawnFactory.SpawnItem (itemIdentifier, spawnTile.MapPosition, relativePosition);
            }

            base.Die ();

            if (this.Selectable && this.Selected) {
                this.SetSelected (false);
                this.GameController.CurrentGroup.Remove (this);
            }

            this.SetTrait<IUnitTraitAttacker> (this.GameObject.AddComponentIfNecessary<UnitTraitNonAttacker> ());
            this.SetTrait<IUnitTraitBuilder> (this.GameObject.AddComponentIfNecessary<UnitTraitNonBuilder> ());
            this.SetTrait<IUnitTraitDecaying> (this.GameObject.AddComponentIfNecessary<UnitTraitNonDecaying> ());
            this.SetTrait<IUnitTraitInteractive> (this.GameObject.AddComponentIfNecessary<UnitTraitNonInteractive> ());
            this.SetTrait<IUnitTraitMender> (this.GameObject.AddComponentIfNecessary<UnitTraitNonMender> ());
            this.SetTrait<IUnitTraitMiner> (this.GameObject.AddComponentIfNecessary<UnitTraitNonMiner> ());
            this.SetTrait<IUnitTraitMoving> (this.GameObject.AddComponentIfNecessary<UnitTraitNonMoving> ());
            this.SetTrait<IUnitTraitShooter> (this.GameObject.AddComponentIfNecessary<UnitTraitNonShooter> ());
            this.SetTrait<IUnitTraitSpellcaster> (this.GameObject.AddComponentIfNecessary<UnitTraitNonSpellcaster> ());

            UnitTraitNonMoving trait = (UnitTraitNonMoving) this.GetTrait<IUnitTraitMoving> ();
            trait.Initialize (this, this.SpawnFactory.GetData<UnitTraitDataNonMoving> ());
            trait.RelativePosition = relativePosition;

            //this.Transform.SetZ (this.Transform.position.z - 2.5f);

            //this.AudioManager.Play (this.Data.DeadSound);

            if (deathType == DeathType.Falling) {
                this.StartCoroutine (this.Fall (trait));
            } else {
                this.Play (AnimationType.Dying);
                this.HealthBar.Deactivate ();
            }

            this.ReleaseClaimedTiles ();

            //this.Collider.enabled = false;

            this.MissionStatistics.Score -= 20;
            this.MissionStatistics.UnitsEnemyDestroyed ++;

            this.Faction.RemoveUnit (this);
            this.Map.RemoveUnit (this);

            /*
            Projectile projectile = Projectile.Instantiate<Projectile> (this.Data.ProjectilePrefab);
            position.z -= 10.0f;
            projectile.Transform.position = position;
            projectile.Tile = this.Tile;
            projectile.Activate ();
            */

            //this.OnAnimationTrigger (AnimationType.Decomposing, AnimationTriggerType.OnDecomposed);

            /*
            int count = this.Map.GetUnits (UnitType.Wolfie).Count;

            if (count == 0) {
                ServiceLocator.Instance.ScoreText.text = "You win... but AT WHAT COST!?";
            } else {
                ServiceLocator.Instance.ScoreText.text = string.Format ("{0} wolfies remaining", count);
            }
            */
        }
    }

    private void EnterAttackingMode () {
        this.ContextMenu.SetNode (this.ContextMenu.CancelNode);
    }

    public void EnterBuilding () {
        this.Renderer.enabled = false;
        this.Play (AnimationType.Idle);
    }

    private void EnterMendingMode () {
        this.ContextMenu.SetNode (this.ContextMenu.CancelNode);
    }

    private void EnterSpellcastingMode (SpellType spellType, bool targetMustBeAlive = true) {
        IUnitTraitSpellcaster trait = this.GetTrait<IUnitTraitSpellcaster> ();

        if (! trait.MayCast (spellType)) {
            this.statusBar.SetText ("Not enough mana to cast spell", 3.0f);
            //Debug.Log (this.Type + " may not cast " + spellType);
            return;
        }

        if (trait.RequiresTarget (spellType)) {
            this.ContextMenu.SetNode (this.ContextMenu.CancelNode);
        } else {
            trait.Cast (spellType, this.Tile);
        }
    }

    private IEnumerator Fall (UnitTraitNonMoving trait) {
        this.GameController.CurrentGroup.Clear ();

        this.Play (AnimationType.Walking);
        this.GetComponent<Light>().enabled = true;

        MapTile targetTile = this.Tile.GetNeighbour (this.Direction);
        Vector3 basePosition = this.Tile.RealPosition;
        float hitPoints = this.CurrentHitPoints;
        float delta = 1.0f;
        float totalDistance = Vector3.Distance (this.Tile.RealPosition, targetTile.RealPosition);

        do {
            yield return null;

            Vector3 origin = basePosition + trait.RelativePosition;
            Vector3 destination = targetTile.RealPosition;

            float distance = Vector3.Distance (origin, destination);

            trait.RelativePosition = Vector3.Lerp (origin, destination, (50.0f * Time.deltaTime) / distance);
            this.transform.position = trait.RelativePosition;
            trait.RelativePosition -= basePosition;

            delta = Mathf.InverseLerp (
                1.0f,
                totalDistance,
                Vector3.Distance (this.Transform.position, targetTile.RealPosition)
            );

            this.SetHitPoints (Mathf.FloorToInt (delta * hitPoints));
        } while (delta > 0.0f);

        this.GetComponent<Light>().enabled = false;

        this.Play (AnimationType.Dying);
        this.HealthBar.Deactivate ();
    }

    public void ForcedMove (
        IMovementDestination destination,
        bool overlapTarget = true,
        bool recalculation = false
    ) {
        this.ForcedMove (destination, this.GetTrait<IUnitTraitMoving> (), overlapTarget, recalculation);
    }

    public void ForcedMove (
        IMovementDestination destination,
        IMovementListener movementListener,
        bool overlapTarget = true,
        bool recalculation = false
    ) {
        this.GetTrait<IUnitTraitAttacker> ().Deactivate ();
        this.GetTrait<IUnitTraitMender> ().Deactivate ();
        this.GetTrait<IUnitTraitMiner> ().Deactivate ();

        this.Move (destination, movementListener, overlapTarget, recalculation);
    }

    //Returns either Tile or TargetTile, whichever is closest
    public override MapTile GetClosestTile () {
        if (this.TargetTile == null) {
            return this.Tile;
        }

        float distanceFromTarget = Vector3.Distance (this.Transform.position, this.TargetTile.RealPosition);
        float totalDistance = Vector3.Distance (this.Tile.RealPosition, this.TargetTile.RealPosition);
        float progress = Mathf.InverseLerp (totalDistance, 0.0f, distanceFromTarget);

        return (progress > 0.5f) ? this.TargetTile : this.Tile;
    }

    public override MapTile GetRealTile () {
        return this.TargetTile ?? this.Tile;
    }

    public MapTile GetTargetTile () {
        return this.TargetTile;
    }

    public override void Initialize (MapTile tile) {
        base.Initialize (tile);

        this.statusBar = ServiceLocator.Instance.StatusBar;

        this.ClaimTile (tile);
        this.Transform.position = tile.RealPosition;

        this.Collider.enabled = true;

        if (this.Faction.ControllingPlayer.Data.HumanPlayer) {
		    //this.Tile.Discover ();
        }

        this.MeshAnimator.RegisterTriggerListener (this);
    }

    protected override void InitializeSelection () {
        this.selection.InitializeSelection (this.transform, this.Data.SelectionSize);
        //this.GetComponent<BoxCollider2D> ().size = this.Data.SelectionSize * 32.0f;
    }

    public void Interact (Unit unit) {
        this.GetTrait<IUnitTraitInteractive> ().Interact (unit.GetTrait<IUnitTraitInteractive> ());
    }

    public void Interact (MapTile tile) {
        this.GetTrait<IUnitTraitInteractive> ().Interact (tile);
    }

    private void LateUpdate () {
        this.GetTrait<IUnitTraitMoving> ().LateManualUpdate ();
        this.UpdateDepth ();
    }

    public void LeaveBuilding () {
        this.Renderer.enabled = true;
        this.Collider.enabled = true;
    }

    public override void ManualUpdate () {
        if (GameController.Paused) {
            return;
        }
        
        base.ManualUpdate ();

#if HERE_TO_CATCH_A_BUG
        if (this.GetTrait<IUnitTraitMoving> () == null) {
            Debug.Log (this);
            return;
        }
#endif
        
        this.GetTrait<IUnitTraitMoving> ().ManualUpdate ();
        this.GetTrait<IUnitTraitAttacker> ().ManualUpdate ();
    }

    public bool MayCast (SpellType spellType) {
        return this.GetTrait<IUnitTraitSpellcaster> ().MayCast (spellType);
    }

    public void Mend (Building building) {
        this.GetTrait<IUnitTraitMender> ().Mend (building);
    }

    public void Mend (Decoration decoration) {
        this.GetTrait<IUnitTraitMender> ().Mend (decoration);
    }

    public void Mine (Building mine) {
        this.GetTrait<IUnitTraitMiner> ().Mine (mine);
    }

    public void Move (
        IMovementDestination destination,
        bool overlapTarget = true,
        bool recalculation = false
    ) {
        this.Move (destination, this.GetTrait<IUnitTraitMoving> (), overlapTarget, recalculation);
    }

    public void Move (
        IMovementDestination destination,
        IMovementListener movementListener,
        bool overlapTarget = true,
        bool recalculation = false
    ) {
        this.GetTrait<IUnitTraitMoving> ().Move (destination, movementListener, overlapTarget, recalculation);
    }

    public override void OnAnimationTrigger (AnimationType animationType, AnimationTriggerType triggerType) {
        base.OnAnimationTrigger (animationType, triggerType);

        switch (triggerType) {
            case AnimationTriggerType.OnAttack:
                this.OnAttack ();
                break;

            case AnimationTriggerType.OnDecomposed:

                break;

            case AnimationTriggerType.OnFinished:
                switch (animationType) {
                    case AnimationType.Attacking:
                        this.GetTrait<IUnitTraitAttacker> ().AttackAfterCooldown ();
                        break;

                    case AnimationType.Mending:
                        this.GetTrait<IUnitTraitMender> ().MendAfterCooldown ();
                        break;

                    default:
                        throw new NotSupportedException($"Received unexpected value: {animationType}");
                }

                break;

            case AnimationTriggerType.OnMending:
                this.OnMend ();
                break;

            default:
                throw new NotSupportedException($"Received unexpected value: {triggerType}");
        }
    }
    
    protected void OnAttack () {
        this.GetTrait<IUnitTraitAttacker> ().OnAttack ();
    }

    /*
    protected void OnAttackingAnimationFinished (object sender, EventArgs args) {
        this.GetTrait<IUnitTraitAttacker> ().AttackAfterCooldown ();
    }
    */

    private void OnDeathAnimationFinished (object sender, EventArgs args) {
        //this.Play (SpriteAnimationType.Decomposing, 2.0f);
    }

    /*
    private void OnTriggerEnter2D (Collider2D other) {
        Unit unit = other.GetComponent<Unit> ();
        
        if (unit != null && ! unit.Dead && ! this.Dead && unit.Faction.IsEnemy (this.Faction)) {
            int hitPoints = this.CurrentHitPoints;
            this.Damage (unit.CurrentHitPoints);
            unit.Damage (hitPoints);
        }
    }
    */

    public override bool OnManualMouseDown () {
        base.OnManualMouseDown ();
        /*
        Unit unit = this.GameController.CurrentGroup.GetNextUnit();

        if (unit == this) {
            return false;
        }

        if (this.inputController.MayNotify (unit)) {
            unit.Interact (this);

            return true;
        }
        */
        return false;
    }

    private void OnMend () {
        this.GetTrait<IUnitTraitMender> ().OnMend ();
    }

    public void OnOrderAccepted () {
        //this.AudioManager.Play (this.Data.AcknowledgeSound);
    }

    public void OnPathFindingFailed () {

    }

    public override void Play (AnimationType type, bool inverted = false) {
        base.Play (type, inverted);

        if (this.Data.WanderWhileIdle) {
            if (type == AnimationType.Idle && this.wanderEnumerator == null) {
                this.wanderEnumerator = this.WanderCoroutine ();
                this.StartCoroutine (this.wanderEnumerator);
            } else if (type != AnimationType.Idle && this.wanderEnumerator != null) {
                this.StopCoroutine (this.wanderEnumerator);
                this.wanderEnumerator = null;
            }
        }
    }

    public override void RefreshPosition () {
        this.GetTrait<IUnitTraitMoving> ().RefreshPosition ();
    }

    protected void RegisterAnimationFinished (AnimationType type, EventHandler handler) {
        /*
        if (this.AnimationsByType.ContainsKey (type))
            this.AnimationsByType[type].Finished += new AnimationFinishedHandler (handler);
        else
            Debug.LogError (this + " does not have an animation of type " + type);
        */
    }

    protected void SetData (UnitData data) {
        this.Data = data;
        base.SetData (data);
        this.SetTraits (data);

        this.RegisterAnimationFinished (AnimationType.Dying, this.OnDeathAnimationFinished);
        //this.RegisterAnimationFinished (SpriteAnimationType.Decomposing, this.OnDecomposingAnimationFinished);

        /*
        if (this.HasTrait (UnitTraitType.Attacker)) {
            this.RegisterAnimationFinished (AnimationType.Attacking, this.OnAttackingAnimationFinished);
        } else if (this.HasTrait (UnitTraitType.Builder)) {
            this.RegisterAnimationFinished (AnimationType.Harvesting, this.OnAttackingAnimationFinished);
        }
        */

        //this.RegisterSpriteTrigger (SpriteTriggerType.Attack, this.OnAttackSpriteTrigger);

        this.SetRandomDirection ();

        //this.HealthBar.Initialize (this);
    }

    public void SetDestination (MapTile tile) {
        this.GetTrait<IUnitTraitMoving> ().SetDestination (tile);
    }

    public void SetRandomDirection () {
        if (this.GetTrait<IUnitTraitMoving> ().MayMoveDiagonally) {
            this.Direction = (Direction) Random.Range ((int) Direction.North, (int) Direction.Northwest);
        } else {
            this.Direction = (Direction) (Random.Range (0, 4) * 2 + 1);
        }

        this.Play (AnimationType.Idle);
    }

    public override void SetSelected (bool selected) {
        base.SetSelected (selected);

        //this.GetTrait<IUnitTraitMoving> ().SetSelected (selected);
    }

    private void SetTraits (UnitData data) {
        this.InitializeTraits ();

        if (data.AttackerTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonAttacker> ().AddTrait (this);
        } else {
            data.AttackerTrait.AddTrait (this);
        }

        if (data.BuilderTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonBuilder> ().AddTrait (this);
        } else {
            data.BuilderTrait.AddTrait (this);
        }

        if (data.DecayingTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonDecaying> ().AddTrait (this);
        } else {
            data.DecayingTrait.AddTrait (this);
        }

        if (data.InteractiveTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonInteractive> ().AddTrait (this);
        } else {
            data.InteractiveTrait.AddTrait (this);
        }

        if (data.MenderTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonMender> ().AddTrait (this);
        } else {
            data.MenderTrait.AddTrait (this);
        }
        
        if (data.MinerTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonMiner> ().AddTrait (this);
        } else {
            data.MinerTrait.AddTrait (this);
        }
        
        if (data.MovingTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonMoving> ().AddTrait (this);
        } else {
            data.MovingTrait.AddTrait (this);
        }

        if (data.ShooterTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonShooter> ().AddTrait (this);
        } else {
            data.ShooterTrait.AddTrait (this);
        }

        if (data.SpellcasterTrait == null) {
            this.SpawnFactory.GetData<UnitTraitDataNonSpellcaster> ().AddTrait (this);
        } else {
            data.SpellcasterTrait.AddTrait (this);
        }
    }

    public void SetUnitType (UnitType type) {
        this.SetData (this.SpawnFactory.GetData (type));
    }

    public void Shoot (IShootingListener listener, Projectile projectilePrefab, ITarget target) {
        this.GetTrait<IUnitTraitShooter> ().Shoot (listener, projectilePrefab, target);
    }

    public void Stop () {
        this.GetTrait<IUnitTraitMoving> ().Deactivate ();
    }

    public void UpdateDepth () {
        /*
        if (this.Tile != null) {
            if (this.IsDead ()) {
                if (this.Tile.Slot != null) {
                    this.transform.SetZ (this.Tile.Slot.GetLayer (1).Transform.position.z - 0.1f);
                }
            } else {
                float rowOffset;

                if (Settings.Instance.Isometric) {
                    if (this.Tile.Visible) {
                        rowOffset = this.Tile.Slot.SceneObject.Transform.position.z;

                        if (this.TargetTile != null && this.TargetTile.Visible) {
                            float targetZ = this.TargetTile.Slot.SceneObject.Transform.position.z;

                            /*
                            float totalDistance = Vector3.Distance (
                                this.Tile.RealPosition,
                                this.TargetTile.RealPosition
                            );

                            float distanceToTarget = Vector3.Distance (
                                this.Transform.position,
                                this.TargetTile.RealPosition
                            );

                            float delta = 1.0f - distanceToTarget / totalDistance;

                            Debug.Log (delta);
                            */

/*                            if (this.Direction == Direction.North || this.Direction == Direction.East) {
                                rowOffset = Mathf.Max (rowOffset, targetZ);
                            } else {
                                //rowOffset = Mathf.Lerp (rowOffset, targetZ, delta);
                                //rowOffset = Mathf.Min (rowOffset, targetZ);
                            }
                        }

                        rowOffset -= 21.0f;
                        //rowOffset -= 7.5f;
                    } else {
                        rowOffset = 0.0f;
                    }

                    //rowOffset = this.Tile.Visible ? this.Tile.Slot.SceneObject.Transform.position.z - 41.0f : 0.0f;

                    //rowOffset = Camera.main.WorldToViewportPoint (this.Transform.position).y;
                    //rowOffset = Mathf.Lerp (- 30.0f, 0.0f, rowOffset);
                } else {
                    rowOffset = this.Tile.Visible ? this.Tile.Slot.Row * 2.0f : 0.0f;
                    rowOffset = - 2.0f - rowOffset;
                }

                if (this.name.Contains ("Elara")) {
                    rowOffset --;
                }

                this.Transform.SetZ (rowOffset);
            }
        }*/
    }

    private IEnumerator WanderCoroutine () {
        do {
            float seconds = Random.Range (2.0f, 10.0f);
            yield return new WaitForSeconds (seconds);

            this.SetRandomDirection ();
        } while (true);
    }
}
