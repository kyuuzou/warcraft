using UnityEngine;

public class BuildingTraitDataMinable : BuildingTraitData {

    public override BuildingTraitType Type {
        get { return BuildingTraitType.Minable; }
    }

    [SerializeField]
    private int remainingGold = 0;
    public int RemainingGold {
        get { return this.remainingGold; }
    }

    public override BuildingTrait AddTrait(Building building) {
        BuildingTraitMinable trait = building.gameObject.AddComponent<BuildingTraitMinable>();
        trait.Initialize(building, BuildingTraitDataMinable.Instantiate(this));

        building.SetTrait<IBuildingTraitMinable>(trait);

        return trait;
    }
}
