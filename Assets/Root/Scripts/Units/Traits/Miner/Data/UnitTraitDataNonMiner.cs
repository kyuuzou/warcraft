using System.Collections;
using UnityEngine;

public class UnitTraitDataNonMiner : UnitTraitData {
    
    public override UnitTraitType Type {
        get { return UnitTraitType.Miner; }
    }
    
    public override UnitTrait AddTrait (Unit unit) {
        UnitTraitNonMiner trait = unit.gameObject.AddComponent<UnitTraitNonMiner> ();
        trait.Initialize (unit, UnitTraitDataNonMiner.Instantiate (this));
        
        unit.SetTrait<IUnitTraitMiner> (trait);
        
        return trait;
    }
}
