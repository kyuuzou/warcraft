using System.Collections;
using UnityEngine;

public class UnitTraitDataNonAttacker : UnitTraitData {

    public override UnitTraitType Type {
        get { return UnitTraitType.Attacker; }
    }

    public override UnitTrait AddTrait (Unit unit) {
        UnitTraitNonAttacker trait = unit.gameObject.AddComponent<UnitTraitNonAttacker> ();
        trait.Initialize (unit, (UnitTraitDataNonAttacker) UnitTraitDataNonAttacker.Instantiate (this));
        
        unit.SetTrait<IUnitTraitAttacker> (trait);

        return trait;
    }
}
