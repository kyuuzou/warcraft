public class UnitTraitDataNonDecaying : UnitTraitData {

    public override UnitTraitType Type {
        get { return UnitTraitType.Decaying; }
    }

    public override UnitTrait AddTrait(Unit unit) {
        UnitTraitNonDecaying trait = unit.gameObject.AddComponent<UnitTraitNonDecaying>();
        trait.Initialize(unit, UnitTraitDataNonDecaying.Instantiate(this));

        unit.SetTrait<IUnitTraitDecaying>(trait);

        return trait;
    }
}
