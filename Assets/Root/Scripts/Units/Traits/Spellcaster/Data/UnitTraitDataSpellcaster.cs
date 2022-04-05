using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTraitDataSpellcaster : UnitTraitData {

    [SerializeField]
    private Spell[] spells;
    public Spell[] Spells {
        get { return this.spells; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Spellcaster; }
    }

    public override UnitTrait AddTrait (Unit unit) {
        UnitTraitSpellcaster trait = unit.gameObject.AddComponent<UnitTraitSpellcaster> ();
        trait.Initialize (unit, UnitTraitDataSpellcaster.Instantiate (this));
        
        unit.SetTrait<IUnitTraitSpellcaster> (trait);

        return trait;
    }
}
