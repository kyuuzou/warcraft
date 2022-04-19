public abstract class BuildingTrait : SpawnableTrait {

    protected Building Building { get; private set; }

    protected void Initialize(Building building) {
        this.Building = building;
    }
}
