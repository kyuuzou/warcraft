public class UnitTraitDataNonMoving : UnitTraitData {

    public override UnitTraitType Type {
        get { return UnitTraitType.Moving; }
    }

    public override UnitTrait AddTrait(Unit unit) {
        UnitTraitNonMoving trait = unit.gameObject.AddComponent<UnitTraitNonMoving>();
        trait.Initialize(unit, UnitTraitDataNonMoving.Instantiate(this));

        unit.SetTrait<IUnitTraitMoving>(trait);

        return trait;
    }
}
