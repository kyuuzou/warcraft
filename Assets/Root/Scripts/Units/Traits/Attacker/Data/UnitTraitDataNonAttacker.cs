public class UnitTraitDataNonAttacker : UnitTraitData {

    public override UnitTraitType Type {
        get { return UnitTraitType.Attacker; }
    }

    public override UnitTrait AddTrait(Unit unit) {
        UnitTraitNonAttacker trait = unit.gameObject.AddComponent<UnitTraitNonAttacker>();
        trait.Initialize(unit, UnitTraitDataNonAttacker.Instantiate(this));

        unit.SetTrait<IUnitTraitAttacker>(trait);

        return trait;
    }
}
