using UnityEngine;

public class RazeFactionRequirement : MissionRequirement {

    [SerializeField]
    private FactionIdentifier identifier;

    private Faction faction;
    private Map map;

    public override void Initialize(Mission mission) {
        base.Initialize(mission);

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.map = serviceLocator.Map;

        this.faction = serviceLocator.GameController.GetFaction(this.identifier);

        this.map.RegisterBuildingRemoved(this.ValidateBuildingRemoved);
    }

    private void Validate() {
        int buildingCount = this.faction.GetBuildings().Count;

        if (buildingCount == 0) {
            if (!this.Satisfied) {
                this.SetSatisfied(true);
            }
        } else {
            this.SetSatisfied(false);
        }
    }

    private void ValidateBuildingRemoved(object sender, BuildingRemovedArgs args) {
        this.Validate();
    }
}
