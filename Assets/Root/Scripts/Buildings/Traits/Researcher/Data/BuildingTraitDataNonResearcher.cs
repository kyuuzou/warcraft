using System.Collections;
using UnityEngine;

public class BuildingTraitDataNonResearcher : BuildingTraitData {
    
    public override BuildingTraitType Type {
        get { return BuildingTraitType.Researcher; }
    }

    public override BuildingTrait AddTrait (Building building) {
        BuildingTraitNonResearcher trait = building.gameObject.AddComponent<BuildingTraitNonResearcher> ();
        trait.Initialize (building, BuildingTraitDataNonResearcher.Instantiate (this));
        
        building.SetTrait<IBuildingTraitResearcher> (trait);
        
        return trait;
    }
}
