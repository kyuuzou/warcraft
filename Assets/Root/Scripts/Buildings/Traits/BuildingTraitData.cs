using System.Collections;
using UnityEngine;

public abstract class BuildingTraitData : TraitData {
    
    public abstract BuildingTraitType Type { get; }

    public abstract BuildingTrait AddTrait (Building building);
}
