using UnityEngine;
using System.Collections;

public class InteractionModeHarvestArgs : InteractionModeArgs {
    
    private Unit harvester;
    
    public InteractionModeHarvestArgs (Unit harvester) {
        this.harvester = harvester;
    }
    
    public Unit GetHarvester () {
        return this.harvester;
    }
}
