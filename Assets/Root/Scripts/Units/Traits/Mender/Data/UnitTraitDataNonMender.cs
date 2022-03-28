using System.Collections;
using UnityEngine;

public class UnitTraitDataNonMender : UnitTraitData {

    public override UnitTraitType Type {
        get { return UnitTraitType.Mender; }
    }

    public override UnitTrait AddTrait (Unit unit) {
        UnitTraitNonMender trait = unit.gameObject.AddComponent<UnitTraitNonMender> ();
        trait.Initialize (unit, (UnitTraitDataNonMender) UnitTraitDataNonMender.Instantiate (this));
        
        unit.SetTrait<IUnitTraitMender> (trait);

        return trait;
    }
}
