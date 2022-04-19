public class BuildingTraitDataNonMinable : BuildingTraitData {

    public override BuildingTraitType Type {
        get { return BuildingTraitType.Minable; }
    }

    public override BuildingTrait AddTrait(Building building) {
        BuildingTraitNonMinable trait = building.gameObject.AddComponent<BuildingTraitNonMinable>();
        trait.Initialize(building, BuildingTraitDataNonMinable.Instantiate(this));

        building.SetTrait<IBuildingTraitMinable>(trait);

        return trait;
    }
}
