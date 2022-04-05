using System.Collections;
using UnityEngine;

public class UnitTraitDataNonSpellcaster : UnitTraitData {

    public override UnitTraitType Type {
        get { return UnitTraitType.Spellcaster; }
    }

    public override UnitTrait AddTrait (Unit unit) {
        UnitTraitNonSpellcaster trait = unit.gameObject.AddComponent<UnitTraitNonSpellcaster> ();
        trait.Initialize (unit, UnitTraitDataNonSpellcaster.Instantiate (this));
        
        unit.SetTrait<IUnitTraitSpellcaster> (trait);

        return trait;
    }
}
