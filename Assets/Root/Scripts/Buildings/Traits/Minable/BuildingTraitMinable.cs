using System.Collections;
using UnityEngine;

public class BuildingTraitMinable : BuildingTrait, IBuildingTraitMinable {

    public BuildingTraitDataMinable Data { get; set; }

    private int remainingGold;

    public void Initialize (Building building, BuildingTraitDataMinable data) {
        base.Initialize (building);
        
        this.Data = data;
        this.remainingGold = data.RemainingGold;
    }

    public bool IsMinable () {
        if (this.remainingGold > 0) {
            return true;
        }

        Debug.Log ("Mine is depleted");

        return false;
    }

    public int Mine (IUnitTraitMiner miner) {
        int gold = this.remainingGold < 100 ? this.remainingGold : 100;
        
        this.remainingGold -= gold;
        
        if (this.remainingGold == 0) {
            this.Building.Die ();
        }
        
        return gold;
    }
}
