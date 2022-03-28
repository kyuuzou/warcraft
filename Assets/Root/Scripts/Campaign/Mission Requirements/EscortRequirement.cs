using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscortRequirement : MissionRequirement {

    [SerializeField]
    private UnitType type;

    [SerializeField]
    private int quantity;

    [SerializeField]
    private Rect exitArea;

    private Map map;

    public override void Initialize (Mission mission) {
        base.Initialize (mission);
        
        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.map = serviceLocator.Map;

        this.Surrogate.StartCoroutine (this.Validate ());
    }

    private IEnumerator Validate () {
        do {
            int count = 0;

            foreach (Unit unit in this.map.GetUnits (this.type)) {
                if (unit.Tile.MapPosition.IsWithinBounds (this.exitArea)) {
                    count ++;
                }
            }

            if (count >= this.quantity && ! this.Satisfied) {
                this.SetSatisfied (true);
            } else if (count < this.quantity) {
                this.SetSatisfied (false);
            }

            yield return new WaitForSeconds (2.0f);
        } while (true);
    }

}
