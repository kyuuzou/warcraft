using UnityEngine;
using System.Collections;

public class InteractionModeBuildingArgs : InteractionModeArgs {
    
    private Building building;
    private Unit builder;
    
    public InteractionModeBuildingArgs (Building building, Unit peasant) {
        this.building = building;
        this.builder = builder;
    }
    
    public Building GetBuilding () {
        return this.building;
    }

    public Unit GetBuilder () {
        return this.builder;
    }
}
