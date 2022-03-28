using System.Collections;
using UnityEngine;

public class BuildingTraitNonResearcher : BuildingTrait, IBuildingTraitResearcher {

    public BuildingTraitDataNonResearcher Data { get; set; }

    public void Initialize (Building building, BuildingTraitDataNonResearcher data) {
        base.Initialize (building);
        
        this.Data = data;
    }

    public void Research (Upgrade upgrade, int rank = 0) {

    }

    public void Research (UpgradeIdentifier identifier, int rank = 0) {
        Debug.LogWarning (
            string.Format (
                "Trying to research '{0}' on '{1}', but it does not have the Researcher trait.",
                identifier,
                this.Building.Type
            )
        );
    }
}
