public class UnitTraitDataNonBuilder : UnitTraitData {

    public override UnitTraitType Type {
        get { return UnitTraitType.Builder; }
    }

    public override UnitTrait AddTrait(Unit unit) {
        UnitTraitNonBuilder trait = unit.gameObject.AddComponent<UnitTraitNonBuilder>();
        trait.Initialize(unit, UnitTraitDataNonBuilder.Instantiate(this));

        unit.SetTrait<IUnitTraitBuilder>(trait);

        return trait;
    }
}
