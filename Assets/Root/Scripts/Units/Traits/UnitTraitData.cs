using System.Collections;
using UnityEngine;

public abstract class UnitTraitData : TraitData {
    
    public abstract UnitTraitType Type { get; }

    public abstract UnitTrait AddTrait (Unit unit);
}
