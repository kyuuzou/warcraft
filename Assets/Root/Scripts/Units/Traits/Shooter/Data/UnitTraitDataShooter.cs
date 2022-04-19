using UnityEngine;

public class UnitTraitDataShooter : UnitTraitData {

    [SerializeField]
    private Muzzle[] muzzles;
    public Muzzle[] Muzzles {
        get { return this.muzzles; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Shooter; }
    }

    public override UnitTrait AddTrait(Unit unit) {
        UnitTraitShooter trait = unit.gameObject.AddComponent<UnitTraitShooter>();
        trait.Initialize(unit, UnitTraitDataShooter.Instantiate(this));

        unit.SetTrait<IUnitTraitShooter>(trait);

        return trait;
    }
}
