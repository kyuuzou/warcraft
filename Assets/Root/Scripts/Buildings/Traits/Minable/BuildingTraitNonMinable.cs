public class BuildingTraitNonMinable : BuildingTrait, IBuildingTraitMinable {

    public BuildingTraitDataNonMinable Data { get; set; }

    public void Initialize(Building building, BuildingTraitDataNonMinable data) {
        base.Initialize(building);

        this.Data = data;
    }

    public bool IsMinable() {
        return false;
    }

    public int Mine(IUnitTraitMiner miner) {
        return 0;
    }
}
