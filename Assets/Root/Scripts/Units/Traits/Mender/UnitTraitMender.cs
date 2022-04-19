using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public class UnitTraitMender : UnitTrait, IDeathListener, IMovementListener, IUnitTraitMender {

    public UnitTraitDataMender Data { get; private set; }

    private SpawnableSprite target;
    private bool engaging = false;
    private MapTile lastTargetTile;
    private Map map;
    private IEnumerator mendAfterCooldownCoroutine = null;

    public override UnitTraitType Type {
        get { return UnitTraitType.Mender; }
    }

    public void ApproachingTarget() {

    }

    protected int CalculateMendAmount() {
        return this.Data.MinimumMendAmount + Random.Range(0, this.Data.RandomMendAmount);
    }

    public override void Deactivate() {
        base.Deactivate();

        this.Unit.MeshAnimator.UnregisterTriggerListener(this);

        if (this.mendAfterCooldownCoroutine != null) {
            this.StopCoroutine(this.mendAfterCooldownCoroutine);
        }

        this.Disengage();
    }

    private void Disengage() {
        this.engaging = false;

        /*
        if (this.Unit.CurrentAnimationType == AnimationType.Mending) {
            //TODO: If the line below is uncommented, mending animations get interrupted when the target dies.
            //Find some better way to prevent units from mending dead targets
            //this.Unit.Play (AnimationType.Idle);
        }
        */
    }

    private void Engage() {
        if (this.engaging) {
            return;
        }

        this.engaging = true;

        this.Unit.Stop();

        this.Unit.FindDirection(this.Unit.Tile, this.target.Tile);
        this.Unit.Play(AnimationType.Mending);
    }

    public void Initialize(Unit unit, UnitTraitDataMender data) {
        base.Initialize(unit);

        this.Data = data;

        this.map = ServiceLocator.Instance.Map;
    }

    protected bool IsTargetInRange() {
        MapTile tile = this.Unit.GetRealTile();

        Vector2Int destination = this.map.FindClosestBoundary(tile, this.target);

        int distance = tile.MapPosition.EstimateDistance(destination);

        return (this.Data.MendRange >= distance - 1);
    }

    public bool IsTileTraversable(MapTile tile) {
        return tile.IsTraversable(this.Unit.GetTrait<IUnitTraitMoving>().MovementType, this.Unit);
    }

    public void Mend(Building building) {
        this.Mend((SpawnableSprite)building);
    }

    public void Mend(Decoration decoration) {
        this.Mend((SpawnableSprite)decoration);
    }

    private void Mend(SpawnableSprite sprite) {
        this.Activate();

        this.SetTarget(sprite);
    }

    public void MendAfterCooldown() {
        if (this.mendAfterCooldownCoroutine != null) {
            this.StopCoroutine(this.mendAfterCooldownCoroutine);
        }

        this.mendAfterCooldownCoroutine = this.MendAfterCooldown(this.Data.MendCooldown);
        this.StartCoroutine(this.mendAfterCooldownCoroutine);
    }

    protected virtual IEnumerator MendAfterCooldown(float delay) {
        if (!this.Active || this.Unit.IsDead()) {
            yield break;
        }

        this.Unit.Play(AnimationType.Idle);

        if (this.IsTargetInRange()) {
            yield return new WaitForSeconds(delay);

            if (!this.Active || this.Unit.IsDead()) {
                yield break;
            }

            if (this.IsTargetInRange()) {
                this.Unit.Play(AnimationType.Mending);
            } else {
                this.RecalculatePath();
            }
        } else {
            this.RecalculatePath();
        }
    }

    public virtual void MendTarget(int mendAmount) {
        if (this.Active) {
            this.Unit.FindDirection(this.Unit.Tile, this.target.GetRealTile());
        }

        this.target.Restore(mendAmount);
    }

    public void OnAnimationTrigger(AnimationType animationType, AnimationTriggerType triggerType) {
        if (animationType == AnimationType.Mending && triggerType == AnimationTriggerType.OnFinished) {
            this.MendAfterCooldown();
        }
    }

    public void OnDeathNotification(SpawnableSprite sprite) {

    }

    public void OnMend() {
        if (!this.Active) {
            return;
        }

        if (this.IsTargetInRange()) {
            this.MendTarget(this.CalculateMendAmount());

            if (this.target.CurrentHitPoints >= this.target.Data.HitPoints) {
                this.Unit.Play(AnimationType.Idle);
                this.Deactivate();
            }
        } else {
            this.RecalculatePath();
        }
    }

    public void OnOrderAccepted() {

    }

    public void ReachedTarget() {
        if (this.IsTargetInRange()) {
            this.Engage();
        } else {
            this.RecalculatePath();
        }

        //if (! this.engaging && ! this.target.IsDead ())
        //    this.unit.MoveTowards (this.target);
    }

    public void RecalculatePath() {
        this.Disengage();

        /*
        if (this.Unit.CurrentAnimationType != AnimationType.Walking) {
            this.Unit.Play (AnimationType.Idle);
        }
        */

        this.Unit.Move(this.target, this, false, true);//this.target.Tile == this.lastTargetTile);
        this.lastTargetTile = this.target.Tile;

        //        this.Unit.Move (this.target, false, true);
        //this.unit.StartCoroutine (this.map.FindPath (null, this.unit, this.unit.Tile, this.target.Tile, false));
    }

    /*
    public void SetDestination (Tile tile) {
        this.destination = tile;
    }
    */

    private void SetTarget(SpawnableSprite target) {
        //this.Unit.StopAllCoroutines ();
        //this.engaging = false;

        this.target = target;
        this.target.AddDeathListener(this);
        this.lastTargetTile = this.target.Tile;

        if (this.IsTargetInRange()) {
            this.Engage();
        } else {
            target.AddDeathListener(this);
            this.Unit.Move(this.target, this, false, false);
        }
    }

    public void TileChanged() {
        if (this.target.IsDead()/* || this.destination == null*/) {
            return;
        }

        if (this.IsTargetInRange()) {
            this.Engage();
        } else {
            this.Disengage();

            if (this.target.Tile == this.lastTargetTile) {
                // base.UpdateTargetTile (ref targetTile, ref waypoints);
                /*
                if (waypoints.Count > 0) {
                    Unit unit = waypoints[0].Unit;
                    if (unit != null && unit != this.Unit) {
                        this.RecalculatePath ();
                        return;
                    }
                    
                    this.Unit.TargetTile = waypoints[0];
                    waypoints.RemoveAt (0);
                    
                    this.Unit.FindDirection (this.Unit.Tile, this.Unit.TargetTile);
                    
                    if (waypoints.Count == 0) {
                        this.ApproachingTarget ();
                    }
                } else {
                    this.Unit.TargetTile = null;
                    
                    this.ReachedTarget ();
                }
                */
            } else {
                this.RecalculatePath();
                //this.lastTargetTile = this.target.Tile;
            }
        }
    }
}
