using System.Collections;
using UnityEngine;

public class UnitTraitDataNonShooter : UnitTraitData {

    public override UnitTraitType Type {
        get { return UnitTraitType.Shooter; }
    }

    public override UnitTrait AddTrait (Unit unit) {
        UnitTraitNonShooter trait = unit.gameObject.AddComponent<UnitTraitNonShooter> ();
        trait.Initialize (unit, (UnitTraitDataNonShooter) UnitTraitDataNonShooter.Instantiate (this));
        
        unit.SetTrait<IUnitTraitShooter> (trait);

        return trait;
    }
}
