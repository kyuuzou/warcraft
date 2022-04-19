public class UnitTraitNonBuilder : UnitTrait, IUnitTraitBuilder, IMovementListener {

    public UnitTraitDataNonBuilder Data { get; set; }

    public override bool IsNullObject {
        get { return true; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Builder; }
    }

    public void ApproachingTarget() {

    }

    public void Build(BuildingType type) {

    }

    public void Initialize(Unit unit, UnitTraitDataNonBuilder data) {
        base.Initialize(unit);

        this.Data = data;
    }

    public bool IsTileTraversable(MapTile tile) {
        return false;
    }

    public void MoveToConstructionSite(Building building) {

    }

    public void OnOrderAccepted() {

    }

    public void OnWorkComplete() {

    }

    public void ReachedTarget() {

    }

    public void ShowAdvancedStructures() {

    }

    public void ShowBasicStructures() {

    }

    public void TileChanged() {

    }
}
