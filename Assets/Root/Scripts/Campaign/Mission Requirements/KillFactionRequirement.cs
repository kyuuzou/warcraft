using System.Collections;
using UnityEngine;

public class KillFactionRequirement : MissionRequirement {

    [SerializeField]
    private FactionIdentifier identifier;

    private Faction faction;
    private Map map;

    public override void Initialize (Mission mission) {
        base.Initialize (mission);
        
        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.map = serviceLocator.Map;

        this.faction = serviceLocator.GameController.GetFaction (this.identifier);

        this.map.RegisterUnitRemoved (this.ValidateUnitRemoved);
    }

    private void Validate () {
        int unitCount = this.faction.GetUnits ().Count;

        if (unitCount == 0) {
            if (! this.Satisfied) {
                this.SetSatisfied (true);
            }
        } else {
            this.SetSatisfied (false);
        }
    }

    private void ValidateUnitRemoved (object sender, UnitRemovedArgs args) {
        this.Validate ();
    }
}
