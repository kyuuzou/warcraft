using UnityEngine;
using System.Collections;

public abstract class BuildingTrait : SpawnableTrait {

    protected Building Building { get; private set; }

    protected void Initialize (Building building) {
        this.Building = building;
    }
}
