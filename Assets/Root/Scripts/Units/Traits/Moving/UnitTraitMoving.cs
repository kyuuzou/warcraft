#define DRAW_PATH

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTraitMoving : UnitTrait, IUnitTraitMoving {

    public UnitTraitDataMoving Data { get; set; }

    private MainCamera mainCamera;
    private Map map;
    private MapGrid grid;

    private Task movement = null;

    private MapTile destination = null;
    private MapTile lastDestinationPivot = null;

    private List<MapTile> waypoints;

    private Vector3 basePosition;

    private bool overlapTarget;
    private IMovementListener movementListener;

    private int recalculations = 0;
    private static readonly int recalculationLimit = 10;

#if DRAW_PATH
    private Color color;
#endif

    public bool MayMoveDiagonally {
        get { return this.Data.MayMoveDiagonally; }
    }

    public MovementType MovementType {
        get { return this.Data.MovementType; }
    }

    public Vector3 RelativePosition { get; set; }

    private bool SearchingPath { get; set; }

    public override UnitTraitType Type {
        get { return UnitTraitType.Moving; }
    }

    public void ApproachingTarget() {

    }

    public void ChangePath(List<MapTile> waypoints) {
        if (waypoints[0] == this.Unit.Tile) {
            waypoints.RemoveAt(0);
        }

        this.waypoints.Clear();
        this.waypoints.AddRange(waypoints);

        //if (this.Unit.TargetTile == null) {
        if (waypoints.Count == 0) {
            //Debug.Log (this, "Empty path");
            this.movementListener.ReachedTarget();
            return;
        }

        if (this.IsTileTraversable(waypoints[0])) {
            this.Unit.Play(AnimationType.Walking);
            this.recalculations = 0;
            this.lastDestinationPivot = null;
        }

        this.UpdateTargetTile();
        //}

        this.SearchingPath = false;
    }

    public override void Deactivate() {
        base.Deactivate();

        this.waypoints.Clear();

        if (!this.Unit.IsDead()) {
            this.Unit.Play(AnimationType.Idle);
        }
    }

    public void Initialize(Unit unit, UnitTraitDataMoving data) {
        base.Initialize(unit);

        this.Data = data;

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.grid = serviceLocator.Grid;
        this.map = serviceLocator.Map;
        this.mainCamera = serviceLocator.MainCamera;

        this.SearchingPath = false;
        this.waypoints = new List<MapTile>();

#if DRAW_PATH
        this.color = new Color(Random.Range(0.25f, 1.0f), Random.Range(0.25f, 1.0f), Random.Range(0.25f, 1.0f));
#endif
    }

    public bool IsTileTraversable(MapTile tile) {
        return tile.IsTraversable(this.Unit.GetTrait<IUnitTraitMoving>().MovementType, this.Unit);
    }

    public void LateManualUpdate() {

    }

    public void ManualUpdate() {
        if (this.Unit.Tile == null) {
            return;
        }

        this.basePosition = this.Unit.Tile.RealPosition;

        if (this.Unit.TargetTile != null && !this.Unit.IsDead()) {
            Vector3 origin = this.basePosition + this.RelativePosition;
            Vector3 destination = this.Unit.TargetTile.RealPosition;

            float distance = Vector3.Distance(origin, destination);

            this.RelativePosition = Vector3.Lerp(origin, destination, (this.Data.Speed * Time.deltaTime) / distance);

            this.transform.position = this.RelativePosition;
            this.RelativePosition -= this.basePosition;

            if (Vector3.Distance(this.transform.position, destination) < 0.1f) {
                this.Unit.Tile = this.Unit.TargetTile;

                this.Unit.ReleaseClaimedTiles();
                this.Unit.ClaimTile(this.Unit.Tile);

                this.RelativePosition = destination - this.transform.position;

                this.movementListener.TileChanged();
                this.UpdateTargetTile();
            }
        } else {
            this.transform.position = this.basePosition + this.RelativePosition;
        }
    }

    public void Move(
        IMovementDestination destination, IMovementListener movementListener, bool overlapTarget, bool recalculation
    ) {
        if (destination == null) {
            return;
        }

        if (this.movement != null) {
            this.movement.Stop();
        }

        this.overlapTarget = overlapTarget;
        this.movementListener = movementListener;

        if (recalculation && destination.Pivot == this.lastDestinationPivot) {
            if (this.ReachedRecalculationLimit()) {
                return;
            }
        }

        this.lastDestinationPivot = destination.Pivot;

        this.movement = new Task(this.MoveCoroutine(destination, overlapTarget));
        this.movement.Start();
    }

    private IEnumerator MoveCoroutine(IMovementDestination destination, bool overlapTarget = true) {
        if (!this.isActiveAndEnabled) {
            yield break;
        }

        MapTile origin = this.Unit.TargetTile == null ? this.Unit.Tile : this.Unit.TargetTile;

        IEnumerator coroutine = this.map.FindPath(
            this.movement,
            this.Unit,
            origin,
            destination,
            overlapTarget,
            this.movementListener
        );

        Task task = new Task(coroutine, false, this.movement);
        yield return this.StartCoroutine(task.YieldStart());
    }

#if DRAW_PATH
    private void OnDrawGizmos() {
        if (!Application.isPlaying) {
            return;
        }

        MapTile lastWaypoint = this.Unit.Tile;

        Gizmos.color = this.color;

        if (this.Unit.TargetTile != null) {
            Gizmos.DrawLine(lastWaypoint.RealPosition, this.Unit.TargetTile.RealPosition);
            lastWaypoint = this.Unit.TargetTile;
        }

        foreach (MapTile waypoint in this.waypoints) {
            Gizmos.DrawLine(lastWaypoint.RealPosition, waypoint.RealPosition);
            lastWaypoint = waypoint;
        }
    }
#endif

    public void OnGroupChanged() {

    }

    public void OnOrderAccepted() {

    }

    private bool ReachedRecalculationLimit() {
        this.recalculations++;

        if (this.recalculations > UnitTraitMoving.recalculationLimit) {
            //Debug.LogWarning (this + " gave up trying to recalculate path.");

            return true;
        }

        return false;
    }

    public void ReachedTarget() {
        //if (this.Destination == this.Unit.Tile) {
        //this.Unit.SetMode (UnitModeType.Idle);

        this.Unit.Play(AnimationType.Idle);
        //}
    }

    public virtual void RecalculatePath() {
        this.Move(this.destination, this.movementListener, this.overlapTarget, true);
    }

    public void RefreshPosition() {
        
    }

    public void SetDestination(MapTile tile) {
        this.destination = tile;
    }

    public void TileChanged() {

    }

    public void UpdateTargetTile() {
        if (this.waypoints.Count > 0) {
            MapTile tile = this.waypoints[0];

            if (!this.movementListener.IsTileTraversable(tile)) {
                this.Unit.Stop();
                this.Unit.Play(AnimationType.Idle);
                this.RecalculatePath();
                return;
            }

            this.Unit.TargetTile = tile;

            tile.Discover();
            this.waypoints.RemoveAt(0);

            this.Unit.FindDirection(this.Unit.Tile, this.Unit.TargetTile);

            if (this.waypoints.Count == 0) {
                this.movementListener.ApproachingTarget();
            }
        } else {
            this.Unit.TargetTile = null;

            this.movementListener.ReachedTarget();
        }

        if (this.Unit.TargetTile != null) {
            Unit unit = this.Unit.TargetTile.GetInhabitant<Unit>();

            if (unit == null) {
                this.Unit.ClaimTile(this.Unit.TargetTile);
            }
        }
    }
}
