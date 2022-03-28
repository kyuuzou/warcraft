using UnityEngine;

public class UnitTraitNonDecaying : UnitTrait, IUnitTraitDecaying {

    public UnitTraitDataNonDecaying Data { get; private set; }

    public override bool IsNullObject {
        get { return true; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Decaying; }
    }

    public void Initialize (Unit unit, UnitTraitDataNonDecaying data) {
        base.Initialize (unit);
        
        this.Data = data;
    }

    public void OnOrderAccepted () {
        
    }
}
