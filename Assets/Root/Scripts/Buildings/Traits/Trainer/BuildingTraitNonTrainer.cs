using System.Collections;
using UnityEngine;

public class BuildingTraitNonTrainer : BuildingTrait, IBuildingTraitTrainer {

    public BuildingTraitDataNonTrainer Data { get; set; }

    public void Initialize (Building building, BuildingTraitDataNonTrainer data) {
        base.Initialize (building);
        
        this.Data = data;
    }

    public void Train (UnitType type) {
        Debug.LogWarning (
            string.Format (
                "Trying to train '{0}' on '{1}', but it does not have the Trainer trait.",
                type,
                this.Building.Type
            )
        );
    }
}
