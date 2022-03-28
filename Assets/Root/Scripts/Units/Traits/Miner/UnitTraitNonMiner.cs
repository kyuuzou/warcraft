using System.Collections;
using UnityEngine;

public class UnitTraitNonMiner : UnitTrait, IUnitTraitMiner, IMovementListener {

    public UnitTraitDataNonMiner Data { get; set; }

    public override bool IsNullObject {
        get { return true; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Miner; }
    }

    public void ApproachingTarget () {

    }

    public void Initialize (Unit unit, UnitTraitDataNonMiner data) {
        base.Initialize (unit);
        
        this.Data = data;
    }

    public bool IsTileTraversable (MapTile tile) {
        return false;
    }

    public void Mine (Building building) {
        
    }

    public void OnOrderAccepted () {
        
    }
    
    public void ReachedTarget () {
        
    }

    public void TileChanged () {

    }
}
