using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTraitNonMoving : UnitTrait, IUnitTraitMoving {

    public UnitTraitDataNonMoving Data { get; set; }

    public override bool IsNullObject {
        get { return true; }
    }

    public bool MayMoveDiagonally {
        get { return false; }
    }

    public MovementType MovementType { 
        get { return MovementType.Land; }
    }

    public Vector3 RelativePosition { get; set; }

    public override UnitTraitType Type {
        get { return UnitTraitType.Moving; }
    }

    public void ApproachingTarget () {
        
    }
    
    public void ChangePath (List<MapTile> waypoints) {

    }

    public override void Deactivate () {
        
    }

    public void Initialize (Unit unit, UnitTraitDataNonMoving data) {
        base.Initialize (unit);
        
        this.Data = data;

        this.Activate ();
    }

    public bool IsTileTraversable (MapTile tile) {
        return true;
    }

    public void LateManualUpdate () {
        if (this.Unit.Tile == null) {
            return;
        }

        this.RefreshPosition ();
    }

    public void ManualUpdate () {

    }
    
    public void Move (
        IMovementDestination destination, IMovementListener movementListener, bool overlapTarget, bool recalculation
    ) {

    }

    public void OnGroupChanged () {

    }

    public void OnOrderAccepted () {
        
    }
    
    public void ReachedTarget () {
        
    }
    
    public void RefreshPosition () {
        if (! this.Active) {
            return;
        }

        Vector3 position = this.Unit.Tile.RealPosition + this.RelativePosition;
        position = position.Add(this.Unit.Offset);
        position.z -= 0.1f;

        this.transform.position = position;
    }

    public void SetDestination (MapTile tile) {

    }

    public void TileChanged () {

    }
}
