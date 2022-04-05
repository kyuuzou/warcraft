using System.Collections;
using UnityEngine;

public class UnitTraitDataDecaying : UnitTraitData {

    [SerializeField]
    private int decayRate = 45;
    public int DecayRate {
        get { return this.decayRate; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Decaying; }
    }

    public override UnitTrait AddTrait (Unit unit) {
        UnitTraitDecaying trait = unit.gameObject.AddComponent<UnitTraitDecaying> ();
        trait.Initialize (unit, UnitTraitDataDecaying.Instantiate (this));
        
        unit.SetTrait<IUnitTraitDecaying> (trait);

        return trait;
    }
}
