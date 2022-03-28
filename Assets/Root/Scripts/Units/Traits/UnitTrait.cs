using UnityEngine;
using System.Collections;

public abstract class UnitTrait : SpawnableTrait {

    public virtual bool IsNullObject {
        get { return false; }
    }

    public Unit Unit { get; private set; }

    public abstract UnitTraitType Type { get; }

    protected void Initialize (Unit unit) {
        this.Unit = unit;
    }
}
