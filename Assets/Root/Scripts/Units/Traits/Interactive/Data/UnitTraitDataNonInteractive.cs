using System.Collections;
using UnityEngine;

public class UnitTraitDataNonInteractive : UnitTraitData {

    public override UnitTraitType Type {
        get { return UnitTraitType.Interactive; }
    }

    public override UnitTrait AddTrait (Unit unit) {
        UnitTraitNonInteractive trait = unit.gameObject.AddComponent<UnitTraitNonInteractive> ();
        trait.Initialize (unit, (UnitTraitDataNonInteractive) UnitTraitDataNonInteractive.Instantiate (this));
        
        unit.SetTrait<IUnitTraitInteractive> (trait);

        return trait;
    }
}
