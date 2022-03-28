using System.Collections;
using UnityEngine;

public class BuildingTraitDataResearcher : BuildingTraitData {
    
    public override BuildingTraitType Type {
        get { return BuildingTraitType.Researcher; }
    }

    public override BuildingTrait AddTrait (Building building) {
        BuildingTraitResearcher trait = building.gameObject.AddComponent<BuildingTraitResearcher> ();
        trait.Initialize (building, (BuildingTraitDataResearcher) BuildingTraitDataResearcher.Instantiate (this));
        
        building.SetTrait<IBuildingTraitResearcher> (trait);
        
        return trait;
    }
}
