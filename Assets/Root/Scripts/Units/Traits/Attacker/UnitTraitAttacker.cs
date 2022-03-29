using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class UnitTraitAttacker : UnitTrait, IDeathListener, IMovementListener, IPhasedOutListener, IUnitTraitAttacker {

    public UnitTraitDataAttacker Data { get; private set; }

    protected AudioManager AudioManager { get; private set; }
    protected ITarget Target { get; private set; }

    private IEnumerator attackAfterCooldownCoroutine = null;
    //private Tile destination;
    private bool engaging = false;
    private float lastAttack = float.MinValue;
    private Map map;

    private Dictionary<Direction, List<IntVector2>> positionsInRange;

    /// <summary>
    /// The positions around the positions in range. For encompassing units that are about to be in range.
    /// </summary>
    private Dictionary<Direction, List<IntVector2>> extendedPositionsInRange;

    public override UnitTraitType Type {
        get { return UnitTraitType.Attacker; }
    }

    public void ApproachingTarget () {
        
    }
    
    public void Attack (SpawnableSprite target) {
        this.Activate ();

        this.SetTarget (target);
        this.Unit.OnOrderAccepted ();
    }

    public virtual void Attack (MapTile target) {

    }

    public void AttackAfterCooldown () {
        if (this.attackAfterCooldownCoroutine != null) {
            this.StopCoroutine (this.attackAfterCooldownCoroutine);
        }
        
        this.attackAfterCooldownCoroutine = this.AttackAfterCooldown (this.Data.AttackCooldown);
        this.StartCoroutine (this.attackAfterCooldownCoroutine);
    }
    
    protected virtual IEnumerator AttackAfterCooldown (float delay) {
        if (! this.Active || this.Unit.IsDead ()) {
            yield break;
        }

        // So triggers on the last frame of an attack animation get a chance to be processed
        yield return null;

        this.Unit.Play (this.Unit.TargetTile == null ? AnimationType.Idle : AnimationType.Walking);

        if (this.IsTargetInRange ()) {
            yield return new WaitForSeconds (delay);

            if (! this.Active || this.Unit.IsDead ()) {
                yield break;
            }

            if (this.IsTargetInRange ()) {
                this.lastAttack = Time.time;
                this.Unit.Play (AnimationType.Attacking);
            } else {
                this.Deactivate ();
            }
        } else {
            this.Deactivate ();
        }
    }
    
    protected int CalculateAttackDamage () {
        UpgradeRank upgrade = this.Unit.Faction.GetUpgrade (UpgradeType.Attack, this.Unit.Type);

        int strength = upgrade == null ? 0 : upgrade.Strength;

#if STRONG_ATTACKS
        strength = 500;
#endif

        return this.Data.MinimumDamage + Random.Range (0, this.Data.RandomDamage) + strength;
    }

    public virtual void DamageTarget (int damage) {
        /*
        if (this.Active) {
            this.Unit.FindDirection (this.Unit.Tile, this.Target.GetRealTile ());
        }
        */

        this.Target.Damage (damage);
    }
    
    public override void Deactivate () {
        base.Deactivate ();

        if (! this.Unit.Dead) {
            this.Unit.Play (this.Unit.TargetTile == null ? AnimationType.Idle : AnimationType.Walking);
        }

        this.Unit.MeshAnimator.UnregisterTriggerListener (this);

        if (this.attackAfterCooldownCoroutine != null) {
            this.StopCoroutine (this.attackAfterCooldownCoroutine);
        }

        this.Disengage ();
    }
    
    private void Disengage () {
        this.engaging = false;

        /*
        if (this.Unit.CurrentAnimationType == AnimationType.Attacking) {
            //TODO: If the line below is uncommented, attack animations get interrupted when the target dies.
            //Find some better way to prevent units from attacking dead targets
            //this.Unit.Play (AnimationType.Idle);
        }
        */
    }

    private void Engage () {
        if (this.engaging) {
            return;
        }

        if (this.Target.IsDead () || this.Target.PhasedOut) {
            this.Deactivate ();
            return;
        }

        if (this.lastAttack + this.Data.AttackCooldown > Time.time) {
            this.Deactivate ();
            return;
        }

        this.engaging = true;

        //this.Unit.Stop ();

        /*
        this.Unit.FindDirection (this.Unit.Tile, this.Target.Tile);
        */

        this.lastAttack = Time.time;
        this.Unit.Play (AnimationType.Attacking);
    }

    private Vector2 FindUnitPositionInPattern () {
        string[] lines = this.Data.RangePattern.Split ('\n');

        for (int y = 0; y < lines.Length; y ++) {
            int x = lines[y].IndexOf ("U");
            
            if (x > -1) {
                return new Vector2 (x, y);
            }
        }

        return Vector2.zero;
    }

    public List<MapTile> GetExtendedTilesInRange (Direction direction) {
        List<MapTile> extendedTilesInRange = new List<MapTile> ();

        MapTile currentTile = this.Unit.Tile;
        IntVector2 directionOffset = direction.GetData ().Offset;

        foreach (IntVector2 position in this.positionsInRange[direction]) {
            IntVector2 extendedPosition = position + directionOffset;

            MapTile tileInRange = this.map.GetTile (currentTile.MapPosition + extendedPosition);

            if (tileInRange != null) {
                extendedTilesInRange.Add (tileInRange);
            }
        }

        return extendedTilesInRange;
    }

    public List<MapTile> GetExtendedTilesInRange () {
        List<MapTile> extendedTilesInRange = new List<MapTile> ();
        
        MapTile currentTile = this.Unit.Tile;

        if (! this.extendedPositionsInRange.ContainsKey(this.Unit.Direction)) {
            return new List<MapTile>();
        }

        foreach (IntVector2 position in this.extendedPositionsInRange[this.Unit.Direction]) {
            MapTile tileInRange = this.map.GetTile (currentTile.MapPosition + position);
            
            if (tileInRange != null) {
                extendedTilesInRange.Add (tileInRange);
            }
        }
        
        return extendedTilesInRange;
    }

    public List<MapTile> GetTilesInRange () {
        if (! this.positionsInRange.ContainsKey (this.Unit.Direction)) {
            return new List<MapTile> ();
        }

        List<MapTile> tilesInRange = new List<MapTile> ();

        MapTile currentTile = this.Unit.Tile;
        tilesInRange.Add (currentTile);

        foreach (IntVector2 position in this.positionsInRange[this.Unit.Direction]) {
            MapTile tileInRange = this.map.GetTile (currentTile.MapPosition + position);
            
            if (tileInRange != null) {
                tilesInRange.Add (tileInRange);
            }
        }

        return tilesInRange;
    }
    
    public void Initialize (Unit unit, UnitTraitDataAttacker data) {
        base.Initialize (unit);

        this.Data = data;

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.AudioManager = serviceLocator.AudioManager;
        this.map = serviceLocator.Map;

        this.InitializePositionsInRange ();
        this.InitializeExtendedPositionsInRange ();
    }

    private void InitializeDirectionDictionary (ref Dictionary<Direction, List<IntVector2>> dictionary) {
        dictionary = new Dictionary<Direction, List<IntVector2>> () {
            { Direction.North, new List<IntVector2> () },
            { Direction.East,  new List<IntVector2> () },
            { Direction.South, new List<IntVector2> () },
            { Direction.West,  new List<IntVector2> () }
        };
    }

    private void InitializePositionsInRange () {
        this.InitializeDirectionDictionary (ref this.positionsInRange);

        Vector2 unitPosition = this.FindUnitPositionInPattern ();
        string[] lines = this.Data.RangePattern.Split ('\n');

        for (int y = 0; y < lines.Length; y ++) {
            for (int x = 0; x < lines[y].Length; x ++) {
                if (lines[y][x] == 'X') {
                    IntVector2 position = new IntVector2 (x - unitPosition.x, unitPosition.y - y);

                    this.positionsInRange[Direction.East].Add (position);
                    this.positionsInRange[Direction.West].Add (position * -1.0f);
                    this.positionsInRange[Direction.North].Add (new IntVector2 (position.Y * -1.0f, position.X));
                    this.positionsInRange[Direction.South].Add (new IntVector2 (position.Y, position.X * -1.0f));
                }
            }
        }
    }

    private void InitializeExtendedPositionsInRange () {
        this.InitializeDirectionDictionary (ref this.extendedPositionsInRange);

        foreach (Direction key in this.positionsInRange.Keys) {
            List<IntVector2> positions = this.positionsInRange[key];

            foreach (IntVector2 position in positions) {
                // Expand range in all directions
                foreach (Direction direction in this.positionsInRange.Keys) {
                    IntVector2 extendedPosition = position + direction.GetData().Offset;

                    if (! positions.Contains (extendedPosition)) {
                        this.extendedPositionsInRange[key].Add (extendedPosition);
                    }
                }
            }
        }
    }

    protected bool IsTargetInRange () {
        MapTile tile = this.Unit.GetRealTile ();
        IntVector2 destination = this.map.FindClosestBoundary(tile, this.Target);

        tile = this.map.GetTile(destination);

        List<MapTile> tilesInRange = this.GetTilesInRange();

        if (tilesInRange.Contains(tile)) {
            return true;
        }

        tilesInRange = this.GetExtendedTilesInRange ();

        if (tilesInRange.Contains (this.Target.TargetTile)) {
            return true;
        }

        tilesInRange = this.GetExtendedTilesInRange (this.Unit.Direction);

        if (tilesInRange.Contains (this.Target.GetClosestTile ())) {
            return true;
        }

        return false;
    }

    public bool IsTileTraversable (MapTile tile) {
        return tile.IsTraversable (this.Unit.GetTrait<IUnitTraitMoving> ().MovementType, this.Unit);
    }

    public void ManualUpdate () {
        if (this.Active) {
            return;
        }

        List<MapTile> tilesInRange = this.GetTilesInRange ();

        foreach (MapTile tile in tilesInRange) {
            Unit unit = tile.GetInhabitant<Unit> ();

            if (unit != null && unit.Faction.IsEnemy (this.Unit.Faction)) {
                this.Attack (unit);
                break;
            }
        }

        foreach (MapTile tile in this.GetExtendedTilesInRange ()) {
            Unit unit = tile.GetInhabitant<Unit> ();
            
            if (unit == null || ! unit.Faction.IsEnemy (this.Unit.Faction)) {
                continue;
            }

            if (tilesInRange.Contains (unit.TargetTile)) {
                this.Attack (unit);
                break;
            }
        }

        if (this.Unit.TargetTile != null && this.Unit.GetClosestTile () == this.Unit.TargetTile) {
            List<MapTile> extendedTiles = this.GetExtendedTilesInRange (this.Unit.Direction);

            foreach (MapTile tile in extendedTiles) {
                Unit unit = tile.GetInhabitant<Unit> ();

                if (unit == null || ! unit.Faction.IsEnemy (this.Unit.Faction)) {
                    continue;
                }

                if (extendedTiles.Contains (unit.GetClosestTile ())) {
                    this.Attack (unit);
                    break;
                }
            }
        }
    }

    public void OnAnimationTrigger (AnimationType animationType, AnimationTriggerType triggerType) {
        if (animationType == AnimationType.Attacking && triggerType == AnimationTriggerType.OnFinished) {
            this.AttackAfterCooldown ();
        }
    }
    
    public virtual void OnAttack () {
        if (! this.Active) {
            return;
        }
        
        if (this.IsTargetInRange ()) {
            this.DamageTarget (this.CalculateAttackDamage ());
            this.AudioManager.Play (this.Data.AttackSound);
        } else {
            this.Deactivate ();
        }
    }

    public void OnDeathNotification (SpawnableSprite sprite) {
        if (sprite == this.Target) {
            //Debug.Log ("target is dead");
            
            /*
            if (this.Unit.OffensiveMoving) {
                this.Unit.SetMode (UnitModeType.OffensiveMoving, force: true);
            } else {
                this.Unit.Stop ();
                this.Unit.SetMode (UnitModeType.Idle);
            }
            */

            this.Deactivate ();
        }
    }

    public void OnOrderAccepted () {

    }

    public void OnPhasedOut (SpawnableSprite sprite) {
        if (sprite == this.Target) {
            Debug.Log ("Phased out");
            this.Deactivate ();
            //this.Unit.Stop ();
        }
    }

    public void ReachedTarget () {
        if (this.IsTargetInRange ()) {
            this.Engage ();
        } else {
            this.Deactivate ();
        }

        //if (! this.engaging && ! this.target.IsDead ())
        //    this.unit.MoveTowards (this.target);
    }

    protected void SetTarget (ITarget target) {
        //this.Unit.StopAllCoroutines ();
        //this.engaging = false;
        
        this.Target = target;
        this.Target.AddDeathListener (this);
        this.Target.AddPhasedOutListener (this);

        if (this.IsTargetInRange ()) {
            this.Engage ();
        } else {
            this.Deactivate ();
        }
    }

    public void TileChanged () {
        if (! this.Active || this.Target.IsDead () || this.Target.PhasedOut /* || this.destination == null*/) {
            return;
        }
        
        if (this.IsTargetInRange ()) {
            this.Engage ();
        } else {
            this.Deactivate ();
        }
    }
}
