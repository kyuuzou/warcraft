public class UnitTraitDataInteractive : UnitTraitData {

    public override UnitTraitType Type {
        get { return UnitTraitType.Interactive; }
    }

    public override UnitTrait AddTrait(Unit unit) {
        UnitTraitInteractive trait = unit.gameObject.AddComponent<UnitTraitInteractive>();
        trait.Initialize(unit, UnitTraitDataInteractive.Instantiate(this));

        unit.SetTrait<IUnitTraitInteractive>(trait);

        return trait;
    }
}
