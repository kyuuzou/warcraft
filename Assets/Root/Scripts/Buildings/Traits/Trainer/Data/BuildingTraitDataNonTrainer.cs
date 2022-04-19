public class BuildingTraitDataNonTrainer : BuildingTraitData {

    public override BuildingTraitType Type {
        get { return BuildingTraitType.Trainer; }
    }

    public override BuildingTrait AddTrait(Building building) {
        BuildingTraitNonTrainer trait = building.gameObject.AddComponent<BuildingTraitNonTrainer>();
        trait.Initialize(building, BuildingTraitDataNonTrainer.Instantiate(this));

        building.SetTrait<IBuildingTraitTrainer>(trait);

        return trait;
    }
}
