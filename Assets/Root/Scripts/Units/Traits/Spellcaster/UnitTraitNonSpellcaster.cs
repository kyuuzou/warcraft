using System;
using UnityEngine;

public class UnitTraitNonSpellcaster : UnitTrait, IUnitTraitSpellcaster {

    public UnitTraitDataNonSpellcaster Data { get; private set; }

    public override bool IsNullObject {
        get { return true; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Spellcaster; }
    }

    public void Cast (SpellType spellType) {

    }

    public void Cast (SpellType spellType, Building target, MapTile tile) {

    }

    public void Cast (SpellType spellType, MapTile tile) {

    }

    public void Cast (SpellType spellType, Unit target, MapTile tile) {
        
    }
    
    public void Initialize (Unit unit, UnitTraitDataNonSpellcaster data) {
        base.Initialize (unit);
        
        this.Data = data;
    }

    public bool MayCast (SpellType type) {
        return false;
    }

    public void OnOrderAccepted () {
        
    }
    
    public bool RequiresTarget (SpellType type) {
        return false;
    }
}
